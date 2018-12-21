using System;
using System.Net.Sockets;
using System.IO;
using System.Threading.Tasks;

namespace SimpleFTPClient
{
    /// <summary>
    /// Класс, реализующий работу клиента.
    /// </summary>
    public class Client
    {
        private const int port = 8238;


        public static async Task<string> SendRequest(string command)
        {
            string data = "";
            try
            {
                using (var client = new TcpClient("127.0.0.1", port))
                {
                    var stream = client.GetStream();
                    var writer = new StreamWriter(stream);

                    await writer.WriteLineAsync(command);
                    await writer.FlushAsync();

                    var reader = new StreamReader(stream);
                    data = await reader.ReadToEndAsync();
                }
            }
            catch (Exception)
            {

            }

            return data;
        }
        /// <summary>
        /// Метод, отвечающий за связь клиента с сервером. Запрашивает адрес сервера и его порт
        /// Отправляет команды клиента на выполнение серверу.
        /// </summary>
        //public static async Task<string> SendRequest(string command)
        //{
        //    try
        //    {
        //        using (var client = new TcpClient("127.0.0.1", port)
        //        {
        //            var stream = client.GetStream();
        //            var writer = new StreamWriter(stream);

        //            writer.WriteLine(command);
        //            writer.Flush();

        //            var reader = new StreamReader(stream);
        //            var data = reader.ReadToEnd();

        //            return data;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Невозможно подключиться к серверу");
        //    }
        //}
    }
}
