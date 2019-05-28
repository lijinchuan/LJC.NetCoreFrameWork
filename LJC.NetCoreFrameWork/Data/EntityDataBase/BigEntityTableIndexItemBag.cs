using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace LJC.NetCoreFrameWork.Data.EntityDataBase
{
    public class BigEntityTableIndexItemBag
    {
        private ConcurrentDictionary<string, Dictionary<long, BigEntityTableIndexItem>> _dics = new ConcurrentDictionary<string, Dictionary<long, BigEntityTableIndexItem>>();
        public ConcurrentDictionary<string, Dictionary<long, BigEntityTableIndexItem>> Dics
        {
            get
            {
                return _dics;
            }
        }

        public DateTime LastUsed
        {
            get;
            set;
        }

        public long LastOffset
        {
            get;
            set;
        }
    }
}
