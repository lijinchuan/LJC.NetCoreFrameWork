using System;
using System.Collections.Generic;
using System.Text;

namespace LJC.NetCoreFrameWork.SocketApplication
{
    internal class LoginRequestMessage
    {
        public string LoginID
        {
            get;
            set;
        }

        public string LoginPwd
        {
            get;
            set;
        }
    }
}
