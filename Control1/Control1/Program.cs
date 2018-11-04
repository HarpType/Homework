using System;
using System.IO;

namespace Control1
{
    class Program
    {
        static void Main(string[] args)
        {
            CheckSum checker = new CheckSum();
            SafeCheckSum safeChecker = new SafeCheckSum();

            //string fullPath = Path.GetFullPath("");
            string testDir = new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;
            
            string result1 = checker.GetCheckSum($@"{testDir}\TestDir");

            string result2 = safeChecker.GetCheckSum($@"{testDir}\TestDir");

            Console.WriteLine(result1);
            Console.WriteLine(result2);
            Console.ReadKey();
        }
    }
}
