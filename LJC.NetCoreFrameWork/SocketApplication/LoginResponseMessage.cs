﻿using System;
using System.Collections.Generic;
using System.Text;

namespace LJC.NetCoreFrameWork.SocketApplication
{
    internal class LoginResponseMessage
    {
        public string LoginID
        {
            get;
            set;
        }

        public bool LoginResult
        {
            get;
            set;
        }

        public string SessionID
        {
            get;
            set;
        }

        public int SessionTimeOut
        {
            get;
            set;
        }

        public int HeadBeatInterVal
        {
            get;
            set;
        }

        public string LoginFailReson
        {
            get;
            set;
        }
    }
}
