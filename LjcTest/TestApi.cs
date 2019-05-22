using System;
using System.Collections.Generic;
using System.Text;

namespace LjcTest
{
    public class GetUserRequest
    {
        [LJC.NetCoreFrameWork.Attr.PropertyDescription("name")]
        public string Name
        {
            get;
            set;
        }

        public int Sex
        {
            get;
            set;
        }
    }

    public class GetUserResponse
    {
        public string Name
        {
            get;
            set;
        }

        public int Sex
        {
            get;
            set;
        }

        public string Addr
        {
            get;
            set;
        }
    }

    public class TestApi
    {
        [LJC.NetCoreFrameWork.WebApi.APIMethod()]
        public GetUserResponse GetUser(GetUserRequest request)
        {
            return new GetUserResponse
            {

            };
        }
    }
}
