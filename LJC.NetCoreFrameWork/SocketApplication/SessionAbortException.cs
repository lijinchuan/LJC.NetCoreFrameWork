using System;
using System.Collections.Generic;
using System.Text;

namespace LJC.NetCoreFrameWork.SocketApplication
{
    internal class SessionAbortException : Exception
    {
        public SessionAbortException()
            : base()
        {

        }

        public SessionAbortException(string msg)
            : base(msg)
        {

        }
    }
}
