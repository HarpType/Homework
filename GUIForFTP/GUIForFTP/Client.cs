using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GUIForFTP
{
    /// <summary>
    /// Класс, реализующий работу клиента.
    /// </summary>
    public class Client
    {

        //public static async Task<string> SendRequest(string server, int port, string command)
        //{
        //    string data = "";
        //    try
        //    {
        //        using (var client = new TcpClient(server, port))
        //        {
        //            var stream = client.GetStream();
        //            var writer = new StreamWriter(stream);

        //            await writer.WriteLineAsync(command);
        //            await writer.FlushAsync();

        //            var reader = new StreamReader(stream);
        //            data = await reader.ReadToEndAsync();
        //        }
        //    }
        //    catch (SocketException)
        //    {
        //        throw new Exception("Ошибка подключения к серверу");
        //    }

        //    return data;
        //}

        private string server = "127.0.0.1";
        private int port = 8238;

        public async Task<List<FileInfo>> DoListCommand(string dirPath)
        {
            string command = "1 " + dirPath;

            using (var client = new TcpClient(server, port))
            {
                var stream = client.GetStream();
                var writer = new StreamWriter(stream) { AutoFlush = true };

                await writer.WriteLineAsync(command);

                var reader = new StreamReader(stream);
                string size = await reader.ReadLineAsync();

                if (size == "-1")
                {
                    // TODO: EXIT
                }

                int dirFileCount = 0;
                try
                {
                    dirFileCount = int.Parse(size);
                }
                catch (FormatException)
                {
                    //TODO: EXIT
                }

                List<FileInfo> filesInfo = new List<FileInfo>();
                for (int i = 0; i < dirFileCount; i++)
                {
                    string fileItem = reader.ReadLine();

                    string[] items = fileItem.Split();
                    FileItemType itemType = FileItemType.Directory;
                    if (items[1] == "false")
                    {
                        itemType = FileItemType.File;
                    }

                    var fileInfo = new FileInfo(items[0], itemType);

                    filesInfo.Add(fileInfo);
                }

                return filesInfo;
            }
        }
    }
}
