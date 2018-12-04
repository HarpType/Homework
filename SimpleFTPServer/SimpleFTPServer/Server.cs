using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace SimpleFTPServer
{
    /// <summary>
    /// Класс, реализующий поведение сервера.
    /// </summary>
    class Server
    {
        private const int port = 8238;

        private TcpListener listener = new TcpListener(IPAddress.Any, port);

        /// <summary>
        /// Метод запускает сервер.
        /// </summary>
        public void Start()
        {
            listener.Start();

            Console.WriteLine("Listening...");

            while (true)
            {
                var socket = listener.AcceptSocket();

                var newTask = new Task(
                    async () =>
                    {
                        var stream = new NetworkStream(socket);
                        var reader = new StreamReader(stream);
                        var data = await reader.ReadLineAsync();

                        Console.WriteLine($"Received: {data}");
                        Console.WriteLine($"Sending \"Hi!\"");

                        var writer = new StreamWriter(stream);
                        await writer.WriteAsync("Hi!");
                        await writer.FlushAsync();
                        socket.Close();
                    }
                    );

                newTask.Start();
            }
        }
    }
}
