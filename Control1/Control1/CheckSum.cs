using System.IO;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace Control1
{
    class CheckSum
    {
        public CheckSum()
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
            string hashString = "";

            List<string> hashList = new List<string>();

            foreach (var item in dir.GetDirectories())
            {
                hashString += GetDirCheckSum(item.FullName);
            }
            foreach (var item in dir.GetFiles())
            {
                hashList.Add(GetFileCheckSum(item.FullName));
            }

            hashList.Sort();

            foreach(string hash in hashList)
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
