using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Control1;
using System.Security.Cryptography;
using System.IO;

namespace CheckTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            CheckSum checker = new CheckSum();

            string testDir = new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;

            string result1 = checker.GetCheckSum($@"{testDir}\TestDir");

            string testString = "";

            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead($@"{testDir}\TestDir\hello.txt"))
                {
                    byte[] hashBytes = md5.ComputeHash(stream);
                    testString += System.Text.Encoding.Default.GetString(hashBytes);
                }

                DirectoryInfo dir = new DirectoryInfo($@"{testDir}\TestDir");

                byte[] inputBytes = System.Text.Encoding.Default.GetBytes(dir.FullName + testString);
                byte[] hashByte = md5.ComputeHash(inputBytes);
                testString = System.Text.Encoding.Default.GetString(hashByte);
            }

            Assert.AreEqual(testString, result1);
        }

        [TestMethod]
        public void TestMethod2()
        {
            CheckSum checker = new CheckSum();

            string testDir = new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;

            string result1 = checker.GetCheckSum($@"{testDir}\TestDir");

            SafeCheckSum safeChecker = new SafeCheckSum();
            string result2 = safeChecker.GetCheckSum($@"{testDir}\TestDir");

            Assert.AreEqual(result2, result1);
        }
    }
}
