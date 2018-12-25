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
        private string server = "127.0.0.1";
        private int port = 8238;

        public async Task<List<FileInfo>> DoListCommand(string dirPath)
        {
            string command = "1 " + dirPath;
            try
            {
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
            }
            catch
            {
                return new List<FileInfo>();
            }
        }
    }
}
