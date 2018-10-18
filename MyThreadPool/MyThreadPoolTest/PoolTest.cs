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
    }
}
