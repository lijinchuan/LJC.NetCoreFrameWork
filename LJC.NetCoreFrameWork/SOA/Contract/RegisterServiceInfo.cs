using System;
using System.Collections.Generic;
using System.Text;

namespace LJC.NetCoreFrameWork.SOA.Contract
{
    public class RegisterServiceInfo
    {
        public int ServiceNo
        {
            get;
            set;
        }

        public string[] RedirectTcpIps
        {
            get;
            set;
        }

        public int RedirectTcpPort
        {
            get;
            set;
        }

        public string[] RedirectUdpIps
        {
            get;
            set;
        }

        public int RedirectUdpPort
        {
            get;
            set;
        }
    }
}
