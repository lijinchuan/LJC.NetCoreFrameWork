using System;
using System.Collections.Generic;
using System.Text;

namespace LJC.NetCoreFrameWork.SocketApplication.SocketEasyUDP
{
    public class UDPRevResultMessage
    {
        public long BagId
        {
            get;
            set;
        }

        public int[] Miss
        {
            get;
            set;
        }

        public bool IsReved
        {
            get;
            set;
        }
    }
}
