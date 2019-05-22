using System;
using System.Collections.Generic;
using System.Text;

namespace LJC.NetCoreFrameWork.SOA
{
    internal interface IService
    {
        bool RegisterService();

        object DoResponse(int funcId, byte[] request, string clientid);

    }
}
