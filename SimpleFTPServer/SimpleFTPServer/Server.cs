using System;
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
                receiveData = "Команда не найдена";
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
            string dataString;

            var dir = new DirectoryInfo(dirPath);

            if (!dir.Exists)
            {
                return "-1";
            }

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

            dataString = dirFilesCount.ToString() + " " + dirString + fileString;

            return dataString;
        }

        /// <summary>
        /// Формирует ответ на запрос Get.
        /// </summary>
        /// <param name="filePath">Путь к файлу.</param>
        /// <returns>Ответ в указанном формате.</returns>
        private string DoGetCommand(string filePath)
        {
            string dataString;

            long fileLength;
            byte[] content;

            try
            {
                using (var file = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    fileLength = file.Length;
                }

                content = File.ReadAllBytes(filePath);

                dataString = fileLength.ToString() + " " + Encoding.UTF8.GetString(content);
            }
            catch (Exception)
            {
                dataString = "-1";
            }

            return dataString;
        }
    }
}
