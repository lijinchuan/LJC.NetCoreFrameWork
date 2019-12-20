using System;
using System.Collections.Generic;
using System.Text;
using LJC.NetCoreFrameWork.Comm;
using LJC.NetCoreFrameWork.EntityBuf;
using LJC.NetCoreFrameWork.Net.HTTP.Server;

namespace LJC.NetCoreFrameWork.WebApi
{
    public class ApiGenRequestHandler : APIHandler
    {
        private APIHandler _hander;
        private string _apimethodname;

        public ApiGenRequestHandler(string apimethodname, APIHandler hander)
        {
            this._apimethodname = apimethodname;
            this._hander = hander;
        }

        public override bool Process(HttpServer server, HttpRequest request, HttpResponse response)
        {
            string json = string.Empty;

            var apiType = typeof(APIResult<>);

            var apiResultType = _hander._requestType;

            json = JsonUtil<object>.Serialize(EntityBufCore.DeSerialize(apiResultType, EntityBufCoreEx.GenSerialize(apiResultType),false), true);
            StringBuilder sb = new StringBuilder();
            sb.Append("<html>");
            sb.Append("<body>");
            sb.Append("<pre>" + json + "</pre>");


            sb.Append("</body>");
            sb.Append(@"<script>
                           var ifr=parent&&parent.document.getElementById('req');
                           if(ifr)
                           {
                              ifr.height=document.body.scrollHeight;
                           }
                     </script>");

            response.Content = sb.ToString();
            response.ReturnCode = 200;

            return true;
        }
    }
}
