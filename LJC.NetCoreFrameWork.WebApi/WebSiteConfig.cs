using System;
using System.Collections.Generic;
using System.Text;

namespace LJC.NetCoreFrameWork.WebApi
{
    [Serializable]
    public class WebSiteConfig
    {
        public string[] Host
        {
            get;
            set;
        }

        public int Port
        {
            get;
            set;
        }
    }
}
