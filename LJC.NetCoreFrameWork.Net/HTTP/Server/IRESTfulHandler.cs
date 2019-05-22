using System;
using System.Collections.Generic;
using System.Text;

namespace LJC.NetCoreFrameWork.Net.HTTP.Server
{
    public interface IRESTfulHandler
    {
        bool Process(HttpServer server, HttpRequest request, HttpResponse response, Dictionary<string, string> param);
    }
}
