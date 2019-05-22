using System;
using System.Collections.Generic;
using System.Text;

namespace LjcTest
{
    public class PersonService : LJC.NetCoreFrameWork.SOA.ESBService
    {
        public PersonService() : base(9999)
        {

        }

        public override object DoResponse(int funcId, byte[] Param, string clientid)
        {
            switch (funcId)
            {
                case 1:
                    {
                        return new Person
                        {
                            Duties=new List<string> { "worker","teacher","教练"},
                            ID=Guid.NewGuid().GetHashCode(),
                            Name=Guid.NewGuid().ToString("N"),
                            Sex=Guid.NewGuid().GetHashCode()%2
                        };
                    }
                default:
                    {
                        throw new NotSupportedException();
                    }
            }
        }
    }
}
