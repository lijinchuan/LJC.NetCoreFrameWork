﻿using LJC.NetCoreFrameWork.SocketApplication;
using LJC.NetCoreFrameWork.SocketApplication.SocketSTD;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using LJC.NetCoreFrameWork.SOA.Contract;

namespace LJC.NetCoreFrameWork.SOA
{
    public class ESBService : SessionClient, IService
    {
        private bool SupportTcpServiceRidrect
        {
            get;
            set;
        }

        private bool SupportUDPServiceRedirect
        {
            get;
            set;
        }


        private string ServiceName
        {
            get;
            set;
        }

        private string EndPointName
        {
            get;
            set;
        }

        public ESBService(string serverIP, int serverPort, int sNo, bool supportTcpServiceRedirect = false, bool supportUdpServiceRedirect = false,
            string serviceName = null, string endPointName = null)
            : base(serverIP, serverPort, false)
        {
            this.ServiceNo = sNo;
            this.BeferLogout += this.UnRegisterService;
            this.OnClientReset += ESBService_OnClientReset;

            this.SupportTcpServiceRidrect = supportTcpServiceRedirect;
            this.SupportUDPServiceRedirect = supportUdpServiceRedirect;

            this.ServiceName = serviceName;
            this.EndPointName = endPointName;
        }

        public ESBService(int sNo, bool supportTcpServiceRidrect = false, bool supportUdpServiceRedirect = false,
            string serviceName = null, string endPointName = null)
           : base(ESBConfig.ReadConfig().ESBServer, ESBConfig.ReadConfig().ESBPort, false)
        {
            this.ServiceNo = sNo;
            this.BeferLogout += this.UnRegisterService;
            this.OnClientReset += ESBService_OnClientReset;

            this.SupportTcpServiceRidrect = supportTcpServiceRidrect;
            this.SupportUDPServiceRedirect = supportUdpServiceRedirect;

            this.ServiceName = serviceName;
            this.EndPointName = endPointName;
        }

        void ESBService_OnClientReset()
        {
            while (true)
            {
                try
                {
                    if (RegisterService())
                    {
                        //LogHelper.Instance.Info("连接重置后注册服务成功");
                        break;
                    }
                    else
                    {
                        //LogHelper.Instance.Info("连接重置后注册服务失败");
                    }
                }
                catch (Exception ex)
                {
                    //LogHelper.Instance.Error("连接重置后注册服务失败", ex);
                }
                Thread.Sleep(3000);
            }
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        public void StartService()
        {
            while (!StartClient())
            {
                Thread.Sleep(1000);
            }
            Login(null, null);
        }

        public int ServiceNo
        {
            get;
            private set;
        }

        protected sealed override void ReciveMessage(Message message)
        {
            if (message.IsMessage((int)SOAMessageType.QueryServiceNo))
            {
                var responseMsg = new Message((int)SOAMessageType.QueryServiceNo);
                responseMsg.MessageHeader.TransactionID = message.MessageHeader.TransactionID;
                QueryServiceNoResponse responseBody = new QueryServiceNoResponse();
                responseBody.ServiceNo = ServiceNo;

                responseMsg.SetMessageBody(responseBody);
                SendMessage(responseMsg);

                return;
            }
            else if (message.IsMessage((int)SOAMessageType.DoSOATransferRequest))
            {
                SOATransferRequest request = null;
                try
                {
                    var responseMsg = new Message((int)SOAMessageType.DoSOATransferResponse);
                    responseMsg.MessageHeader.TransactionID = message.MessageHeader.TransactionID;
                    SOATransferResponse responseBody = new SOATransferResponse();
                    request = message.GetMessageBody<SOATransferRequest>();
                    responseBody.ClientTransactionID = request.ClientTransactionID;
                    responseBody.ClientId = request.ClientId;

                    //if (SocketApplicationEnvironment.TraceMessage)
                    //{
                    //    LogHelper.Instance.Debug(string.Format("接收服务请求,请求号:{0}", request.ClientTransactionID));
                    //}

                    try
                    {
                        var result = DoResponse(request.FundId, request.Param, request.ClientId);
                        responseBody.Result = EntityBuf.EntityBufCore.Serialize(result);
                        responseBody.IsSuccess = true;

                        //if (SocketApplicationEnvironment.TraceMessage)
                        //{
                        //    LogHelper.Instance.Debug(string.Format("处理请求：请求号:{0},客户端请求号:{1},服务号:{2},功能号:{3},结果:{4},序列化结果:{5}",
                        //        responseMsg.MessageHeader.TransactionID, request.ClientTransactionID, ServiceNo, request.FundId, Comm.JsonUtil<object>.Serialize(result), Convert.ToBase64String(responseBody.Result)));

                        //}
                    }
                    catch (Exception ex)
                    {
                        responseBody.IsSuccess = false;
                        responseBody.ErrMsg = ex.Message;

                        //LogHelper.Instance.Error(string.Format("服务转发出错,请求号:{0},服务号:{1},功能号:{2}",
                        //    request.ClientTransactionID, ServiceNo, request.FundId), ex);

                    }

                    responseMsg.SetMessageBody(responseBody);

                    SendMessage(responseMsg);

                    return;
                }
                catch (Exception ex)
                {
                    //LogHelper.Instance.Error(string.Format("服务转发出错,请求号:{0},服务号:{1},功能号:{2}",
                    //    request == null ? "0" : request.ClientTransactionID,
                    //    ServiceNo, request == null ? 0 : request.FundId), ex);

                    return;
                }
            }

            base.ReciveMessage(message);
        }

        protected override byte[] DoMessage(SocketApplication.Message message)
        {
            if (message.IsMessage((int)SOAMessageType.RegisterService))
            {
                return message.MessageBuffer;
            }
            else if (message.IsMessage((int)SOAMessageType.UnRegisterService))
            {
                return message.MessageBuffer;
            }
            return base.DoMessage(message);
        }

        public virtual object DoResponse(int funcId, byte[] Param, string clientid)
        {
            return null;
        }

        public bool RegisterService()
        {
            if (this.ServiceNo < 0)
                throw new Exception("注册服务失败：服务号不能为负数");

            StartRedirectService();

            Message msg = new Message((int)SOAMessageType.RegisterService);
            msg.MessageHeader.TransactionID = SocketApplicationComm.GetSeqNum();
            RegisterServiceRequest req = new RegisterServiceRequest();
            req.ServiceNo = this.ServiceNo;
            if (SupportTcpServiceRidrect)
            {
                req.RedirectTcpIps = RedirectTcpServiceServer.GetBindIps();
                req.RedirectTcpPort = RedirectTcpServiceServer.GetBindTcpPort();
            }
            if (SupportUDPServiceRedirect)
            {
                req.RedirectUdpIps = RedirectUpdServiceServer.GetBindIps();
                req.RedirectUdpPort = RedirectUpdServiceServer.GetBindUdpPort();
            }

            if (!string.IsNullOrWhiteSpace(this.ServiceName) && !string.IsNullOrWhiteSpace(this.EndPointName))
            {
                msg.AddCustomData(nameof(this.ServiceName), this.ServiceName);
                msg.AddCustomData(nameof(this.EndPointName), this.EndPointName);
            }

            msg.SetMessageBody(req);

            bool boo = SendMessageAnsy<RegisterServiceResponse>(msg).IsSuccess;

            return boo;
        }

        /// <summary>
        /// 发送soa通知
        /// </summary>
        /// <param name="recivers"></param>
        /// <param name="type"></param>
        /// <param name="body"></param>
        /// <param name="needresult"></param>
        /// <returns></returns>
        public SOANoticeResponse SendNotice(string[] recivers, int type, byte[] body, bool needresult)
        {
            if (recivers == null || recivers.Length == 0)
            {
                return new SOANoticeResponse();
            }

            Message m = new Message((int)SOAMessageType.SOANoticeRequest);

            m.SetMessageBody(new Contract.SOANoticeRequest
            {
                NeedResult = needresult,
                NoticeBody = body,
                NoticeType = type,
                ReciveClients = recivers
            });

            if (needresult)
            {
                m.MessageHeader.TransactionID = SocketApplicationComm.GetSeqNum();
                return SendMessageAnsy<Contract.SOANoticeResponse>(m);
            }
            else
            {
                return new SOANoticeResponse
                {
                    IsDone = SendMessage(m)
                };
            }
        }

        public void UnRegisterService()
        {
            Message msg = new Message((int)SOAMessageType.UnRegisterService);
            msg.MessageHeader.TransactionID = SocketApplicationComm.GetSeqNum();
            UnRegisterServiceRequest req = new UnRegisterServiceRequest();
            req.ServiceNo = this.ServiceNo;
            msg.SetMessageBody(req);

            bool boo = SendMessageAnsy<UnRegisterServiceResponse>(msg).IsSuccess;

            if (boo)
            {
                Console.WriteLine("取消注册成功");
            }

        }

        protected override void OnLoginSuccess()
        {
            base.OnLoginSuccess();
            while (true)
            {
                try
                {
                    if (RegisterService())
                    {
                        //LogHelper.Instance.Info("注册服务成功");
                        break;
                    }
                    else
                    {
                        //LogHelper.Instance.Info("注册服务失败");
                    }
                }
                catch (Exception ex)
                {
                    //LogHelper.Instance.Error("注册服务失败", ex);
                }
                Thread.Sleep(3000);
            }
        }

        protected override void OnError(Exception e)
        {
            base.OnError(e);
        }

        protected override void OnSessionResume()
        {
            base.OnSessionResume();

            while (true)
            {
                try
                {
                    if (RegisterService())
                    {
                        //LogHelper.Instance.Info("连接恢复后注册服务成功");
                        break;
                    }
                    else
                    {
                        //LogHelper.Instance.Info("连接恢复后注册服务失败");
                    }
                }
                catch (Exception ex)
                {
                    //LogHelper.Instance.Error("连接恢复后注册服务失败", ex);
                }

                Thread.Sleep(3000);
            }
        }

        #region 开通直连服务
        private ESBRedirectService RedirectTcpServiceServer = null;
        private ESBUDPService RedirectUpdServiceServer = null;

        public void StartRedirectService()
        {
            var localhost = System.Net.Dns.GetHostName();
            var addrs = System.Net.Dns.GetHostAddresses(localhost);
            List<System.Net.IPAddress> bindips = new List<System.Net.IPAddress>();
            if (addrs != null && addrs.Length > 0)
            {
                foreach (var addr in addrs)
                {
                    if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        bindips.Add(addr);
                    }
                }
            }

            if (addrs.Length > 0)
            {
                int iport = 0;
                if (SupportTcpServiceRidrect && RedirectTcpServiceServer == null)
                {
                    int trytimes = 0;
                    while (true)
                    {
                        try
                        {
                            iport = SocketApplicationComm.GetIdelTcpPort();

                            RedirectTcpServiceServer = new ESBRedirectService(ServiceNo, bindips.Select(p => p.ToString()).ToArray(), iport);
                            RedirectTcpServiceServer.DoResponseAction = DoResponse;
                            RedirectTcpServiceServer.StartServer();
                            break;
                        }
                        catch (Exception ex)
                        {
                            trytimes++;
                            if (trytimes >= 10)
                            {
                                OnError(new Exception("启动tcp直连服务端口失败,已尝试" + trytimes + "次，端口:" + iport, ex));
                                break;
                            }
                        }
                    }
                }

                if (SupportUDPServiceRedirect && this.RedirectUpdServiceServer == null)
                {
                    int trytimes = 0;
                    while (true)
                    {
                        try
                        {
                            iport = SocketApplicationComm.GetIdelUdpPort(iport);

                            RedirectUpdServiceServer = new ESBUDPService(ServiceNo, bindips.Select(p => p.ToString()).ToArray(), iport);
                            RedirectUpdServiceServer.DoResponseAction = DoResponse;
                            RedirectUpdServiceServer.StartServer();
                            break;
                        }
                        catch (Exception ex)
                        {
                            trytimes++;
                            if (trytimes >= 10)
                            {
                                OnError(new Exception("启动udp直连服务端口失败,已尝试" + trytimes + "次，端口:" + iport, ex));
                                break;
                            }
                        }
                    }
                }
            }
        }

        #endregion

    }
}
