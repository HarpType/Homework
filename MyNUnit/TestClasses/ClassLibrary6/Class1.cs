using System;
using MyNUnit.Attributes;
using System.Threading;

namespace ClassLibrary6
{
    public class Class1
    {
        [Test(Expected = typeof(DivideByZeroException))]
        public void TrueTest()
        {
            throw new DivideByZeroException("Zero Exception");
        }

        [Test(Expected = typeof(Exception))]
        public void FalseTest()
        {
           
        }

        [Test(Ignore = "Reason")]
        public void IgnoredMethod()
        {
            Thread.Sleep(123456789);
        }
    }

    public class Class2
    {
        private bool flag = false;

        [Test]
        public void ChangeFlag()
        {
            flag = true;
        }

        [AfterClass]
        public void CheckFlag()
        {
            if (!flag)
                throw new Exception();
        }

        public void ExtraMethod()
        {

        }
    }
}
