using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using LJC.NetCoreFrameWork.Comm;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LJC.NetCoreFrameWork.SOA
{
    [Serializable]
    public class ESBConfig
    {
        private static string configfile = "ESBConfig.xml";

        public string ESBServer
        {
            get;
            set;
        }

        public int ESBPort
        {
            get;
            set;
        }

        public bool IsSecurity
        {
            get;
            set;
        }

        public bool AutoStart
        {
            get;
            set;
        }

        private static bool FindFile(string baseDir, string fileName,ref string fullPath)
        {
            Trace.WriteLine("searching:" + baseDir);
            var path = Path.Combine(baseDir, fileName);

            if (File.Exists(path))
            {
                fullPath = path;
                return true;
            }

            foreach(var dir in  Directory.GetDirectories(baseDir))
            {
                if(FindFile(dir, fileName,ref fullPath))
                {
                    return true;
                }
            }

            return false;
        }

        private static ESBConfig _esbConfig = null;
        public static ESBConfig ReadConfig()
        {
            if (_esbConfig != null)
                return _esbConfig;
            if (!File.Exists(configfile))
            {
                var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                if (!FindFile(baseDir,configfile,ref configfile))
                {
                    return null;
                }
                
            }

            _esbConfig = SerializerHelper.DeSerializerFile<ESBConfig>(configfile, true);
            if (_esbConfig.ESBServer.IndexOf('.') == -1
                && _esbConfig.ESBServer.IndexOf(':') == -1)
            {
                var ipaddress = System.Net.Dns.GetHostAddresses(_esbConfig.ESBServer);
                if (ipaddress == null)
                {
                    throw new Exception("配置服务地址无效。");
                }

                _esbConfig.ESBServer = ipaddress.FirstOrDefault(p => p.AddressFamily != AddressFamily.InterNetworkV6).ToString();
            }

            return _esbConfig;
        }

        public static void WriteConfig(string esbServer, int esbPort, bool autoStrat = false)
        {
            ESBConfig config = new ESBConfig();
            config.ESBServer = esbServer;
            config.ESBPort = esbPort;
            config.AutoStart = autoStrat;
            SerializerHelper.SerializerToXML(config, configfile);
        }
    }
}
