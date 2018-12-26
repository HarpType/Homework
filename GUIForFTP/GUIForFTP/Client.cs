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
        /// <summary>
        /// Адрес сервера
        /// </summary>
        private string servAddress = "127.0.0.1";

        /// <summary>
        /// Порт сервера.
        /// </summary>
        private int port = 8238;

        /// <summary>
        /// Отправляет запрос List на сервер. Возвращает набор файлов и папок, хранящиеся 
        /// по указанному пути на сервере.
        /// </summary>
        /// <param name="dirPath">Директория, для которой необходимо вернуть файлы и папки.</param>
        /// <returns>Список файлов и папок.</returns>
        public async Task<List<FileInfo>> DoListCommand(string dirPath)
        {
            string command = "1 " + dirPath;

            var client = new TcpClient();
            try
            {
                await client.ConnectAsync(servAddress, port);

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
                    string fileDirName = dirPath + reader.ReadLine();
                    string isDirectory = reader.ReadLine();

                    FileItemType itemType = FileItemType.File;
                    if (isDirectory == "True")
                    {
                        fileDirName += @"\";
                        itemType = FileItemType.Directory;
                    }

                    var fileInfo = new FileInfo(fileDirName, itemType);

                    filesInfo.Add(fileInfo);
                }

                return filesInfo;

            }
            catch
            {
                client.Close();
                return new List<FileInfo>();
            }
            finally
            {
                client.Close();
            }
        }

        /// <summary>
        /// Скачивает файл с сервера. 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="downloadPath"></param>
        /// <returns>Полное название файла, если он скачан, null в противном случае.</returns>
        public async Task<string> DownloadFile(FileInfo file, string downloadPath)
        {
            string command = "2 " + file.FullName;

            var client = new TcpClient();
            try
            {
                await client.ConnectAsync(servAddress, port);

                var stream = client.GetStream();
                var writer = new StreamWriter(stream) { AutoFlush = true };

                await writer.WriteLineAsync(command);

                var reader = new StreamReader(stream);
                string size = await reader.ReadLineAsync();

                if (size == "-1")
                {
                    return null;
                }

                string filePath = FileCreator.Create(file.Name, downloadPath);

                if (filePath == null)
                {
                    return null;
                }

                using (var fileWriter = new StreamWriter(filePath))
                {
                    string data;
                    while ((data = await reader.ReadLineAsync()) != null)
                    {
                        await fileWriter.WriteLineAsync(data);
                    }
                }

                return filePath;
            }
            catch
            {
                client.Close();
                return null;
            }
            finally
            {
                client.Close();
            }
        }
    }
}
