using System;
using System.Collections.Generic;
using System.Text;

namespace LJC.NetCoreFrameWork.EntityBuf
{
    public class EntityBufException : Exception
    {
        public EntityBufException()
        {

        }

        public EntityBufException(string message)
            : base(message)
        {

        }

        public EntityBufException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
