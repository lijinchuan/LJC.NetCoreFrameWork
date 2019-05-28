using System;
using System.Collections.Generic;
using System.Text;

namespace LJC.NetCoreFrameWork.IO.TextReaderWriter
{
    public enum ObjTextReaderWriterEncodeType : byte
    {
        jsongzip = 1,
        json,
        protobuf,
        //扩展的功能，支持回读
        protobufex,
        jsonbuf,
        //扩展的功能，支持回读
        jsonbufex,
        entitybuf,
        entitybufex,
        entitybuf2
    }
}
