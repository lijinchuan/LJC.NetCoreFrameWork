using System;
using System.Collections.Generic;
using System.Text;

namespace LJC.NetCoreFrameWork.SOA.Contract
{
    public class SOAResponse
    {
        private bool _isSuccess = false;
        public bool IsSuccess
        {
            get
            {
                return _isSuccess;
            }
            set
            {
                _isSuccess = value;
            }
        }

        public DateTime ResponseTime
        {
            get;
            set;
        }

        public string ErrMsg
        {
            get;
            set;
        }

        public byte[] Result
        {
            get;
            set;
        }
    }
}
