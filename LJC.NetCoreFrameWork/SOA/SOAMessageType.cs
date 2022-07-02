using System;
using System.Collections.Generic;
using System.Text;

namespace LJC.NetCoreFrameWork.SOA
{
    internal enum SOAMessageType : ushort
    {
        DoSOARequest = 100,
        DoSOAResponse,
        DoSOATransferRequest,
        DoSOATransferResponse,
        RegisterService,
        UnRegisterService,
        DoSOARedirectRequest,
        DoSOARedirectResponse,
        SOANoticeRequest,
        SOANoticeResponse,
        SOANoticeClientMessage,
        QueryServiceNo
    }
}
