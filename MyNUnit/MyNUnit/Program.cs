using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNUnit
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Путь не задан");
                return;
            }


            TestConsoleInfo.WriteTestInfo(TestLauncher.Launch(args[0]));

            Console.ReadKey();
            
        }
    }
}
