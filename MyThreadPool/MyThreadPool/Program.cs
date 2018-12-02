using System;
using System.Threading;

namespace MyThreadPool
{
    class Program
    {
        static void Main(string[] args)
        {
            var threadPool = new MyThreadPool(1);

            string SayHello() => "Hello";
            var helloTask = threadPool.AddTask(SayHello);

            string SayWorld(string str) => str + " World!";
            var helloworldTask = helloTask.ContinueWith(SayWorld);

            if (!helloTask.IsCompleted)
            {
                Thread.Sleep(500);
            }

            Console.WriteLine(helloTask.Result);
            Console.ReadKey();
        }
    }
}
