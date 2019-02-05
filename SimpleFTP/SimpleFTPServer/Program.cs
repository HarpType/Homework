using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleFTPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new Server();
            server.Start();

            Console.ReadKey();

            server.Shutdown();
        }
    }
}
