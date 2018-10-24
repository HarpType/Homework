using System;
using System.Threading;

namespace MyThreadPool
{
    /// <summary>
    /// Класс реализует поведение пула потоков.
    /// </summary>
    public class MyThreadPool
    {
        Object lockObject = new object();

        CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken token;

        private SafeQueue<Action> que = new SafeQueue<Action>();
        private Thread[] threads;

        /// <summary>
        /// Инициализирует и запускает фиксированное количество потоков.
        /// </summary>
        /// <param name="n">Количество запускаемых потоков.</param>
        public MyThreadPool(int n)
        {
            this.token = cts.Token;
            threads = new Thread[n];

            for (int i = 0; i < n; ++i)
            {
                threads[i] = new Thread(Run);
                threads[i].IsBackground = true;
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }
        }

        /// <summary>
        /// Добавляет задачу в очередь.
        /// </summary>
        /// <typeparam name="TResult">Тип возвращаемого значения.</typeparam>
        /// <param name="func">Функция.</param>
        /// <returns></returns>
        public MyTask<TResult> AddTask<TResult>(Func<TResult> func)
        {
            MyTask<TResult> newTask = new MyTask<TResult>(func, this.que);

            Action action = Wrapper<TResult>(newTask);

            que.Enqueue(action);

            return newTask;
        }


        /// <summary>
        /// Оборачививает значение MyTask в Action.
        /// </summary>
        /// <typeparam name="TResult">Тип возвращаемого задачей значения.</typeparam>
        /// <param name="task">Задача.</param>
        /// <returns></returns>
        private Action Wrapper<TResult>(MyTask<TResult> task)
        {
            void action()
            {
                    TResult result = task.Result;
            }

            return action;
        }


        /// <summary>
        /// Метод, который постоянное исполняется в потоках.
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

                if (que.Size != 0)
                {
                    Action action;
                    lock (lockObject)
                    {
                        if (que.Size != 0)
                        {
                            action = que.Dequeue();
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
            foreach (var thread in this.threads)
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
