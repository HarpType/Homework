
namespace SimpleFTPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new Server();

            Server.Start();
        }
    }
}
