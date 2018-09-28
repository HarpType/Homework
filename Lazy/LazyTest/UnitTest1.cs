using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using Lazy;

namespace LazyTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestAdd()
        {
            int Add()
            {
                return 10 + 25;
            }

            SimpleLazy<int> simpleLazy = LazyFactory.CreateSimpleLazy(Add);

            Assert.AreEqual(Add(), simpleLazy.Get);
        }

        [TestMethod]
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

            SimpleLazy<int> simpleLazy = LazyFactory.CreateSimpleLazy(Binpow);

            Assert.AreEqual(Binpow(), simpleLazy.Get);
        }

        [TestMethod]
        public void LazyNullTest()
        {
            bool? NullSupplier()
            {
                return null;
            }

            SimpleLazy<bool?> simpleLazy = LazyFactory.CreateSimpleLazy(NullSupplier);

            Assert.AreEqual(null, simpleLazy.Get);
        }

        [TestMethod]
        public void ThreadTest()
        {
            int Binpow()
            {
                int a = 3;
                int k = 8;

                int res = 1;
                while (k != 0)
                {
                    if (k % 2 == 1)
                        res *= a;

                    a *= a;

                    k /= 2;
                }

                return res;

            }

            const int n = 10;
            SafeLazy<int> safeLazy = LazyFactory.CreateSafeLazy(Binpow);

            Thread[] threads = new Thread[n];

            for (int i = 0; i < n; ++i)
            {
                int k = i;
                threads[i] = new Thread(() => { int res = safeLazy.Get; });
            }

            foreach (Thread thread in threads)
            {
                thread.Start();
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            Assert.AreEqual(Binpow(), safeLazy.Get);
        }

        [TestMethod]
        public void SleepingThreadsTest()
        {
            int GetRandom()
            {
                Random random = new Random();

                int res = random.Next(0, 100);

                return res;
            }

            SafeLazy<int> safeLazy = LazyFactory.CreateSafeLazy(GetRandom);

            const int n = 10;

            Thread[] threads = new Thread[n];

            for (int i = 0; i < n; ++i)
            {
                threads[i] = new Thread(() =>
                {
                    Random random = new Random();
                    const int k = 15;

                    Thread.Sleep(random.Next(0, 100) * 10);

                    for (int j = 0; j < k; ++j)
                    {
                        int res = safeLazy.Get;
                    }
                });
            }

            foreach (Thread thread in threads)
                thread.Start();

            foreach (Thread thread in threads)
                thread.Join();

            int result = safeLazy.Get;
            Assert.AreEqual(result, safeLazy.Get);
        }
    }
}
