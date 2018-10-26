using System;

namespace Control1
{
    class Program
    {
        static void Main(string[] args)
        {
            CheckSum checker = new CheckSum();

            string result = checker.GetCheckSum(@"D:\Temp");

            Console.WriteLine(result);
            Console.ReadKey();
        }
    }
}
