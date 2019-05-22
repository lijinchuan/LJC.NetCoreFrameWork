using System;
using System.Collections.Generic;
using System.Text;

namespace LJC.NetCoreFrameWork.SocketApplication
{
    public class SendFileMessage
    {
        public string FileName
        {
            get;
            set;
        }

        public long FieSize
        {
            get;
            set;
        }

        public bool IsFinished
        {
            get;
            set;
        }

        public byte[] FileBytes
        {
            get;
            set;
        }
    }
}
