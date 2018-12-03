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

        private int threadCount;

        private SafeQueue<Action> taskQueue = new SafeQueue<Action>();
        private Thread[] threads;
        private ManualResetEvent threadEvent = new ManualResetEvent(false);

        /// <summary>
        /// Инициализирует и запускает фиксированное количество потоков.
        /// </summary>
        /// <param name="n">Количество запускаемых потоков.</param>
        public MyThreadPool(int n)
        {
            threadCount = n;

            token = cts.Token;

            threads = new Thread[threadCount];

            for (int i = 0; i < threadCount; ++i)
            {
                int j = i;
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
            MyTask<TResult> newTask = new MyTask<TResult>(func, this);

            Action action = () => newTask.Compute();

            taskQueue.Enqueue(action);

            threadEvent.Set();

            return newTask;
        }


        /// <summary>
        /// Метод, который исполняется в каждом из потоков.
        /// Позволяет каждому из потоков обработать функцию task'а.
        /// Завершается при запросе cancellation token.
        /// </summary>
        private void Run()
        {
            while (true)
            {
                threadEvent.WaitOne();

                if (token.IsCancellationRequested)
                {
                    return;
                }
                else if (taskQueue.Size == 1)
                {
                    threadEvent.Reset();
                }

                Action action = null;
                lock (lockObject)
                {
                    if (taskQueue.Size != 0)
                    {
                        action = taskQueue.Dequeue();
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

        /// <summary>
        /// Этот метод закрывает пул потоков.
        /// </summary>
        public void Shutdown()
        {
            cts.Cancel();
            cts.Dispose();

            threadEvent.Set();
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
        /// Класс предоставляет интерфейс для управления задачами.
        /// </summary>
        /// <typeparam name="TResult">Тип возвращаемого задачей значения.</typeparam>
        private class MyTask<TResult> : IMyTask<TResult>
        {
            private volatile AggregateException aggException = null;

            private object lockObject = new object();

            private volatile bool hasValue = false;

            /// <summary>
            /// Свойство возвращает true, если значение вычислено, false иначе.
            /// </summary>
            public bool IsCompleted { get { return hasValue; } }

            private TResult result;
            private Func<TResult> func;

            private MyThreadPool threadPool;
            private SafeQueue<Action> nextActions = new SafeQueue<Action>();

            private ManualResetEvent valueEvent = new ManualResetEvent(false);

            /// <summary>
            /// Конструктор класса задач. Помещает задачу в заданный пул poolQueue.
            /// </summary>
            /// <param name="supplier"></param>
            /// <param name="nextActions"></param>
            public MyTask(Func<TResult> func, MyThreadPool threadPool)
            {
                this.func = func;

                this.threadPool = threadPool;
            }


            /// <summary>
            /// Метод, отвечающий за вычисление функции. 
            /// Исполняет функцию и обрабатывает её возможные исключения. 
            /// Этот метод запускается в одном из потоков MyThreadPool.
            /// </summary>
            public void Compute()
            {
                try
                {
                    result = func();
                }
                catch (Exception ex)
                {
                    aggException = new AggregateException(ex);
                }

                if (aggException == null)
                {
                    hasValue = true;
                }

                AddActionsToPool();

                func = null;

                valueEvent.Set();
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

                TNewResult supplier()
                {
                    TNewResult result = func(Result);

                    return result;
                }

                MyTask<TNewResult> nextTask = new MyTask<TNewResult>(supplier, threadPool);

                Action action = () => nextTask.Compute();

                if (hasValue)
                {
                    threadPool.taskQueue.Enqueue(action);

                    threadPool.threadEvent.Set();
                }
                else
                {
                    nextActions.Enqueue(action);

                    if (hasValue)
                    {
                        AddActionsToPool();
                    }
                }

                return nextTask;
            }


            /// <summary>
            /// Свойство, возвращающее результат вычислений. 
            /// Ждёт событие valueEvent, которое взводится после
            /// </summary>
            public TResult Result
            {
                get
                {
                    valueEvent.WaitOne();

                    if (aggException != null)
                    {
                        throw aggException;
                    }

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
                    threadPool.taskQueue.Enqueue(action);
                }

                threadPool.threadEvent.Set();
            }
        }

    }
}
