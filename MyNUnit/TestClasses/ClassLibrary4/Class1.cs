using System;
using MyNUnit.Attributes;

namespace ClassLibrary3
{
    public class Class1
    {
        int t = 0;

        [BeforeClass]
        public void BeforeClassMethod()
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
        public void AfterClassMethod()
        {
            t = 0;
        }
    }
}
