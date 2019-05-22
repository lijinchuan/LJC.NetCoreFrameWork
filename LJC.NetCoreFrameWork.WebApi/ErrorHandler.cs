using LJC.NetCoreFrameWork.Net.HTTP.Server;
using System;
using System.Collections.Generic;
using System.Text;

namespace LJC.NetCoreFrameWork.WebApi
{
    public class ErrorHandler : IHttpHandler
    {
        private Exception _ex;

        public bool IsReusable
        {
            get { throw new NotImplementedException(); }
        }

        public ErrorHandler(Exception ex)
        {
            this._ex = ex;
        }

        public bool Process(HttpServer server, HttpRequest request, HttpResponse response)
        {
            APIResult<string> result = new APIResult<string>
            {
                ResponseBody = _ex.StackTrace,
                ResultCode = 0,
                ResultMessage = _ex.Message
            };
            response.Content = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            response.ReturnCode = 200;
            return true;
        }
    }
}
