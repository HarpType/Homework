using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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

        private string defaultPath = @"\";

        /// <summary>
        /// Подключение к серверу.
        /// </summary>
        /// <param name="address">Адрес сервера.</param>
        /// <param name="port">Номер порта.</param>
        public async Task ConnectToServer(string address, int port)
        {
            List<FileInfo> dirInfo = await client.DoListCommand(defaultPath);

            Files.Clear();
            foreach (var item in dirInfo)
            {
                Files.Add(item);
            }
        }

        /// <summary>
        /// Запросить список файлов и папок у сервера по заданной директории.
        /// </summary>
        /// <param name="dirPath">Директория, для которой необходимо узнать список.</param>
        public async Task GetDirectory(string dirPath)
        {
            List<FileInfo> dirInfo = await client.DoListCommand(dirPath);

            Files.Clear();
            if (dirPath != defaultPath)
            {
                string firstPath = Path.GetDirectoryName(dirPath);
                string parentPath = Path.GetDirectoryName(firstPath);
                if (parentPath != @"\")
                    parentPath += @"\";
                Files.Add(new FileInfo(parentPath, FileItemType.Upper));
            }
            foreach (var item in dirInfo)
            {
                Files.Add(item);
            }
        }
    }
}
