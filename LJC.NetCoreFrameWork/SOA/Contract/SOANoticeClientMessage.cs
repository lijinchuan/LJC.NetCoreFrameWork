using System;
using System.Collections.Generic;
using System.Text;

namespace LJC.NetCoreFrameWork.SOA.Contract
{
    public class SOANoticeClientMessage
    {
        public int ServiceNo
        {
            get;
            set;
        }

        public int NoticeType
        {
            get;
            set;
        }

        public byte[] NoticeBody
        {
            get;
            set;
        }
    }
}
