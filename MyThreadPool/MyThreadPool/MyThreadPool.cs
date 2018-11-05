using System;
using System.Threading;

namespace MyThreadPool
{
    /// <summary>
    /// Класс реализует поведение пула потоков.
    /// </summary>
    public class MyThreadPool
    {
        private object lockObject = new object();

        private CancellationTokenSource cts = new CancellationTokenSource();
        private CancellationToken token;

        private SafeQueue<Action> taskQueue = new SafeQueue<Action>();
        private Thread[] threads;

        /// <summary>
        /// Инициализирует и запускает фиксированное количество потоков.
        /// </summary>
        /// <param name="n">Количество запускаемых потоков.</param>
        public MyThreadPool(int n)
        {
            token = cts.Token;

            threads = new Thread[n];

            for (int i = 0; i < n; ++i)
            {
                threads[i] = new Thread(Run);
                threads[i].IsBackground = true;
            }

            foreach (Thread thread in threads)
            {
                thread.Start();
            }
        }

        /// <summary>
        /// Добавляет задачу в очередь.
        /// </summary>
        /// <typeparam name="TResult">Тип возвращаемого функцией значения.</typeparam>
        /// <param name="func">Функция, которую необходимо вычислить.</param>
        public MyTask<TResult> AddTask<TResult>(Func<TResult> func)
        {
            MyTask<TResult> newTask = new MyTask<TResult>(func, taskQueue);

            Action action =
                () =>
                {
                    TResult result = newTask.Result;
                };

            taskQueue.Enqueue(action);

            return newTask;
        }

        /// <summary>
        /// Метод, который постоянно исполняется в каждом из потоков.
        /// В бесконечном цикле происходит проверка завершения пула потоков. 
        /// После проверки каждый свободный поток пытается взять для себя 
        /// новую задачу из очереди и, если таковая имеется, исполняет её.
        /// </summary>
        private void Run()
        {
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                if (taskQueue.Size != 0)
                {
                    Action action;
                    lock (lockObject)
                    {
                        if (taskQueue.Size != 0)
                        {
                            action = taskQueue.Dequeue();
                        }
                        else
                        {
                            action = null;
                        }
                    }

                    if (action != null)
                    {
                        try
                        {
                            action();
                        }
                        catch { };
                    }
                }
            }
        }

        /// <summary>
        /// Этот метод закрывает пул потоков.
        /// </summary>
        public void Shutdown()
        {
            cts.Cancel();
            cts.Dispose();
        }


        /// <summary>
        /// Считает все активные потоки и возвращает их количество.
        /// </summary>
        public int AliveThreadsCount()
        {
            int count = 0;
            foreach (var thread in threads)
            {
                if (thread.IsAlive)
                {
                    count += 1;
                }
            }

            return count;
        }
    }
}
