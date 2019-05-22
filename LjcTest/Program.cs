using System;

namespace LjcTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                LJC.NetCoreFrameWork.WebApi.APIFactory.Init("LjcTest");
                Console.WriteLine("站点开启");
                Console.Read();
            }
            catch(Exception ex)
            {
                Console.Write(ex.ToString());
                return;
            }

            Person p = new Person
            {
                ID = 1058985,
                Name = "钱得斯",
                Sex = 2,
                Duties = new System.Collections.Generic.List<string> { "teacher", "worker", "father" }
            };

            var buf = LJC.NetCoreFrameWork.EntityBuf.EntityBufCore.Serialize(p);
            var newp = LJC.NetCoreFrameWork.EntityBuf.EntityBufCore.DeSerialize<Person>(buf);

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(newp));

            PersonService sev = new PersonService();
            sev.Error += Sev_Error;
            sev.LoginFail += Sev_LoginFail;
            sev.LoginSuccess += Sev_LoginSuccess;
            
            sev.StartService();
            Console.WriteLine("service running");
            Console.ReadLine();

            sev.UnRegisterService();
            sev.Dispose();
            Console.WriteLine("service stop");
            Console.ReadLine();
        }

        private static void Sev_LoginSuccess()
        {
            Console.WriteLine("login success");
        }

        private static void Sev_LoginFail()
        {
            Console.WriteLine("login fail");
        }

        private static void Sev_Error(Exception obj)
        {
            Console.WriteLine(obj.Message);
        }
    }
}
