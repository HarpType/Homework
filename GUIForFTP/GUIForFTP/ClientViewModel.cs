using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUIForFTP
{
    class ClientViewModel
    {
        /// <summary>
        /// Коллекция файлов
        /// </summary>
        public ObservableCollection<FileInfo> Files { get; set; } = new ObservableCollection<FileInfo>();

        /// <summary>
        /// Подключение к серверу.
        /// </summary>
        /// <param name="address">Адрес сервера.</param>
        /// <param name="port">Номер порта.</param>
        /// <returns></returns>
        public async Task ConnectToServer(string address, int port)
        {
            string defaultPath = @".";

            string command = "1 " + defaultPath;

            string dirInfo = await Client.SendRequest(address, port, command);

            ParseInfo(dirInfo, defaultPath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dirInfo"></param>
        /// <param name="path"></param>
        public void ParseInfo(string path)
        {
            string[] info = dirInfo.Split(' ');

            int.TryParse(info[0], out int fileCount);

            for (int i = 1; i <= fileCount; ++i)
            {
                string name = info[2 * i - 1];
                if (info[2 * i] == "true")
                {
                    Files.Add(new FileInfo(path + name, name, true));
                }
                else
                {
                    Files.Add(new FileInfo(path + name, name, false));
                }
                
            }
        }
    }
}
