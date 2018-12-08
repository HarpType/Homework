using System;
using System.Net.Sockets;
using System.IO;

namespace SimpleFTPClient
{
    /// <summary>
    /// Класс, реализующий работу клиента.
    /// </summary>
    class Client
    {
        /// <summary>
        /// Метод, отвечающий за связь клиента с сервером. Запрашивает адрес сервера и его порт
        /// Отправляет команды клиента на выполнение серверу.
        /// </summary>
        public void Start()
        {
            while (true)
            {
                string server;
                string port = "";

                Console.WriteLine("Введите адрес сервера");
                server = Console.ReadLine();

                while (true)
                {
                    Console.WriteLine("Введите порт");
                    port = Console.ReadLine();

                    if (!int.TryParse(port, out int p))
                    {
                        Console.WriteLine("Неверный формат");
                    }
                    else
                    {
                        break;
                    }
                }


                 string command;

                Console.WriteLine("Введите команду");
                command = Console.ReadLine();

                try
                {
                    using (var client = new TcpClient(server, int.Parse(port)))
                    {
                        var stream = client.GetStream();
                        var writer = new StreamWriter(stream);

                        writer.WriteLine(command);
                        writer.Flush();

                        var reader = new StreamReader(stream);
                        var data = reader.ReadToEnd();
                        Console.WriteLine(data);
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Невозможно подключиться к серверу");
                }
            }
        }
    }
}
