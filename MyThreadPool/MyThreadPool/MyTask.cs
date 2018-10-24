using System;
using System.Collections.Generic;

namespace MyThreadPool
{
    public class MyTask<TResult> : IMyTask<TResult>
    {
        AggregateException aggException = null;

        private Object lockObject = new object();

        private volatile bool hasValue;
        public bool IsCompleted { get { return hasValue; } }

        private TResult result;
        private Func<TResult> func;

        private SafeQueue<Action> poolQue;
        private Queue<Action> nextActions = new Queue<Action>();

        /// <summary>
        /// Конструктор класса задач.
        /// </summary>
        /// <param name="func">Функция для вычисления.</param>
        /// <param name="poolQue">Ссылка на очередь пула.</param>
        public MyTask(Func<TResult> func, SafeQueue<Action> poolQue)
        {
            this.func = func;
            this.hasValue = false;

            this.poolQue = poolQue;
        }

        public MyTask(Func<TResult> func)
        {
            this.func = func;
            this.hasValue = false;
        }

        /// <summary>
        /// Добавляет в очередь функций новую, которая опирается на результатах
        /// текущей функции.
        /// </summary>
        /// <typeparam name="TNewResult">Тип новых вычислений.</typeparam>
        /// <param name="func">Функция, описывающая новые вычисления.</param>
        /// <returns>Новая задача.</returns>
        public MyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> func)
        {
            TNewResult supplier()
            {
                TNewResult result = func(this.Result);

                return result;
            }

            MyTask<TNewResult> nextTask = new MyTask<TNewResult>(supplier, this.poolQue);
            
            Action action = ActionWrapper(nextTask);

            if (this.hasValue)
            {
                poolQue.Enqueue(action);
                if (!this.hasValue)
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
        /// Если функция имеет исключение, то при первом вызове он будет помещён aggException. 
        /// При последующих вызовах исключение возвращается без вычисления функции.
        /// </summary>
        public TResult Result
        {
            get
            {
                if (this.aggException != null)
                {
                    throw this.aggException;
                }

                if (!this.hasValue)
                {
                    lock (this.lockObject)
                    {
                        if (!this.hasValue)
                        {
                            try
                            {
                                this.result = this.func();
                            }
                            catch (Exception ex)
                            {
                                this.aggException = new AggregateException(ex);

                                throw this.aggException;
                            }

                            this.hasValue = true;
                            this.func = null;

                            AddActionsToPool();
                        }
                    }
                }

                return this.result;
            }
        }

        /// <summary>
        /// Добавляет все вычисления, зависящие от текущего, в пул потоков.
        /// </summary>
        public void AddActionsToPool()
        {
            while (this.nextActions.Count != 0)
            {
                Action action = nextActions.Dequeue();
                poolQue.Enqueue(action);
            }
        }

        /// <summary>
        /// Оборачивает новую задачу в Action.
        /// </summary>
        /// <typeparam name="TNewResult">Тип нового вычисления.</typeparam>
        /// <param name="task">Объект нового вычисления.</param>
        public Action ActionWrapper<TNewResult>(MyTask<TNewResult> task)
        {
            void action()
            {
                TNewResult result = task.Result;
            }

            return action;
        }
    }
}
