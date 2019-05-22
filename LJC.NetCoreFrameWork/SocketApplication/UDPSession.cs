using LJC.NetCoreFrameWork.SocketApplication.SocketEasyUDP.Sever;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace LJC.NetCoreFrameWork.SocketApplication
{
    public class UDPSession : Session
    {
        public SessionServer SessionServer
        {
            get;
            set;
        }

        public IPEndPoint EndPoint
        {
            get;
            set;
        }

        public override bool SendMessage(Message msg)
        {

            return SessionServer.SendMessage(msg, this.EndPoint);
        }
    }
}
