using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SimpleFTPClient
{
    /// <summary>
    /// Класс, реализующий работу клиента.
    /// </summary>
    public class Client
    {
        private const int port = 8238;


        public static async Task<string> SendRequest(string server, string command)
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
