using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using Lazy;

namespace LazyTest
{
    [TestClass]
    public class LazyTest
    {
        private Random random;

        public LazyTest()
        {
            random = new Random();
        }

        [TestMethod, TestCategory("A")]
        public void TestBinpow()
        {
            int Binpow()
            {
                int a = 3;
                int p = 8;

                int res = 1;
                while (p != 0)
                {
                    if (p % 2 == 1)
                        res *= a;

                    a *= a;

                    p /= 2;
                }

                return res;

            }

            SimpleLazy<int> binpowLazy = LazyFactory.CreateSimpleLazy(Binpow);

            Assert.AreEqual(Binpow(), binpowLazy.Get);
        }

        [TestMethod, TestCategory("A")]
        public void RandomTest()
        {
            int GetRandom() => this.random.Next(50, 100);

            SimpleLazy<int> randomLazy = LazyFactory.CreateSimpleLazy(GetRandom);

            int result = randomLazy.Get;

            const int n = 10;
            for (int i = 0; i < n; ++i)
            {
                Assert.AreEqual(result, randomLazy.Get);
            }
        }

        [TestMethod, TestCategory("A")]
        public void LazyNullTest()
        {
            Func<Object> NullSupplier() => null;

            SimpleLazy<Object> simpleNullLazy = LazyFactory.CreateSimpleLazy<Object>(NullSupplier);

            Assert.IsNull(simpleNullLazy.Get);
        }

        [TestMethod, TestCategory("A")]
        public void ThreadTest()
        {
            string HelloFunc() => "Hello";

            const int n = 10;
            SafeLazy<string> helloLazy = LazyFactory.CreateSafeLazy(HelloFunc);

            string[] resStrings = new string[n];

            Thread[] threads = new Thread[n];

            for (int i = 0; i < n; ++i)
            {
                int k = i;
                threads[i] = new Thread(() => { resStrings[k] = helloLazy.Get; });
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            for (int i = 0; i < n; ++i)
                Assert.AreEqual(HelloFunc(), resStrings[i]);
        }

        [TestMethod, TestCategory("A")]
        public void SleepingThreadsTest()
        {
            int GetRandom() => this.random.Next(0, 100);

            SafeLazy<int> safeLazy = LazyFactory.CreateSafeLazy(GetRandom);

            const int n = 10;

            int[,] resultMatrix = new int[n, n];

            Thread[] threads = new Thread[n];

            for (int i = 0; i < n; ++i)
            {
                int k = i;
                threads[i] = new Thread(() =>
                {
                    Thread.Sleep(this.random.Next(0, 100) * 10);

                    for (int j = 0; j < n; ++j)
                    {
                        resultMatrix[k, j] = safeLazy.Get;
                    }
                });
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            int result = resultMatrix[0, 0];

            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    Assert.AreEqual(result, resultMatrix[i, j]);
                }
            }
        }
    }
}
