using System;
using System.Threading;

namespace MyThreadPool
{
    /// <summary>
    /// Класс реализует поведение пула потоков.
    /// </summary>
    public class MyThreadPool
    {
        private readonly object lockObject = new object();
        private readonly object eventLockObject = new object();

        private CancellationTokenSource cts = new CancellationTokenSource();
        private CancellationToken token;

        private readonly int threadCount = 0;
        private int stopCount = 0;

        private SafeQueue<Action> taskQueue = new SafeQueue<Action>();
        private Thread[] threads;
        private ManualResetEvent threadEvent = new ManualResetEvent(false);
        private ManualResetEvent allstopEvent = new ManualResetEvent(false);

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
                threads[i] = new Thread(Run) { IsBackground = true };

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
            var newTask = new MyTask<TResult>(func, this);

            Action action = () => newTask.Compute();

            AddToQueue(action);

            return newTask;
        }

        /// <summary>
        /// Метод, позваляющий добавить action в очередь пула.
        /// </summary>
        /// <param name="action"></param>
        private void AddToQueue(Action action)
        {
            lock (eventLockObject)
            {
                taskQueue.Enqueue(action);

                threadEvent.Set();
            }
        }


        /// <summary>
        /// Метод, который исполняется в каждом потоке.
        /// Позволяет каждому потоку обработать функцию task'а.
        /// Завершается при запросе cancellation token.
        /// </summary>
        private void Run()
        {
            while (true)
            {
                threadEvent.WaitOne();

                lock (eventLockObject)
                {
                    if (taskQueue.Size == 1)
                    {
                        threadEvent.Reset();
                    }
                }

                if (token.IsCancellationRequested)
                {
                    threadEvent.Set();

                    lock (lockObject)
                    {
                        stopCount++;
                    }

                    if (stopCount == threadCount)
                        allstopEvent.Set();

                    return;
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

            allstopEvent.WaitOne();
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

            private readonly object lockObject = new object();

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
            /// Конструктор класса задач.
            /// </summary>
            /// <param name="func">Функция, которую необходимо вычислить в отдельном потоке.</param>
            /// <param name="threadPool">Экземпляр класса MyThreadPool.</param>
            public MyTask(Func<TResult> func, MyThreadPool threadPool)
            {
                this.func = func;

                this.threadPool = threadPool;
            }


            /// <summary>
            /// Метод, отвечающий за вычисление функции. 
            /// Исполняет функцию и обрабатывает её возможные исключения.
            /// После этого добавляет все зависимые задачи в основной пул задач MyThreadPool.
            /// Взводит сигнал valueEvent.
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

                var nextTask = new MyTask<TNewResult>(supplier, threadPool);

                Action action = () => nextTask.Compute();

                if (hasValue)
                {
                    threadPool.AddToQueue(action);
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
            /// Ожидает сигнала valueEvent. В случае, если функция объекта завершается с исключением,
            /// метод также завершается с исключением AggregateError. 
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
            /// Добавляет все вычисления, зависящие от текущего, в очередь задач MyThreadPool.
            /// </summary>
            public void AddActionsToPool()
            {
                while (nextActions.Size != 0)
                {
                    Action action = nextActions.Dequeue();
                    //threadPool.taskQueue.Enqueue(action);
                    threadPool.AddToQueue(action);
                };
            }
        }

    }
}
