using LJC.NetCoreFrameWork.Comm;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Linq;
using LJC.NetCoreFrameWork.Net.HTTP.Server;

namespace LJC.NetCoreFrameWork.WebApi
{
    public class APIFactory
    {
        internal static ConcurrentDictionary<string, APIHandler> apiFunMapper = new ConcurrentDictionary<string, APIHandler>();
        internal static ConcurrentDictionary<string, bool> apiPermission = new ConcurrentDictionary<string, bool>();

        static APIFactory()
        {
            //配置
            ConfigApi();

            //内置api
            Init("LJC.NetCoreFrameWork.WebApi");

        }

        static void ConfigApi()
        {
            var webapiportstr=ConfigHelper.AppConfig("WebApiStartPort");
            int port = 0;
            if (!int.TryParse(webapiportstr, out port) || port <= 0 || port > 65535)
            {
                throw new Exception("站点端口号配置错误");
            }
            Console.WriteLine("网站端口:"+port);
            var server = new HttpServer(new Server(port)).Handlers.Add(new DefalutHttpHandler(request => GetHandler(request, request.Method, request.Url, request.Url)));
        }


        public static IHttpHandler GetHandler(HttpRequest request,string requestType, string url, string pathTranslated)
        {
            try
            {
                var methed = url.Substring(url.LastIndexOf('/') + 1).ToLower();
                
                if ("json".Equals(methed))
                {
                    var urlnodes = url.Split('/');

                    APIHandler fun;
                    methed = urlnodes[urlnodes.Length - 2].ToLower();
                    if (apiFunMapper.TryGetValue(methed, out fun))
                    {
                        if (!fun.ApiMethodProp.IsVisible)
                        {
                            throw new NotSupportedException();
                        }
                        var jsonfun = new APIJsonHandler(methed, fun);
                        jsonfun._ipLimit = ConfigHelper.AppConfig(fun.ApiMethodProp.IpLimitConfig);
                        return jsonfun;
                    }
                    else
                    {
                        throw new NotSupportedException(string.Format("找不到api方法:{0}", methed));
                    }
                }
                else
                {

                    APIHandler fun;
                    if (!apiFunMapper.TryGetValue(methed, out fun))
                    {
                        throw new NotSupportedException(string.Format("找不到api方法:{0}", methed));
                    }

                    if (!string.IsNullOrEmpty(fun.ApiMethodProp.IpLimitConfig))
                    {
                        APIPermission permission = new APIPermission(fun.ApiMethodProp.IpLimitConfig);
                        if (!permission.CheckPermission(request.From.ToString()))
                        {
                            throw new Exception(string.Format("ip[{0}]没有调用权限！", request.From.ToString()));
                        }
                    }

                    return fun;
                }
            }
            catch (Exception ex)
            {
                return new ErrorHandler(ex);
            }
        }

        public void ReleaseHandler(IHttpHandler handler)
        {
            //throw new NotImplementedException();
        }

        public static void Init(params string[] apidomains)
        {
            if (apidomains != null)
            {
                foreach (var apidomain in apidomains)
                {
                    Init(apidomains);
                }
            }
        }

        public static void Init(string apidomain)
        {
            var domain = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(p => p.FullName.Equals(apidomain, StringComparison.OrdinalIgnoreCase));
            if (domain == null)
            {
                domain = AppDomain.CurrentDomain.Load(apidomain);
            }

            if (domain == null)
                throw new Exception("找不到应用程序集:" + apidomain);

            foreach (var mode in domain.GetModules())
            {
                foreach (Type tp in mode.GetTypes())
                {
                    if (tp.IsClass)
                    {
                        foreach (var method in tp.GetMethods())
                        {
                            if (method.IsStatic)
                            {
                                continue;
                            }
                            var apimethod = (APIMethodAttribute)method.GetCustomAttributes(typeof(APIMethodAttribute), true).FirstOrDefault();
                            if (apimethod != null)
                            {
                                string methodname = string.IsNullOrWhiteSpace(apimethod.Aliname) ? method.Name.ToLower() : apimethod.Aliname.ToLower();
                                if ("json".Equals(methodname))
                                {
                                    throw new Exception("json不能用于api方法名");
                                }
                                if (apiFunMapper.ContainsKey(methodname))
                                {
                                    throw new Exception(string.Format("api方法已经被注册:{0}", methodname));
                                }
                                var param = method.GetParameters();

                                var tpInstance = Activator.CreateInstance(tp);
                                if (param.Length == 0)
                                {
                                    apiFunMapper.TryAdd(methodname, new APIEmptyHandler(method.ReturnType, apimethod, () =>
                                    method.Invoke(tpInstance, null)));
                                }
                                else if (param.Length == 1)
                                {
                                    Type t = param[0].ParameterType;
                                    apiFunMapper.TryAdd(methodname, new APIHandler(t, method.ReturnType, apimethod, (o) =>
                                    method.Invoke(tpInstance, new[] { o })));
                                }
                                else
                                {
                                    throw new NotSupportedException("api方法参数太多，建议包装成一个对象传参。");
                                }

                            }
                        }
                    }
                }
            }
        }
    }
}
