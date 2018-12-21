using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SimpleFTPServer
{
    /// <summary>
    /// Класс, реализующий поведение сервера.
    /// </summary>
    class Server
    {
        private const int port = 8238;

        /// <summary>
        /// Метод запускает сервер.
        /// </summary>
        public static async Task Start()
        {
            var listener = new TcpListener(IPAddress.Any, port);

            listener.Start();

            while (true)
            {
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

            string receiveData = "";

            if (command[0] == '1')
            {
                int index = command.IndexOf(" ");
                receiveData = await FileCommander.DoListCommand(command.Substring(index));
            }
            else if (command[0] == '2')
            {
                int index = command.IndexOf(" ");
                receiveData = await FileCommander.DoGetCommand(command.Substring(index));
            }
            else
            {
                receiveData = "Команда не найдена";
            }

            var writer = new StreamWriter(stream);
            await writer.WriteAsync(receiveData);
            await writer.FlushAsync();
            socket.Close();
        }
    }
}
