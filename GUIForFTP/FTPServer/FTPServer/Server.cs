using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace FTPServer
{
    /// <summary>
    /// Класс, реализующий поведение сервера.
    /// </summary>
    public class Server
    {
        private const int port = 8238;

        ConcurrentBag<Socket> sockets = new ConcurrentBag<Socket>();

        TcpListener listener = new TcpListener(IPAddress.Any, port);

        CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct;

        AutoResetEvent mainCloseEvent = new AutoResetEvent(false);
        AutoResetEvent listenerCloseEvent = new AutoResetEvent(false);
        AutoResetEvent socketEvent = new AutoResetEvent(false);
        AutoResetEvent startEvent = new AutoResetEvent(false);

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

            startEvent.WaitOne();
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
                    mainCloseEvent.Set();
                    return;
                }

                foreach (var socket in sockets)
                {
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
        private async void ProcessNewRequest(Socket socket)
        {
            var stream = new NetworkStream(socket);
            var reader = new StreamReader(stream);
            var command = await reader.ReadLineAsync();

            var writer = new StreamWriter(stream) { AutoFlush = true };
            if (command.Length > 2)
            {
                if (command[0] == '1')
                {
                    DoListCommand(command.Substring(1), writer);
                }
                else if (command[0] == '2')
                {
                    DoGetCommand(command.Substring(1), writer);
                }
            }
            else
            {
                writer.WriteLine("Command not found");
            }

            socket.Close();
        }

        private void DoListCommand(string dirPath, StreamWriter writer)
        {
            DirectoryInfo dir = null;

            try
            {
                dir = new DirectoryInfo(dirPath);
            }
            catch (ArgumentException)
            {
                writer.WriteLine("-1");
                return;
            }

            if (!dir.Exists)
            {
                writer.WriteLine("-1");
                return;
            }

            int dirFilesCount = 0;

            List<string> dirFileStrings = new List<string>();
            foreach (var item in dir.GetDirectories())
            {
                dirFilesCount++;
                dirFileStrings.Add(item.Name + " true");
            }

            foreach (var item in dir.GetFiles())
            {
                dirFilesCount++;
                dirFileStrings.Add(item.Name + " false");
            }


            writer.WriteLine(dirFilesCount);
            foreach (var item in dirFileStrings)
            {
                writer.WriteLine(item);
            }
        }

        private void DoGetCommand(string filePath, StreamWriter writer)
        {
            try
            {
                using (var file = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    writer.WriteLine(file.Length);
                }

                var lines = File.ReadLines(filePath);
                foreach (var line in lines)
                {
                    writer.WriteLine(line);
                }
            }
            catch (Exception)
            {
                writer.WriteLine("-1");
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