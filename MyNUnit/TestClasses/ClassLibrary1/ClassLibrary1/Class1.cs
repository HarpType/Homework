using System;
using MyNUnit.Attributes;

namespace ClassLibrary1
{
    public class Class1
    {
        public int t = 0;

        [BeforeClass]
        public void changeT()
        {
            t = 10;
        }

        [Test]
        public void TrueTest0()
        {
            if (t != 10)
            {
                throw new Exception("TTTT");
            }
        }

        [Test(Expected = typeof(DivideByZeroException))]
        public void TrueTest1()
        {
            throw new DivideByZeroException("OK");
        }
    }
}

