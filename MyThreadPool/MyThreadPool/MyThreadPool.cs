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
        /// Добавляет задачу в очередь. Возвращает обработчик задачи.
        /// </summary>
        /// <typeparam name="TResult">Тип возвращаемого функцией значения.</typeparam>
        /// <param name="func">Функция, которую необходимо вычислить.</param>
        public IMyTask<TResult> AddTask<TResult>(Func<TResult> func)
        {
            IMyTask<TResult> newTask = new MyTask<TResult>(func, taskQueue);

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

        /// <summary>
        /// Класс предоставляет интерфейс для управления задачей.
        /// </summary>
        /// <typeparam name="TResult">Тип возвращаемого задачей значения.</typeparam>
        private class MyTask<TResult> : IMyTask<TResult>
        {
            private AggregateException aggException = null;

            private object lockObject = new object();

            private volatile bool hasValue = false;
            public bool IsCompleted { get { return hasValue; } }

            private TResult result;
            private Func<TResult> func;

            private SafeQueue<Action> ownersTaskQueue;
            private SafeQueue<Action> nextActions = new SafeQueue<Action>();

            /// <summary>
            /// Конструктор класса задач. Помещает задачу в заданный пул poolQueue.
            /// </summary>
            /// <param name="supplier"></param>
            /// <param name="nextActions"></param>
            public MyTask(Func<TResult> func, SafeQueue<Action> ownersTaskQueue)
            {
                this.func = func;

                Action action = ActionWrapper(this.func);

                this.ownersTaskQueue = ownersTaskQueue;
                this.ownersTaskQueue.Enqueue(action);
            }

            private Action ActionWrapper(Func<TResult> func)
            {
                return () =>
                {
                    try
                    {
                        result = func();
                    }
                    catch (Exception ex)
                    {
                        aggException = new AggregateException(ex);

                        throw aggException;
                    }

                    hasValue = true;
                    func = null;

                    AddActionsToPool();
                };
            }

            /// <summary>
            /// Добавляет в очередь задач новую, которая опирается на результатах
            /// текущей.
            /// </summary>
            /// <typeparam name="TNewResult">Тип новых вычислений.</typeparam>
            /// <param name="func">Функция, описывающая новые вычисления.</param>
            /// <returns>Новая задача.</returns>
            public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> func)
            {
                MyTask<TNewResult> nextTask;

                //Action action =
                //    () =>
                //    {
                //        TNewResult result = nextTask.Result;
                //    };

                TNewResult supplier() => func(result);

                if (hasValue)
                {
                    nextTask = new MyTask<TNewResult>(supplier, this.ownersTaskQueue);
                }
                else
                {
                    nextTask = new MyTask<TNewResult>(supplier, nextActions);
                    if (hasValue)
                    {
                        AddActionsToPool();
                    }
                }

                return nextTask;
            }


            /// <summary>
            /// Свойство, возвращающее результат вычислений. Организует потокобезопасное ленивое вычисление
            /// заданной функции. После вычисления результатов добавляет все зависимые вычисления в пул потоков.
            /// Если функция имеет исключение, то при первом вызове исключение будет помещено в aggException. 
            /// При последующих вызовах исключение возвращается без вычисления функции.
            /// </summary>
            public TResult Result
            {
                get
                {
                    while (!hasValue)
                    {
                        if (aggException != null)
                        {
                            throw aggException;
                        }
                    }

                    //if (!hasValue)
                    //{
                    //    lock (lockObject)
                    //    {
                    //        if (!hasValue)
                    //        {
                    //            try
                    //            {
                    //                result = func();
                    //            }
                    //            catch (Exception ex)
                    //            {
                    //                aggException = new AggregateException(ex);

                    //                throw aggException;
                    //            }

                    //            hasValue = true;
                    //            func = null;

                    //            AddActionsToPool();
                    //        }
                    //    }
                    //}

                    return result;
                }
            }

            /// <summary>
            /// Добавляет все вычисления, зависящие от текущего, в пул потоков.
            /// </summary>
            public void AddActionsToPool()
            {
                while (nextActions.Size != 0)
                {
                    Action action = nextActions.Dequeue();
                    ownersTaskQueue.Enqueue(action);
                }
            }
        }

    }
}
