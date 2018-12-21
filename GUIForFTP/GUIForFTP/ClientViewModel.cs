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
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<FileInfo> Files { get; set; } = new ObservableCollection<FileInfo>();

        public async Task ConnectToServer(string address, int port)
        {
            string path = @"D:\Temp\";

            string command = "1 " + path;

            string dirInfo = await Client.SendRequest(address, port, command);

            ParseInfo(dirInfo, path);
        }

        public void ParseInfo(string dirInfo, string path)
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
