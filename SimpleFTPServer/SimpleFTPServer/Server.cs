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

                var newTask = new Task(requestSocket => ProcessNewRequest((Socket)requestSocket), socket);

                newTask.Start();
            }
        }

        /// <summary>
        /// Обработчик запроса на севрер.
        /// </summary>
        /// <param name="socket">socket, созданный для общения с клиентом.</param>
        async private void ProcessNewRequest(Socket socket)
        {
            var stream = new NetworkStream(socket);
            var reader = new StreamReader(stream);
            var command = await reader.ReadLineAsync();


            string receiveData = "";

            if (command[0] == '1')
            {
                int index = command.IndexOf(" ");
                receiveData = DoListCommand(command.Substring(index));
            }
            else if (command[0] == '2')
            {
                int index = command.IndexOf(" ");
                receiveData = DoGetCommand(command.Substring(index));
            }
            else
            {
            }

            var writer = new StreamWriter(stream);
            await writer.WriteAsync(receiveData);
            await writer.FlushAsync();
            socket.Close();
        }

        /// <summary>
        /// Формирует ответ на запрос List.
        /// </summary>
        /// <param name="path">Путь к директории.</param>
        /// <returns>Ответ в указанном формате.</returns>
        private string DoListCommand(string dirPath)
        {
            var dir = new DirectoryInfo(dirPath);

            int dirFilesCount = 0;

            string dirString = "";
            foreach (var item in dir.GetDirectories())
            {
                dirFilesCount++;
                dirString += item.Name + " true ";
            }

            string fileString = "";
            foreach (var item in dir.GetFiles())
            {
                dirFilesCount++;
                fileString += item.Name + " false ";
            }

            string dataString = dirFilesCount.ToString() + " " + dirString + fileString;

            return dataString;
        }

        /// <summary>
        /// Формирует ответ на запрос Get.
        /// </summary>
        /// <param name="filePath">Путь к файлу.</param>
        /// <returns>Ответ в указанном формате.</returns>
        private string DoGetCommand(string filePath)
        {
            long fileSize = 0;
            string content = "";

            using (StreamReader fs = new StreamReader(filePath))
            {
                while (true)
                {
                    string temp = fs.ReadLine();

                    if (temp == null) break;

                    content += temp;
                    fileSize += temp.Length;
                }
            }

            return fileSize.ToString() + " " + content;
        }
    }
}
