using System;
using System.Collections.Generic;
using System.Text;

namespace LJC.NetCoreFrameWork.SOA.Contract
{
    internal class UnRegisterServiceResponse
    {
        public bool IsSuccess
        {
            get;
            set;
        }

        public string ErrMsg
        {
            get;
            set;
        }
    }
}
