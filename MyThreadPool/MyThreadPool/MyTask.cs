using System;
using System.Collections.Generic;

namespace MyThreadPool
{
    public class MyTask<TResult> : IMyTask<TResult>
    {
        private Object lockObject = new object();

        private volatile bool hasValue;
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
            this.hasValue = false;

            this.poolQue = poolQue;
        }
        
        /// <summary>
        /// Добавляет в очередь функций новую, которая опирается на результатах
        /// текущей функции
        /// </summary>
        /// <typeparam name="TNewResult">Тип новых вычислений.</typeparam>
        /// <param name="func">Функция, описывающая новые вычисления</param>
        /// <returns>Новая задача.</returns>
        public MyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> func)
        {
            TNewResult supplier()
            {
                TNewResult result = func(this.Result);

                return result;
            }

            MyTask<TNewResult> nextTask = new MyTask<TNewResult>(supplier, this.poolQue);
            
            Action action = Wrapper(nextTask);

            if (this.hasValue)
            {
                poolQue.Enqueue(action);
            }
            else
            {
                nextActions.Enqueue(action);
            }

            return nextTask;
        }


        /// <summary>
        /// Свойство, возвращающее результат вычислений. Организует потокобезопасное ленивое вычислений
        /// заданной функции. После вычисления результатов добавляет все зависимые вычисления в пул потоков.
        /// </summary>
        public TResult Result
        {
            get
            {
                if (!this.hasValue)
                {
                    lock (this.lockObject)
                    {
                        if (!this.hasValue)
                        {
                            this.result = this.func();
                            this.hasValue = true;

                            AddActionsToPool();

                            this.func = null;
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
            while (this.nextActions.Size != 0)
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
        public Action Wrapper<TNewResult>(MyTask<TNewResult> task)
        {
            void action()
            {
                TNewResult result = task.Result;
            }

            return action;
        }
    }
}
