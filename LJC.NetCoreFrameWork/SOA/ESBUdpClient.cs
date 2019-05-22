using LJC.NetCoreFrameWork.EntityBuf;
using LJC.NetCoreFrameWork.SOA.Contract;
using LJC.NetCoreFrameWork.SocketApplication;
using System;
using System.Collections.Generic;
using System.Text;

namespace LJC.NetCoreFrameWork.SOA
{
    public class ESBUdpClient : SocketApplication.SocketEasyUDP.Client.SessionClient
    {
        private int TimeOutTimes = 0;
        private const int MAXTIMEOUTTIMES = 3;

        public ESBUdpClient(string host, int port) : base(host, port)
        {

        }

        internal T DoRequest<T>(int funcid, object param)
        {
            SOARedirectRequest request = new SOARedirectRequest();
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

            try
            {
                var resp = SendMessageAnsy<SOARedirectResponse>(msg, timeOut: 5000);
                if (TimeOutTimes > 0)
                {
                    TimeOutTimes--;
                }
                if (resp.IsSuccess)
                {
                    return EntityBuf.EntityBufCore.DeSerialize<T>(resp.Result);
                }
                else
                {
                    throw new Exception(resp.ErrMsg);
                }
            }
            catch (TimeoutException ex)
            {
                TimeOutTimes++;

                if (TimeOutTimes > MAXTIMEOUTTIMES)
                {
                    OnError(new System.Net.WebException("一段时间内连续超时，可能出现网络问题"));
                }

                throw ex;
            }
        }
    }
}
