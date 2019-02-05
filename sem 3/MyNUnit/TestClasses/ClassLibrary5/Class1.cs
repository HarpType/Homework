using System;
using MyNUnit.Attributes;

namespace ClassLibrary5
{
    public class Class1
    {
        [Test(Ignore = "Reason")]
        public void IgnoredMethod()
        {

        }

        [Test]
        public void FalseTest()
        {
            throw new Exception();
        }
    }
}
