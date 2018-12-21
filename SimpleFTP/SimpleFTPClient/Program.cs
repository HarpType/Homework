using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFTPClient
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Команда не задана либо задана неверно");
                return;
            }

            var data = Client.SendRequest(args[0], args[1] + " " + args[2]).GetAwaiter().GetResult();

            Console.WriteLine(data);
        }
    }
}
