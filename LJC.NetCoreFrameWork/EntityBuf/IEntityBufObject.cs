using System;
using System.Collections.Generic;
using System.Text;

namespace LJC.NetCoreFrameWork.EntityBuf
{
    public interface IEntityBufObject
    {
        byte[] Serialize();
        IEntityBufObject DeSerialize(byte[] bytes);
    }
}
