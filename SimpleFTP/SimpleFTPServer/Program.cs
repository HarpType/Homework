using System;
using System.Threading.Tasks;

namespace SimpleFTPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //var task = new Task(Server.Start);
            //task.Wait();

            Server.Start().GetAwaiter().GetResult();
        }
    }
}
