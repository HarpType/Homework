﻿using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace SimpleFTPServer
{
    /// <summary>
    /// Класс, реализующий поведение сервера.
    /// </summary>
    public class Server
    {
        private const int port = 8238;

        /// <summary>
        /// Набор сокетов для подключенных клиентов.
        /// </summary>
        private ConcurrentBag<Socket> sockets = new ConcurrentBag<Socket>();

        private TcpListener listener = new TcpListener(IPAddress.Any, port);

        private CancellationTokenSource cts = new CancellationTokenSource();
        private CancellationToken ct;

        private AutoResetEvent mainCloseEvent = new AutoResetEvent(false);
        private AutoResetEvent listenerCloseEvent = new AutoResetEvent(false);
        private AutoResetEvent socketEvent = new AutoResetEvent(false);
        private AutoResetEvent startEvent = new AutoResetEvent(false);

        public Server()
        {
            ct = cts.Token;
        }

        /// <summary>
        /// Передаёт основную работу сервера таску.
        /// </summary>
        public void Start()
        {
            Task.Run(() => StartProcess());

            startEvent.WaitOne();
        }

        /// <summary>
        /// Запускает сервер.
        /// </summary>
        /// <returns></returns>
        private void StartProcess()
        {
            Task.Run(() => StartListening());

            while (true)
            {
                socketEvent.WaitOne();

                if (ct.IsCancellationRequested)
                {
                    mainCloseEvent.Set();
                    return;
                }

                for (int i = 0; i < sockets.Count; i++)
                {
                    sockets.TryTake(out Socket socket);
                    var newTask = new Task(requestSocket => ProcessNewRequest((Socket)requestSocket), socket);
                    newTask.Start();
                }
            }
        }

        /// <summary>
        /// Запускает работу listener в отдельном потоке.
        /// </summary>
        private async void StartListening()
        {
            listener.Start();

            startEvent.Set();

            while (true)
            {
                var socket = await listener.AcceptSocketAsync();

                if (ct.IsCancellationRequested)
                {
                    listenerCloseEvent.Set();
                    return;
                }

                sockets.Add(socket);

                socketEvent.Set();
            }
        }

        /// <summary>
        /// Обработчик запроса на сервер.
        /// </summary>
        /// <param name="socket">socket, созданный для общения с клиентом.</param>
        private void ProcessNewRequest(Socket socket)
        {
            try
            {
                var stream = new NetworkStream(socket);
                var reader = new StreamReader(stream);
                var command = reader.ReadLine();

                string receiveData = "Command not found";

                if (command.Length > 2)
                {
                    if (command[0] == '1')
                    {
                        receiveData = FileCommander.DoListCommand(command.Substring(1));
                    }
                    else if (command[0] == '2')
                    {
                        receiveData = FileCommander.DoGetCommand(command.Substring(1));
                    }
                }

                var writer = new StreamWriter(stream);
                writer.Write(receiveData);
                writer.Flush();
            }
            finally
            {
                socket.Close();
            }
        }

        /// <summary>
        /// Завершает исполнение сервера.
        /// </summary>
        public void Shutdown()
        {
            cts.Cancel();
            cts.Dispose();

            socketEvent.Set();

            listener.Stop();

            mainCloseEvent.WaitOne();
        }
    }
}
