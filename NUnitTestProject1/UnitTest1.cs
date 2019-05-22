using NUnit.Framework;

namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            NUnitTestProject1.Person p = new NUnitTestProject1.Person
            {
                ID=1058985,
                Name="«Æµ√Àπ",
                Sex=2,
                Duties=new System.Collections.Generic.List<string> { "teacher","worker","father"}
            };

            var buf = LJC.NetCoreFrameWork.EntityBuf.EntityBufCore.Serialize(p);
            var newp = LJC.NetCoreFrameWork.EntityBuf.EntityBufCore.DeSerialize<NUnitTestProject1.Person>(buf);

            
            Assert.Pass();
        }
    }
}