using System;
using MyNUnit.Attributes;

namespace ClassLibrary4
{
    public class Class1
    {
        public int t = 0;

        [Before]
        public void BeforeMethod()
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

        [After]
        public void AfterMethod()
        {
            t = 0;
        }
    }
}
