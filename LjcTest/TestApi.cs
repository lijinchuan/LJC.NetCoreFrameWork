using LJC.NetCoreFrameWork.Data.EntityDataBase;
using LJC.NetCoreFrameWork.EntityBuf;
using LJC.NetCoreFrameWork.WebApi;
using System;
using System.Collections.Generic;
using System.Text;

namespace LjcTest
{

    public class AddPersonRequest
    {
        public List<Person> PersonList
        {
            get;
            set;
        }
    }

    public class AddPersonResponse
    {
        public bool Succ
        {
            get;
            set;
        }
    }

    public class GetPersonByIdRequest
    {
        public int Id
        {
            get;
            set;
        }
    }

    public class GetPersonByIdResponse
    {
        public Person Person
        {
            get;
            set;
        }
    }

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

        public int Age
        {
            get;
            set;
        }
    }

    public class TestApi
    {
        static TestApi()
        {
            try
            {
                BigEntityTableEngine.LocalEngine.CreateTable("Person", "ID", typeof(Person),
                    new IndexInfo[]
                    {
                    new IndexInfo
                    {
                        IndexName="Name",
                        Indexs=new IndexItem[]
                        {
                            new IndexItem
                            {
                               Field="Name",
                               Direction=1,
                               FieldType=EntityType.STRING
                            }
                        }
                    }
                    });

                Console.WriteLine("create table success");
            }
            catch (Exception ex)
            {
                Console.WriteLine("create table error:" + ex.ToString());
            }
        }

        [LJC.NetCoreFrameWork.WebApi.APIMethod()]
        public GetUserResponse GetUser(GetUserRequest request)
        {
            return new GetUserResponse
            {

            };
        }

        [APIMethod]
        public AddPersonResponse AddPerson(AddPersonRequest request)
        {
            foreach(var item in request.PersonList)
            {
                BigEntityTableEngine.LocalEngine.Insert<Person>("Person", item);
            }

            return new AddPersonResponse
            {
                Succ = true
            };
        }

        [APIMethod]
        public GetPersonByIdResponse GetPersonById(GetPersonByIdRequest request)
        {
            var p= BigEntityTableEngine.LocalEngine.Find<Person>("Person", request.Id);

            return new GetPersonByIdResponse
            {
                Person=p
            };
        }
    }
}
