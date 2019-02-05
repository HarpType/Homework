using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Control1
{
    public class SafeCheckSum
    {
        public SafeCheckSum()
        {
        }

        public string GetCheckSum(string path)
        {
            FileAttributes attr = File.GetAttributes(path);
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                return GetDirCheckSum(path);
            else
                return GetFileCheckSum(path);
        }

        private string GetDirCheckSum(string dirPath)
        {
            DirectoryInfo dir = new DirectoryInfo(dirPath);

            Task<string> dirTask = null;
            List<Task<string>> taskList = new List<Task<string>>();

            foreach (var item in dir.GetDirectories())
            {
                dirTask = new Task<string>(path => GetDirCheckSum((string)path), item.FullName);
                dirTask.Start();
            }

            foreach (var item in dir.GetFiles())
            {
                Task<string> fileTask = new Task<string>(path => GetFileCheckSum((string)path), item.FullName);
                taskList.Add(fileTask);
                fileTask.Start();
            }

            string hashString = "";
            if (dirTask != null)
                hashString += dirTask.Result;

            List<string> hashList = new List<string>();

            foreach (Task<string> task in taskList)
                hashList.Add(task.Result);

            hashList.Sort();

            foreach (string hash in hashList)
                hashString += hash;

            using (var md5 = MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.Default.GetBytes(dir.FullName + hashString);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                return System.Text.Encoding.Default.GetString(hashBytes);
            }
        }


        private string GetFileCheckSum(string filePath)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    byte[] hashBytes = md5.ComputeHash(stream);
                    return System.Text.Encoding.Default.GetString(hashBytes);
                }
            }
        }
    }
}
