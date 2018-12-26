using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUIForFTP
{
    /// <summary>
    /// Класс-помощник для скачивания файлов с сервера.
    /// </summary>
    class FileCreator
    {
        /// <summary>
        /// Создаёт файл по указанному пути.
        /// </summary>
        /// <param name="fileName">Имя файла.</param>
        /// <param name="downloadPath">Директория, в которой необходимо создать файл.</param>
        /// <returns>Путь к файлу, если он создан, null в противном случае.</returns>
        public static string Create(string fileName, string downloadPath)
        {
            if (!Directory.Exists(downloadPath))
            {
                return null;
            }

            if (!File.Exists(downloadPath + fileName))
            {
                File.Create(downloadPath + fileName).Dispose();

                return downloadPath + fileName;
            }

            int repeatCount = 1;
            string newFilePath = downloadPath + "(" + repeatCount + ")" + fileName;
            while (File.Exists(newFilePath))
            {
                repeatCount++;
                newFilePath = downloadPath + "(" + repeatCount + ")" + fileName;
            }

            File.Create(newFilePath).Dispose();

            return newFilePath;
        }
    }
}
