﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleFTPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //var task = new Task(Server.Start);
            //task.Wait();

            var server = new Server();
            server.Start();
        }
    }
}
