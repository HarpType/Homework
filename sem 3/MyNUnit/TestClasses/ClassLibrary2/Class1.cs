using System;
using MyNUnit.Attributes;
using System.Threading;

namespace ClassLibrary2
{
    public class Class1
    {
        public static int t = 0;

        [BeforeClass]
        static public void BeforeClassMethod()
        {
            t = 10;
        }

        [Test]
        public void TrueTest()
        {
            if (t != 10)
            {
                throw new Exception();
            }
        }

        [AfterClass]
        static public void AfterClassMethod()
        {
            t = 0;
        }
    }
}
