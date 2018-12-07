using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace SimpleFTPClient
{
    class Client
    {
        public void Start()
        {
            while (true)
            {
                string server;
                int port;

                Console.WriteLine("Введите сервер");
                server = Console.ReadLine();
                Console.WriteLine("Введите порт");
                port = Int32.Parse(Console.ReadLine());

                string command;

                Console.WriteLine("Введите команду");
                command = Console.ReadLine();

                try
                {
                    using (var client = new TcpClient(server, port))
                    {
                        var stream = client.GetStream();
                        var writer = new StreamWriter(stream);

                        writer.WriteLine(command);
                        writer.Flush();

                        var reader = new StreamReader(stream);
                        var data = reader.ReadToEnd();
                        Console.WriteLine(data);
                    }
                }
                catch (SocketException)
                {
                    Console.WriteLine("Невозможно подключиться к серверу");
                }
            }
        }
    }
}
