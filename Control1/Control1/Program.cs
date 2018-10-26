using System;

namespace Control1
{
    class Program
    {
        static void Main(string[] args)
        {
            CheckSum checker = new CheckSum();
            SafeCheckSum safeChecker = new SafeCheckSum();

            string result1 = checker.GetCheckSum(@"D:\TASM");

            string result2 = safeChecker.GetCheckSum(@"D:\TASM");

            Console.WriteLine(result1);
            Console.WriteLine(result2);
            Console.ReadKey();
        }
    }
}
