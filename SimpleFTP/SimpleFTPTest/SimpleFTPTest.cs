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

            string data = FileCommander.DoListCommand(path).GetAwaiter().GetResult();

            Assert.AreEqual(expectedData, data);
        }

        [TestMethod]
        public void DoGetTest()
        {
            string filePath = RootPath + "/TestDirectories/Directory2/Hello.txt";
            string expectedData = "29 A Hello World! program in C#.";

            string data = FileCommander.DoGetCommand(filePath).GetAwaiter().GetResult();

            Assert.AreEqual(expectedData, data);
        }

        [TestMethod]
        public void ClientDoListTest()
        {
            string expectedData = "2 Hello.txt false Olleh.txt false ";

            string serv = "127.0.0.1";
            string command = "1 " +  RootPath + "/TestDirectories/Directory2";

            var cts = new CancellationTokenSource();
            var ct = cts.Token;

            Server.Start(ct);

            var data = Client.SendRequest(serv, command).GetAwaiter().GetResult();

            Assert.AreEqual(expectedData, data);

            cts.Cancel();
            cts.Dispose();
        }

        [TestMethod]
        public void ClientDoGetCommand()
        {
            string expectedData = "29 A Hello World! program in C#.";

            string serv = "127.0.0.1";
            string command = "2 " + RootPath + "/TestDirectories/Directory2/Hello.txt";

            var cts = new CancellationTokenSource();
            var ct = cts.Token;

            Server.Start(ct);

            var data = Client.SendRequest(serv, command).GetAwaiter().GetResult();

            Assert.AreEqual(expectedData, data);

            cts.Cancel();
            cts.Dispose();
        }

        [TestMethod]
        public void ClientWrongDoGetCommand()
        {
            string serv = "127.0.0.1";
            string command = "2 " +  RootPath + "WrongDirectory/wrong.txt";

            var cts = new CancellationTokenSource();
            var ct = cts.Token;

            Server.Start(ct);

            Thread.Sleep(200);
            var data = Client.SendRequest(serv, command).GetAwaiter().GetResult();

            Assert.AreEqual("-1", data);

            cts.Cancel();
            cts.Dispose();
        }

        [TestMethod]
        public void ClientWrongCommand()
        {
            string serv = "127.0.0.1";
            string command = "wrongCommand";

            var cts = new CancellationTokenSource();
            var ct = cts.Token;

            Server.Start(ct);

            var data = Client.SendRequest(serv, command).GetAwaiter().GetResult();

            Assert.AreEqual("Command not found", data);

            cts.Cancel();
            cts.Dispose();
        }
    }
}
