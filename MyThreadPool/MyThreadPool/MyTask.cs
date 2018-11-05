using System;
using System.Collections.Generic;

namespace MyThreadPool
{
    /// <summary>
    /// Класс предоставляет интерфейс для управления задачей.
    /// </summary>
    /// <typeparam name="TResult">Тип возвращаемого задачей значения.</typeparam>
    public class MyTask<TResult> : IMyTask<TResult>
    {
        private AggregateException aggException = null;

        private object lockObject = new object();

        private volatile bool hasValue = false;
        public bool IsCompleted { get { return hasValue; } }

        private TResult result;
        private Func<TResult> func;

        private SafeQueue<Action> poolQueue;
        private SafeQueue<Action> nextActions = new SafeQueue<Action>();

        /// <summary>
        /// Конструктор класса задач.
        /// </summary>
        /// <param name="func">Функция для вычисления.</param>
        /// <param name="poolQueue">Ссылка на очередь пула.</param>
        public MyTask(Func<TResult> func, SafeQueue<Action> poolQueue)
        {
            this.func = func;

            this.poolQueue = poolQueue;
        }

        public MyTask(Func<TResult> func)
        {
            this.func = func;
        }

        /// <summary>
        /// Добавляет в очередь задач новую, которая опирается на результатах
        /// текущей.
        /// </summary>
        /// <typeparam name="TNewResult">Тип новых вычислений.</typeparam>
        /// <param name="func">Функция, описывающая новые вычисления.</param>
        /// <returns>Новая задача.</returns>
        public MyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> func)
        {
            TNewResult supplier() => func(Result);

            var nextTask = new MyTask<TNewResult>(supplier, poolQueue);

            Action action =
                () =>
                {
                    TNewResult result = nextTask.Result;
                };

            if (hasValue)
            {
                poolQueue.Enqueue(action);
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
        /// Свойство, возвращающее результат вычислений. Организует потокобезопасное ленивое вычисление
        /// заданной функции. После вычисления результатов добавляет все зависимые вычисления в пул потоков.
        /// Если функция имеет исключение, то при первом вызове исключение будет помещено в aggException. 
        /// При последующих вызовах исключение возвращается без вычисления функции.
        /// </summary>
        public TResult Result
        {
            get
            {
                if (aggException != null)
                {
                    throw aggException;
                }

                if (!hasValue)
                {
                    lock (lockObject)
                    {
                        if (!hasValue)
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
                        }
                    }
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
                poolQueue.Enqueue(action);
            }
        }
    }
}
