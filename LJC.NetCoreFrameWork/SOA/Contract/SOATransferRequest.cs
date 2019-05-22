using System;
using System.Collections.Generic;
using System.Text;

namespace LJC.NetCoreFrameWork.SOA.Contract
{
    internal class SOATransferRequest
    {
        public string ClientId
        {
            get;
            set;
        }

        public string ClientTransactionID
        {
            get;
            set;
        }

        public int FundId
        {
            get;
            set;
        }

        public DateTime RequestTime
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
