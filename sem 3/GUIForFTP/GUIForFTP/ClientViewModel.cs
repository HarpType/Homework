﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace GUIForFTP
{
    class ClientViewModel
    {
        Client client = new Client();

        /// <summary>
        /// Коллекция файлов
        /// </summary>
        public ObservableCollection<FileInfo> Files { get; set; } = new ObservableCollection<FileInfo>();
        public ObservableCollection<DownloadInfo> FilesToDownload { get; set; } = new ObservableCollection<DownloadInfo>();

        private string defaultPath = @"\";

        /// <summary>
        /// Подключение к серверу.
        /// </summary>
        /// <param name="address">Адрес сервера.</param>
        /// <param name="port">Номер порта.</param>
        public async Task ConnectToServer(string address, int port)
        {
            client.Address = address;
            client.Port = port;

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

        /// <summary>
        /// Скачивает файл.
        /// </summary>
        /// <param name="file">Файл, который необходимо скачать.</param>
        /// <param name="downloadPath">Путь для скачивания файла.</param>
        public async Task DownloadFile(FileInfo file, string downloadPath)
        {
            var downFile = new DownloadInfo(file.Name) { Status = DownloadType.InProcess };

            FilesToDownload.Add(downFile);

            string localFilePath = await client.DownloadFile(file, downloadPath);

            if (localFilePath != null)
            {
                string newFileName = Path.GetFileName(localFilePath);

                var newItem = new DownloadInfo(newFileName) { Status = DownloadType.Downloaded };

                FilesToDownload.Remove(downFile);
                FilesToDownload.Add(newItem);
            }
            else
            {
                var newItem = new DownloadInfo(downFile.FileName) { Status = DownloadType.DownloadError };

                FilesToDownload.Remove(downFile);
                FilesToDownload.Add(newItem);
            }
        }

        /// <summary>
        /// Скачивает файл в производном потоке.
        /// </summary>
        /// <param name="file">Файл на скачивание.</param>
        /// <param name="downloadPath">Директория, в которую необходимо скачать файл.</param>
        /// <param name="dispatcher">Позволяет изменять состояние wpf-форм в производном потоке.</param>
        public async Task DownloadFile(FileInfo file, string downloadPath, Dispatcher dispatcher)
        {
            var downFile = new DownloadInfo(file.Name) { Status = DownloadType.InProcess };

            dispatcher.Invoke(
                () =>
                {
                    FilesToDownload.Add(downFile);
                });

            string localFilePath = await client.DownloadFile(file, downloadPath);

            if (localFilePath != null)
            {
                string newFileName = Path.GetFileName(localFilePath);

                var newItem = new DownloadInfo(newFileName) { Status = DownloadType.Downloaded };

                dispatcher.Invoke(
                   () =>
                   {
                       FilesToDownload.Remove(downFile);
                       FilesToDownload.Add(newItem);
                   });
            }
            else
            {
                var newItem = new DownloadInfo(downFile.FileName) { Status = DownloadType.DownloadError };

                dispatcher.Invoke(
                   () =>
                   {
                       FilesToDownload.Remove(downFile);
                       FilesToDownload.Add(newItem);
                   });
            }
        }

        /// <summary>
        /// Скачивает все файлы в текущей директории.
        /// </summary>
        /// <param name="downloadPath">Директория, в которую необходимо скачивать файлы.</param>
        public void DownloadAll(string downloadPath, Dispatcher dispatcher)
        {
            var tasks = new List<Task>();

            foreach (var file in Files)
            {
                if (file.ItemType == FileItemType.File)
                {
                    Task.Run(async () => await DownloadFile(file, downloadPath, dispatcher));
                }
            }
        }

        /// <summary>
        /// Статус скачиваемого файла.
        /// </summary>
        public enum DownloadType
        {
            InProcess,
            Downloaded,
            DownloadError
        }

        /// <summary>
        /// Класс предоставляет информацию о файлах, которые находятся в загрузке.
        /// </summary>
        public class DownloadInfo
        {
            /// <summary>
            /// Имя скачиваемого файла.
            /// </summary>
            public string FileName { get; set; }

            /// <summary>
            /// Статус скачиваемого файла.
            /// </summary>
            public DownloadType Status { get; set; } = DownloadType.InProcess;

            public DownloadInfo(string fileName)
            {
                FileName = fileName;
            }
        }
    }
}
