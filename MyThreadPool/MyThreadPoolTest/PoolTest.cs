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
            var threadPool = new MyThreadPool.MyThreadPool(1);

            string SayHello() => "Hello";

            var helloTask = threadPool.AddTask<string>(SayHello);

            if (!helloTask.IsCompleted)
            {
                Thread.Sleep(500);
            }

            Assert.AreEqual("Hello", helloTask.Result);
        }

        [TestMethod]
        public void SimpleContinueWithTest()
        {
            var threadPool = new MyThreadPool.MyThreadPool(1);

            string SayHello() => "Hello";
            var helloTask = threadPool.AddTask(SayHello);

            string SayWorld(string str) => str + " World!";
            var helloworldTask = helloTask.ContinueWith(SayWorld);

            if (!helloTask.IsCompleted)
            {
                Thread.Sleep(500);
            }

            Assert.AreEqual("Hello", helloTask.Result);
            Assert.AreEqual("Hello World!", helloworldTask.Result);
        }

        [TestMethod]
        public void ThreadsCountTest()
        {
            const int n = 5;
            var threadPool = new MyThreadPool.MyThreadPool(n);

            Assert.AreEqual(n, threadPool.AliveThreadsCount());
        }

        [TestMethod]
        public void BigPoolTest()
        {
            const int n = 3;
            var threadPool = new MyThreadPool.MyThreadPool(n);

            string SayHello() => "Hello";
            string SayHelloWorld(string hello) => hello + " world!";

            int TakeFive() => 5;
            int Add(int number) => number + 15;

            bool GetTrue() => true;
            string MakeChoice(bool b) => b ? "Good" : "Bad";
            string Introduce(string str) => str + " boy";

            var helloTask = threadPool.AddTask(SayHello);
            var takefiveTask = threadPool.AddTask(TakeFive);
            var gettrueTask = threadPool.AddTask(GetTrue);

            var helloworldTask = helloTask.ContinueWith(SayHelloWorld);
            var addTask = takefiveTask.ContinueWith(Add);
            var choiceTask = gettrueTask.ContinueWith(MakeChoice);

            var introduceTask = choiceTask.ContinueWith(Introduce);


            Assert.AreEqual("Hello", helloTask.Result);
            Assert.AreEqual(5, takefiveTask.Result);
            Assert.AreEqual(true, gettrueTask.Result);
            Assert.AreEqual(n, threadPool.AliveThreadsCount());

            Assert.AreEqual("Hello world!", helloworldTask.Result);
            Assert.AreEqual(20, addTask.Result);
            Assert.AreEqual("Good", choiceTask.Result);
            Assert.AreEqual(n, threadPool.AliveThreadsCount());

            Assert.AreEqual("Good boy", introduceTask.Result);
            Assert.AreEqual(n, threadPool.AliveThreadsCount());
        }

        [TestMethod]
        public void ShutdownTest()
        {
            const int n = 5;
            var threadPool = new MyThreadPool.MyThreadPool(n);

            int Pow()
            {
                int a = 3638899;
                int p = 1580589433;

                int res = 1;
                for (int i = 0; i < p; i++)
                    res *= a;

                return res;
            }

            var heavyTask = threadPool.AddTask(Pow);

            Assert.AreEqual(n, threadPool.AliveThreadsCount());

            threadPool.Shutdown();

            Thread.Sleep(10);
            Assert.AreEqual(1, threadPool.AliveThreadsCount());
            Assert.AreEqual(Pow(), heavyTask.Result);

            Thread.Sleep(500);
            Assert.AreEqual(0, threadPool.AliveThreadsCount());
        }

        [TestMethod]
        public void ExceptionTest()
        {
            var threadPool = new MyThreadPool.MyThreadPool(1);

            int ZeroDivide()
            {
                int x = 0;
                return 5 / x;
            }

            var wrongTask = threadPool.AddTask(ZeroDivide);

            Action wrongAction =
                () => 
                {
                    int result = wrongTask.Result;
                };

            Assert.ThrowsException<AggregateException>(() => wrongTask.Result);

            try
            {
                int result = wrongTask.Result;
            }
            catch (AggregateException ae)
            {
                foreach (Exception ex in ae.InnerExceptions)
                {
                    Assert.ThrowsException<DivideByZeroException>(() => throw ex);
                }
            }
        }
    }
}
