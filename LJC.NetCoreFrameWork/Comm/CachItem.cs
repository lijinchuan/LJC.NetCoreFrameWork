using System;
using System.Collections.Generic;
using System.Text;

namespace LJC.NetCoreFrameWork.Comm
{
    [Serializable]
    public class CachItem
    {
        public DateTime CachTime;
        public object CachObj;
    }
}
