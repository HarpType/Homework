using System;
using MyNUnit.Attributes;

namespace ClassLibrary3
{
    public class Class1
    {
        public static int t = 0;

        [BeforeClass]
        static public void BeforeClassMethod()
        {
            t = 10;
        }

        [Test(Expected = typeof(Exception))]
        public void TestMethod()
        {
            if (t == 10)
                throw new Exception();
        }

        [AfterClass]
        static public void AfterClassMethod()
        {
            t = 0;
        }
    }
}
