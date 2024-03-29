﻿using LJC.NetCoreFrameWork.EntityBuf;
using LJC.NetCoreFrameWork.SOA.Contract;
using LJC.NetCoreFrameWork.SocketApplication;
using LJC.NetCoreFrameWork.SocketApplication.SocketSTD;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace LJC.NetCoreFrameWork.SOA
{
    public class ESBClient : SessionClient
    {
        private static ESBClientPoolManager _clientmanager = new ESBClientPoolManager();
        private static Dictionary<int, List<ESBClientPoolManager>> _esbClientDicManager = new Dictionary<int, List<ESBClientPoolManager>>();
        private static Dictionary<int, List<ESBUdpClient>> _esbUdpClientDic = new Dictionary<int, List<ESBUdpClient>>();

        public static event Action<Contract.SOANoticeClientMessage> OnNotice;

        public ESBClient(string serverIP, int serverPort, bool startSession = true, bool isSecurity = false)
            : base(serverIP, serverPort, isSecurity, startSession)
        {
        }

        internal ESBClient()
            : base(ESBConfig.ReadConfig().ESBServer, ESBConfig.ReadConfig().ESBPort, ESBConfig.ReadConfig().IsSecurity, ESBConfig.ReadConfig().AutoStart)
        {

        }

        public string GetESBServer()
        {
            return this.ipString;
        }

        public int GetESBPort()
        {
            return this.ipPort;
        }

        private SessionMessageApp clientSession
        {
            get;
            set;
        }

        protected override void ReciveMessage(Message message)
        {
            if (message.IsMessage((int)SOAMessageType.SOANoticeClientMessage))
            {
                var notice = message.GetMessageBody<Contract.SOANoticeClientMessage>();
                if (OnNotice != null)
                {
                    Task.Run(() => OnNotice(notice));
                }
                return;
            }

            base.ReciveMessage(message);
        }

        internal T DoRequest<T>(int serviceno, int funcid, object param)
        {
            SOARequest request = new SOARequest();
            request.ServiceNo = serviceno;
            request.FuncId = funcid;
            if (param == null)
            {
                request.Param = null;
            }
            else
            {
                request.Param = EntityBufCore.Serialize(param);
            }

            Message msg = new Message((int)SOAMessageType.DoSOARequest);
            msg.MessageHeader.TransactionID = SocketApplicationComm.GetSeqNum();
            msg.MessageBuffer = EntityBufCore.Serialize(request);

            T result = SendMessageAnsy<T>(msg);
            return result;
        }

        internal T DoRedirectRequest<T>(int messageType, object request)
        {
            Message msg = new Message(messageType);
            msg.MessageHeader.TransactionID = SocketApplicationComm.GetSeqNum();
            msg.MessageBuffer = EntityBufCore.Serialize(request);

            T result = SendMessageAnsy<T>(msg);
            return result;
        }

        internal T DoRedirectRequest<T>(int serviceno, int funcid, object param)
        {
            SOARedirectRequest request = new SOARedirectRequest();
            request.ServiceNo = serviceno;
            request.FuncId = funcid;
            if (param == null)
            {
                request.Param = null;
            }
            else
            {
                request.Param = EntityBufCore.Serialize(param);
            }

            Message msg = new Message((int)SOAMessageType.DoSOARedirectRequest);
            msg.MessageHeader.TransactionID = SocketApplicationComm.GetSeqNum();
            msg.MessageBuffer = EntityBufCore.Serialize(request);

            T result = SendMessageAnsy<T>(msg);
            return result;
        }

        protected override byte[] DoMessage(Message message)
        {
            if (message.IsMessage((int)SOAMessageType.DoSOAResponse))
            {
                var resp = message.GetMessageBody<SOAResponse>();
                if (!resp.IsSuccess)
                {
                    BuzException = new Exception(resp.ErrMsg);
                    //这里最好抛出错误来
                    throw BuzException;
                }
                return resp.Result;
            }
            else if (message.IsMessage((int)SOAMessageType.DoSOARedirectResponse))
            {
                var resp = message.GetMessageBody<SOARedirectResponse>();
                if (!resp.IsSuccess)
                {
                    BuzException = new Exception(resp.ErrMsg);
                    //这里最好抛出错误来
                    throw BuzException;
                }
                return resp.Result;
            }
            return base.DoMessage(message);
        }

        public static T DoSOARequest<T>(int serviceId, int functionId, object param)
        {
            //using (var client = new ESBClient())
            //{
            //    client.StartClient();
            //    client.Error += client_Error;
            //    var result = client.DoRequest<T>(serviceId, functionId, param);

            //    return result;
            //}

            var result = _clientmanager.RandClient().DoRequest<T>(serviceId, functionId, param);

            return result;
        }

        private static int OrderIp(string ip)
        {
            if (ip.StartsWith("192.168.0."))
            {
                return 0;
            }

            if (ip.StartsWith("192.168.1."))
            {
                return 10;
            }

            if (ip.StartsWith("192.168."))
            {
                return 50;
            }

            if (ip.StartsWith("172."))
            {
                return 100;
            }

            if (ip.StartsWith("10."))
            {
                return 200;
            }

            return 300;
        }

        public static T DoSOARequest2<T>(int serviceId, int functionId, object param)
        {
            List<ESBUdpClient> udpclientlist = null;
            if (!_esbUdpClientDic.TryGetValue(serviceId, out udpclientlist))
            {
                bool takecleint = false;
                lock (_esbUdpClientDic)
                {
                    if (!_esbUdpClientDic.TryGetValue(serviceId, out udpclientlist))
                    {
                        takecleint = true;
                        if (!_esbClientDicManager.ContainsKey(serviceId))
                        {
                            _esbClientDicManager.Add(serviceId, null);
                        }
                        _esbUdpClientDic.Add(serviceId, null);
                    }
                }

                if (takecleint)
                {
                    Task.Run(() =>
                    {
                        var respserviceinfo = DoSOARequest<GetRegisterServiceInfoResponse>(Consts.ESBServerServiceNo, Consts.FunNo_GetRegisterServiceInfo, new GetRegisterServiceInfoRequest
                        {
                            ServiceNo = serviceId
                        });

                        if (respserviceinfo.Infos != null && respserviceinfo.Infos.Length > 0)
                        {
                            List<ESBClientPoolManager> poollist = new List<ESBClientPoolManager>();
                            List<ESBUdpClient> udppoollist = new List<ESBUdpClient>();
                            foreach (var info in respserviceinfo.Infos)
                            {
                                if (info.RedirectUdpIps != null)
                                {
                                    foreach (var ip in info.RedirectUdpIps.OrderBy(p => OrderIp(p)))
                                    {
                                        try
                                        {
                                            var client = new ESBUdpClient(ip, info.RedirectUdpPort);
                                            client.Error += (ex) =>
                                            {
                                                if (ex is System.Net.WebException)
                                                {
                                                    client.Dispose();
                                                    lock (_esbUdpClientDic)
                                                    {
                                                        _esbUdpClientDic.Remove(serviceId);
                                                    }
                                                }
                                            };

                                            client.StartClient();
                                            client.Login(null, null);
                                            int trytimes = 0;
                                            var maxTryTimes = 10;
                                            var success = false;
                                            while (trytimes < maxTryTimes) 
                                            {
                                                System.Threading.Thread.Sleep(10);
                                                if (client.IsLogin)
                                                {
                                                    var resp = client.DoRedirectRequest<Contract.QueryServiceNoResponse>((int)SOAMessageType.QueryServiceNo, null);
                                                    if (resp.ServiceNo == serviceId)
                                                    {
                                                        success = true;
                                                        udppoollist.Add(client);
                                                    }
                                                    else
                                                    {

                                                    }
                                                    break;
                                                }
                                                trytimes++;
                                            }
                                            if (!success)
                                            {
                                                client.Dispose();
                                                //LogHelper.Instance.Debug(string.Format("创建udp客户端失败:{0},端口{1}", ip, info.RedirectUdpPort));
                                                throw new TimeoutException();
                                            }
                                        }
                                        catch
                                        {

                                        }
                                    }
                                }

                                if (udppoollist.Count == 0 && info.RedirectTcpIps != null)
                                {
                                    foreach (var ip in info.RedirectTcpIps.OrderBy(p => OrderIp(p)))
                                    {
                                        try
                                        {
                                            var client = new ESBClient(ip, info.RedirectTcpPort, false);
                                            client.Error += (ex) =>
                                            {
                                                if (ex is System.Net.WebException)
                                                {
                                                    client.CloseClient();
                                                    client.Dispose();
                                                    lock (_esbClientDicManager)
                                                    {
                                                        _esbClientDicManager.Remove(serviceId);
                                                    }
                                                }
                                            };
                                            if (client.StartSession())
                                            {
                                                var resp = client.DoRedirectRequest<Contract.QueryServiceNoResponse>((int)SOAMessageType.QueryServiceNo, null);
                                                if (resp.ServiceNo == serviceId)
                                                {
                                                    poollist.Add(new ESBClientPoolManager(5, (idx) =>
                                                    {
                                                        if (idx == 0)
                                                        {
                                                            return client;
                                                        }
                                                        var newclient = new ESBClient(ip, info.RedirectTcpPort, false);
                                                        newclient.StartSession();
                                                        newclient.Error += (ex) =>
                                                        {
                                                            if (ex is System.Net.WebException
                                                            || ex is System.Net.Sockets.SocketException
                                                            || !newclient.socketClient.Connected)
                                                            {
                                                                try
                                                                {
                                                                    newclient.CloseClient();
                                                                    newclient.Dispose();
                                                                }
                                                                catch
                                                                {

                                                                }
                                                                lock (_esbClientDicManager)
                                                                {
                                                                    _esbClientDicManager.Remove(serviceId);
                                                                }
                                                            }
                                                        };
                                                        return newclient;
                                                    }));
                                                    //LogHelper.Instance.Debug(string.Format("创建tcp客户端成功:{0},端口{1}", ip, info.RedirectTcpPort));
                                                    break;
                                                }
                                                else
                                                {
                                                    client.CloseClient();
                                                    client.Dispose();
                                                }
                                            }
                                        }
                                        catch
                                        {
                                            //LogHelper.Instance.Debug(string.Format("创建tcp客户端失败:{0},端口{1}", ip, info.RedirectTcpPort));
                                        }
                                    }
                                }
                            }

                            if (udppoollist.Count > 0)
                            {
                                lock (_esbUdpClientDic)
                                {
                                    _esbUdpClientDic[serviceId] = udppoollist;
                                }
                            }
                            if (poollist.Count > 0)
                            {
                                lock (_esbClientDicManager)
                                {
                                    _esbClientDicManager[serviceId] = poollist;
                                }
                            }
                        }
                    });
                }
            }

            if (udpclientlist != null && udpclientlist.Count > 0)
            {
                return udpclientlist.First().DoRequest<T>(serviceId, functionId, param);
            }
            else
            {
                List<ESBClientPoolManager> poolmanagerlist = null;
                if (_esbClientDicManager.TryGetValue(serviceId, out poolmanagerlist) && poolmanagerlist != null && poolmanagerlist.Count > 0)
                {
                    //Console.WriteLine("直连了");
                    var poolmanager = poolmanagerlist.Count == 1 ? poolmanagerlist[0]
                    : poolmanagerlist[new Random().Next(0, poolmanagerlist.Count)];

                    var client = poolmanager.RandClient();
                    //LogHelper.Instance.Debug("功能"+serviceId+"直连" + client.ipString + ":" + client.ipPort);
                    return client.DoRedirectRequest<T>(serviceId, functionId, param);
                }
                else
                {
                    return DoSOARequest<T>(serviceId, functionId, param);
                }
            }
        }

        static void UDPClient_Error(Exception e)
        {
            //LogHelper.Instance.Error("UDPSOA请求错误", e);
        }

        static void client_Error(Exception e)
        {
            //LogHelper.Instance.Error("SOA请求错误", e);
        }

        public static void Close()
        {
            _clientmanager.Dispose();

            foreach (var man in _esbClientDicManager)
            {
                foreach (var m in man.Value)
                {
                    m.Dispose();
                }
            }

            foreach (var item in _esbUdpClientDic)
            {
                foreach (var c in item.Value)
                {
                    c.Dispose();
                }
            }
        }
    }
}
