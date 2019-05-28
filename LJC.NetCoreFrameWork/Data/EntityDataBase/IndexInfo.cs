using System;
using System.Collections.Generic;
using System.Text;

namespace LJC.NetCoreFrameWork.Data.EntityDataBase
{
    public class IndexInfo
    {
        public string IndexName
        {
            get;
            set;
        }

        public IndexItem[] Indexs
        {
            get;
            set;
        }

        public object[] GetIndexValues(object obj, BigEntityTableMeta meta)
        {
            object[] ret = new object[Indexs.Length];
            for (int i = 0; i < Indexs.Length; i++)
            {
                var val = meta.IndexProperties[Indexs[i].Field].GetValueMethed(obj);
                ret[i] = val;
            }

            return ret;
        }
    }
}
