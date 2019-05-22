using System;
using System.Collections.Generic;
using System.Text;

namespace LJC.NetCoreFrameWork.SOA.Contract
{
    public class GetRegisterServiceInfoResponse
    {
        public int ServiceNo
        {
            get;
            set;
        }

        public RegisterServiceInfo[] Infos
        {
            get;
            set;
        }
    }
}
