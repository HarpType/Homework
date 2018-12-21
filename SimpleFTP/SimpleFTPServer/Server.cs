using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleFTPServer
{
    /// <summary>
    /// Класс, реализующий поведение сервера.
    /// </summary>
    public class Server
    {
        private const int port = 8238;

        /// <summary>
        /// Метод запускает сервер.
        /// </summary>
        public static async Task Start(CancellationToken ct)
        {
            var listener = new TcpListener(IPAddress.Any, port);

            listener.Start();

            while (true)
            {
                if (ct.IsCancellationRequested)
                    return;

                var socket = await listener.AcceptSocketAsync();

                var newTask = new Task(requestSocket => ProcessNewRequest((Socket)requestSocket), socket);

                newTask.Start();
            }
        }

        /// <summary>
        /// Обработчик запроса на сервер.
        /// </summary>
        /// <param name="socket">socket, созданный для общения с клиентом.</param>
        static async private void ProcessNewRequest(Socket socket)
        {
            var stream = new NetworkStream(socket);
            var reader = new StreamReader(stream);
            var command = await reader.ReadLineAsync();

            string receiveData = "Command not found";

            if (command.Length > 3)
            {
                if (command[0] == '1')
                {
                    receiveData = await FileCommander.DoListCommand(command.Substring(1));
                }
                else if (command[0] == '2')
                {
                    receiveData = await FileCommander.DoGetCommand(command.Substring(1));
                }
            }

            var writer = new StreamWriter(stream);
            await writer.WriteAsync(receiveData);
            await writer.FlushAsync();
            socket.Close();
        }
    }
}
