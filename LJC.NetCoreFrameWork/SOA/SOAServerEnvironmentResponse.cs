using System;
using System.Collections.Generic;
using System.Text;

namespace LJC.NetCoreFrameWork.SOA
{
    public class SOAServerEnvironmentResponse
    {
        public string MachineName { get; set; }

        public string OSVersion { get; set; }

        public int ProcessorCount { get; set; }
    }
}
