using System;
using System.Collections.Generic;

namespace MyThreadPool
{
    public class MyTask<TResult> : IMyTask<TResult>
    {
        AggregateException aggException = null;

        private object lockObject = new object();

        private volatile bool hasValue = false;
        public bool IsCompleted { get { return hasValue; } }

        private TResult result;
        private Func<TResult> func;

        private SafeQueue<Action> poolQue;
        private SafeQueue<Action> nextActions = new SafeQueue<Action>();

        /// <summary>
        /// Конструктор класса задач.
        /// </summary>
        /// <param name="func">Функция для вычисления.</param>
        /// <param name="poolQue">Ссылка на очередь пула.</param>
        public MyTask(Func<TResult> func, SafeQueue<Action> poolQue)
        {
            this.func = func;

            this.poolQue = poolQue;
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
            TNewResult supplier()
            {
                TNewResult result = func(Result);

                return result;
            }

            MyTask<TNewResult> nextTask = new MyTask<TNewResult>(supplier, poolQue);

            Action action =
                () =>
                {
                    TNewResult result = nextTask.Result;
                };

            if (hasValue)
            {
                poolQue.Enqueue(action);
                if (!hasValue)
                    AddActionsToPool();
            }
            else
            {
                nextActions.Enqueue(action);
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
                poolQue.Enqueue(action);
            }
        }
    }
}
