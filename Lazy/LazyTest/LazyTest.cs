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
                int n = 8;

                int res = 1;
                while (n != 0)
                {
                    if (n % 2 == 1)
                        res *= a;

                    a *= a;

                    n /= 2;
                }

                return res;

            }

            SimpleLazy<int> binpowLazy = LazyFactory.CreateSimpleLazy(Binpow);

            Assert.AreEqual(Binpow(), binpowLazy.Get);
        }

        [TestMethod, TestCategory("A")]
        public void RandomTest()
        {
            int GetRandom()
            {
                int randomRes = this.random.Next(50, 100);

                return randomRes;
            }

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

            SimpleLazy<Object> simpleLazy = LazyFactory.CreateSimpleLazy<Object>(NullSupplier);

            Assert.IsNull(simpleLazy.Get);
        }

        [TestMethod, TestCategory("A")]
        public void ThreadTest()
        {
            string HelloFunc()
            {
                return "Hello";

            }

            const int n = 10;
            SafeLazy<string> helloLazy = LazyFactory.CreateSafeLazy(HelloFunc);

            string[] resStrings = new string[n];

            Thread[] threads = new Thread[n];

            for (int i = 0; i < n; ++i)
            {
                int k = i;
                threads[i] = new Thread(() => { resStrings[k] = helloLazy.Get; });
            }

            foreach (Thread thread in threads)
            {
                thread.Start();
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            for (int i = 0; i < n; ++i)
                Assert.AreEqual(HelloFunc(), resStrings[i]);
        }

        [TestMethod, TestCategory("A")]
        public void SleepingThreadsTest()
        {
            int GetRandom()
            {
                int res = random.Next(0, 100);

                return res;
            }

            SafeLazy<int> safeLazy = LazyFactory.CreateSafeLazy(GetRandom);

            const int n = 10;

            int[,] resultMatrix = new int[n, n];

            Thread[] threads = new Thread[n];

            for (int i = 0; i < n; ++i)
            {
                threads[i] = new Thread(() =>
                {
                    Thread.Sleep(random.Next(0, 100) * 10);

                    int k = i;

                    for (int j = 0; j < n; ++j)
                    {
                        resultMatrix[k, j] = safeLazy.Get;
                    }
                });
            }

            foreach (Thread thread in threads)
            {
                thread.Start();
            }

            foreach (Thread thread in threads)
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
