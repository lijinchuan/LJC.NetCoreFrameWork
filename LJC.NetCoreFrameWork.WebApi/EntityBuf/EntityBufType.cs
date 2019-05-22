using LJC.NetCoreFrameWork.Comm;
using LJC.NetCoreFrameWork.EntityBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace LJC.NetCoreFrameWork.WebApi.EntityBuf
{
    public class EntityBufType
    {
        private EntityType _entityType = EntityType.UNKNOWN;
        public EntityType EntityType
        {
            get
            {
                return _entityType;
            }
            set
            {
                _entityType = value;
            }
        }

        /// <summary>
        /// 类名，可以是数组，类或者列表
        /// </summary>
        public Type ValueType
        {
            get;
            set;
        }

        /// <summary>
        /// 类名，只是类
        /// </summary>
        public Type ClassType
        {
            get;
            set;
        }

        public object DefaultValue
        {
            get;
            set;
        }

        /// <summary>
        /// 属性
        /// </summary>
        public PropertyInfoEx Property
        {
            get;
            set;
        }
    }
}
