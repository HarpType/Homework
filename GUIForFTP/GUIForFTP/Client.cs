using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GUIForFTP
{
    /// <summary>
    /// Класс, реализующий работу клиента.
    /// </summary>
    public class Client
    {
        public static async Task<string> SendRequest(string server, int port, string command)
        {
            string data = "";
            try
            {
                using (var client = new TcpClient(server, port))
                {
                    var stream = client.GetStream();
                    var writer = new StreamWriter(stream);

                    await writer.WriteLineAsync(command);
                    await writer.FlushAsync();

                    var reader = new StreamReader(stream);
                    data = await reader.ReadToEndAsync();
                }
            }
            catch (SocketException)
            {
                throw new Exception("Ошибка подключения к серверу");
            }

            return data;
        }
    }
}
