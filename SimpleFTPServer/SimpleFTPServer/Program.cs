﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFTPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new Server();

            server.Start();
        }
    }
}
