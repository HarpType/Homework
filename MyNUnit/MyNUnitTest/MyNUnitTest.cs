using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyNUnit;

namespace MyNUnitTest
{
    [TestClass]
    public class MyNUnitTest
    {
        private readonly string RootPath = 
            new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;

        [TestMethod]
        public void ArgumentNullTest()
        {
            Assert.ThrowsException<ArgumentNullException>(() => TestLauncher.Launch(null));
        }

        [TestMethod]
        public void DirectoryNotFoundTest()
        {
            Assert.ThrowsException<DirectoryNotFoundException>(() => TestLauncher.Launch("wrongpath"));
        }

        [TestMethod]
        public void OneMethodTest()
        {
            string path = RootPath + @"/TestClasses/ClassLibrary1/bin/Debug/";

            List<TestInfo> infoList = TestLauncher.Launch(path);

            Assert.IsTrue(infoList.Count == 1);

            Assert.IsTrue(infoList[0].Successfull);
        }

        [TestMethod]
        public void BeforeAfterClassTest()
        {
            string path = RootPath + @"/TestClasses/ClassLibrary2/bin/Debug/";

            List<TestInfo> infoList = TestLauncher.Launch(path);

            Assert.IsTrue(infoList.Count == 3);

            foreach (var test in infoList)
            {
                Assert.IsTrue(test.Successfull);
            }
        }

        [TestMethod]
        public void BeforeAfterTest()
        {
            string path = RootPath + @"/TestClasses/ClassLibrary3/bin/Debug/";

            List<TestInfo> infoList = TestLauncher.Launch(path);

            Assert.IsTrue(infoList.Count == 3);

            foreach (var test in infoList)
            {
                Assert.IsTrue(test.Successfull);
            }
        }

        [TestMethod]
        public void ExpectedTest()
        {
            string path = RootPath + @"/TestClasses/ClassLibrary4/bin/Debug/";

            List<TestInfo> infoList = TestLauncher.Launch(path);

            Assert.IsTrue(infoList.Count == 3);

            foreach (var test in infoList)
            {
                Assert.IsTrue(test.Successfull);
            }
        }

        [TestMethod]
        public void IgnoreTest()
        {
            string path = RootPath + @"/TestClasses/ClassLibrary5/bin/Debug/";

            List<TestInfo> infoList = TestLauncher.Launch(path);

            Assert.IsTrue(infoList.Count == 2);

            foreach (var test in infoList)
            {
                Assert.IsFalse(test.Successfull);
            }
        }

        [TestMethod]
        public void TwoClassesTest()
        {
            string path = RootPath + @"/TestClasses/ClassLibrary6/bin/Debug/";

            List<TestInfo> infoList = TestLauncher.Launch(path);

            Assert.IsTrue(infoList.Count == 5);

            Assert.AreEqual(typeof(DivideByZeroException), infoList[0].Result);
            Assert.IsFalse(infoList[1].Successfull);
            Assert.IsTrue(infoList[2].isIgnored);
            Assert.IsTrue(infoList[3].Successfull);
            Assert.IsTrue(infoList[4].Successfull);
        }
    }
}
