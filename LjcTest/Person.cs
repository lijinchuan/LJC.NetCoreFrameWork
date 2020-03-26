using System;
using System.Collections.Generic;
using System.Text;

namespace LjcTest
{
    public class Person
    {
        public int ID
        {
            get;
            set;
        }

        /// <summary>
        /// 姓别 0-女 1-男
        /// </summary>
        public int Sex
        {
            get;
            set;
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 用户职务
        /// </summary>
        public List<string> Duties
        {
            get;
            set;
        }
    }
}
