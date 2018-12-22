using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SimpleFTPServer
{
    /// <summary>
    /// Класс, реализующий поведение сервера.
    /// </summary>
    public class Server
    {
        private const int port = 8238;

        Queue<Socket> socketQueue = new Queue<Socket>();

        TcpListener listener = new TcpListener(IPAddress.Any, port);

        CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct;

        AutoResetEvent closeEvent = new AutoResetEvent(false);
        AutoResetEvent socketEvent = new AutoResetEvent(false);

        public Server()
        {
            ct = cts.Token;
        }

        /// <summary>
        /// Передаёт основную работу сервера таску.
        /// </summary>
        public void Start()
        {
            Task.Run(StartProcess);
         
        }

        /// <summary>
        /// Запускает сервер.
        /// </summary>
        /// <returns></returns>
        private async Task StartProcess()
        {
            StartListening();

            while (true)
            {
                socketEvent.WaitOne();

                if (ct.IsCancellationRequested)
                {
                    closeEvent.Set();
                    return;
                }

                foreach (var socket in socketQueue)
                {
                    var newTask = new Task(requestSocket => ProcessNewRequest((Socket)requestSocket), socket);
                    newTask.Start();
                }
            }
        }

        /// <summary>
        /// Запускает работу listener в отедльном потоке.
        /// </summary>
        private async void StartListening()
        {
            listener.Start();

            while (true)
            {
                var socket = await listener.AcceptSocketAsync();

                socketQueue.Enqueue(socket);

                socketEvent.Set();
            }
        }

        /// <summary>
        /// Обработчик запроса на сервер.
        /// </summary>
        /// <param name="socket">socket, созданный для общения с клиентом.</param>
        private async void ProcessNewRequest(Socket socket)
        {
            var stream = new NetworkStream(socket);
            var reader = new StreamReader(stream);
            var command = await reader.ReadLineAsync();

            string receiveData = "Command not found";

            if (command.Length > 2)
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

        /// <summary>
        /// Завершает исполнение сервера.
        /// </summary>
        public void Shutdown()
        {
            cts.Cancel();
            cts.Dispose();

            socketEvent.Set();

            closeEvent.WaitOne();
        }
    }
}
