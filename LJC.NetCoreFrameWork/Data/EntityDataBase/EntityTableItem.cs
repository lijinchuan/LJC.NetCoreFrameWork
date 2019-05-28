using System;
using System.Collections.Generic;
using System.Text;

namespace LJC.NetCoreFrameWork.Data.EntityDataBase
{
    public class EntityTableItem<T> where T : new()
    {
        public byte Flag
        {
            get;
            set;
        }

        public EntityTableItem()
        {

        }

        public EntityTableItem(T item)
        {
            Data = item;
            Flag = (byte)EntityTableItemFlag.Ok;
        }

        public T Data
        {
            get;
            set;
        }
    }
}
