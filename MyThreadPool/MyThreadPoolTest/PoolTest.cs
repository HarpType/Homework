using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

using MyThreadPool;

namespace MyThreadPoolTest
{
    [TestClass]
    public class PoolTest
    {
        [TestMethod]
        public void SimpleTest()
        {
            MyThreadPool.MyThreadPool threadPool = new MyThreadPool.MyThreadPool(1);

            string SayHello() => "Hello";

            MyTask<string> helloTask = threadPool.AddTask<string>(SayHello);
            //MyTask<string> helloTask = new MyTask<string>(SayHello);

            Thread.Sleep(2000);
            Assert.AreEqual("Hello", helloTask.Result);
        }

        [TestMethod]
        public void SimpleContinueTest()
        {
            MyThreadPool.MyThreadPool threadPool = new MyThreadPool.MyThreadPool(2);

            string SayHello() => "Hello";

            MyTask<string> helloTask = threadPool.AddTask<string>(SayHello);

            string SayWorld(string str) => str + " World!";

            MyTask<string> helloworldTask = helloTask.ContinueWith<string>(SayWorld);

            //Thread.Sleep(2000);
            Assert.AreEqual("Hello", helloTask.Result);
            Assert.AreEqual("Hello World!", helloworldTask.Result);
        }
    }
}
