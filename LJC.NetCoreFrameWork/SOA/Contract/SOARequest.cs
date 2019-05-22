using System;
using System.Collections.Generic;
using System.Text;

namespace LJC.NetCoreFrameWork.SOA.Contract
{
    internal class SOARequest
    {
        public int ServiceNo
        {
            get;
            set;
        }

        public int FuncId
        {
            get;
            set;
        }

        public DateTime ReqestTime
        {
            get;
            set;
        }

        public byte[] Param
        {
            get;
            set;
        }
    }
}
