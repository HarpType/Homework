using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleFTPServer;
using SimpleFTPClient;
using System.Threading;

namespace SimpleFTPTest
{
    [TestClass]
    public class SimpleFTPTest
    {
        private readonly string RootPath =
            new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;

        [TestMethod]
        public void DoListTest()
        {
            string path = RootPath + "/TestDirectories/Directory1";

            string expectedData = "3 TestDirectory true TestFile1.txt false TestFile2.txt false ";

            string data = FileCommander.DoListCommand(path);

            Assert.AreEqual(expectedData, data);
        }

        [TestMethod]
        public void DoGetTest()
        {
            string filePath = RootPath + "/TestDirectories/Directory2/Hello.txt";
            string expectedData = "29 A Hello World! program in C#.";

            string data = FileCommander.DoGetCommand(filePath);

            Assert.AreEqual(expectedData, data);
        }

        [TestMethod]
        public void ClientDoListTest()
        {
            string expectedData = "2 Hello.txt false Olleh.txt false ";

            string serv = "127.0.0.1";
            string command = "1 " +  RootPath + "/TestDirectories/Directory2";

            //var server = new Server();
            //server.Start();

            var data = Client.SendRequest(serv, command).GetAwaiter().GetResult();

            Assert.AreEqual(expectedData, data);

            //server.Shutdown();
        }

        Server server = new Server();

        [TestInitialize]
        public void StartServer()
        {
            server.Start();
        }

        [TestCleanup]
        public void ShutdownServer()
        {
            server.Shutdown();
        }

        [TestMethod]
        public void ClientDoGetCommand()
        {
            string expectedData = "29 A Hello World! program in C#.";

            string serv = "127.0.0.1";
            string command = "2 " + RootPath + "/TestDirectories/Directory2/Hello.txt";

            //var server = new Server();
            //server.Start();

            var data = Client.SendRequest(serv, command).GetAwaiter().GetResult();

            Assert.AreEqual(expectedData, data);

           // server.Shutdown();
        }

        [TestMethod]
        public void ClientWrongDoGetCommand()
        {
            string serv = "127.0.0.1";
            string command = "2 " +  RootPath + "WrongDirectory/wrong.txt";

            //var server = new Server();
            //server.Start();

            var data = Client.SendRequest(serv, command).GetAwaiter().GetResult();

            Assert.AreEqual("-1", data);

            //server.Shutdown();
        }

        [TestMethod]
        public void ClientWrongCommand()
        {
            string serv = "127.0.0.1";
            string command = "wrongCommand";

            //var server = new Server();
            //server.Start();

            var data = Client.SendRequest(serv, command).GetAwaiter().GetResult();

            Assert.AreEqual("Command not found", data);

            //server.Shutdown();
        }
    }
}
