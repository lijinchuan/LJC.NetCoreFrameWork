using System;
using System.Collections.Generic;
using System.Text;

namespace LJC.NetCoreFrameWork.SocketApplication.SocketEasyUDP
{
    public class SendFileCheckResponseMessage
    {
        public string FileName
        {
            get;
            set;
        }

        public long FileLength
        {
            get;
            set;
        }
    }
}
