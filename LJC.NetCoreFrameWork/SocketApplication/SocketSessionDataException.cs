using System;
using System.Collections.Generic;
using System.Text;

namespace LJC.NetCoreFrameWork.SocketApplication
{
    public class SocketSessionDataException : Exception
    {
        public SocketSessionDataException(string message)
            : base(message)
        {

        }
    }
}
