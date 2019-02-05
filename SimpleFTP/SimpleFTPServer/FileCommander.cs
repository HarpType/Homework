using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFTPServer
{
    public class FileCommander
    {
        /// <summary>
        /// Формирует ответ на запрос List.
        /// </summary>
        /// <param name="path">Путь к директории.</param>
        /// <returns>Ответ в указанном формате.</returns>
        public static string DoListCommand(string dirPath)
        {
            string dataString;

            DirectoryInfo dir = null;

            try
            {
                dir = new DirectoryInfo(dirPath);
            }
            catch (ArgumentException)
            {
                return "-1";
            }

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
        public static string DoGetCommand(string filePath)
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
