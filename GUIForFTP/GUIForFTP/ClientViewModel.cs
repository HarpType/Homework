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
        Client client = new Client();

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
            string defaultPath = @"D:\";

            string command = "1 " + defaultPath;

            List<FileInfo> dirInfo = await client.DoListCommand(defaultPath);

            Files.Clear();
            Files.Add(new FileInfo("...", FileItemType.Upper));
            foreach (var item in dirInfo)
            {
                Files.Add(item);
            }
        }
    }
}
