using System;
using System.Collections.Generic;
using System.Text;

namespace LJC.NetCoreFrameWork.IO.TextReaderWriter
{
    [Serializable]
    public class LocalFileQueueCfg
    {
        public long LastPos
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlIgnore]
        public DateTime LastChageTime
        {
            get;
            set;
        }

        public string QueueFile
        {
            get;
            set;
        }
    }
}
