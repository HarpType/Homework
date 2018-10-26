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

            string result1 = checker.GetCheckSum(@"D:\Temp");

            string testString = "";

            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(@"D:\Temp\hello.txt"))
                {
                    byte[] hashBytes = md5.ComputeHash(stream);
                    testString += System.Text.Encoding.Default.GetString(hashBytes);
                }

                DirectoryInfo dir = new DirectoryInfo(@"D:\Temp");

                byte[] inputBytes = System.Text.Encoding.Default.GetBytes(dir.FullName + testString);
                byte[] hashByte = md5.ComputeHash(inputBytes);
            }

            Assert.AreEqual(testString, result1);
        }
    }
}
