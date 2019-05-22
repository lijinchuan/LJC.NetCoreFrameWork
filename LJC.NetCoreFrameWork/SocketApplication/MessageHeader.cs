using System;
using System.Collections.Generic;
using System.Text;

namespace LJC.NetCoreFrameWork.SocketApplication
{
    public class MessageHeader
    {
        public int MessageType
        {
            get;
            set;
        }

        /// <summary>
        /// 流水号
        /// </summary>
        public string TransactionID
        {
            get;
            set;
        }

        public DateTime MessageTime
        {
            get;
            set;
        }
    }
}
