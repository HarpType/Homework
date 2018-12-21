using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUIForFTP
{
    class ClientViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public List<FileInfo> Files = new List<FileInfo>();

        public async Task ConnectToServer(string address, int port)
        {
            string command = @"1 ...\";

            string dirInfo = await Client.SendRequest(address, port, command);

            ParseInfo(dirInfo);
        }

        public void ParseInfo(string dirInfo)
        {
            string[] info = dirInfo.Split(' ');

            int.TryParse(info[0], out int fileCount);

            for (int i = 1; i <= fileCount; ++i)
            {
                string fileName = info[2 * i - 1];
                if (info[2 * i] == "true")
                {
                    Files.Add(new FileInfo(fileName, true));
                }
                else
                {
                    Files.Add(new FileInfo(fileName, false));
                }
                
            }
        }
    }
}
