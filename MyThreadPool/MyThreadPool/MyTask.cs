using System;
using System.Collections.Generic;

namespace MyThreadPool
{
    public class MyTask<TResult> : IMyTask<TResult>
    {
        public bool IsCompleted { get; private set; }

        private TResult result;
        private Func<TResult> func;

        private Queue<Action> poolQue;
        private Queue<Action> nextActions = new Queue<Action>();

        /// <summary>
        /// Конструктор класса задач.
        /// </summary>
        /// <param name="func">Функция для вычисления.</param>
        /// <param name="poolQue">Ссылка на очередь пула.</param>
        public MyTask(Func<TResult> func, Queue<Action> poolQue)
        {
            this.func = func;
            this.IsCompleted = false;

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

            poolQue.Enqueue(action);

            return nextTask;
        }

        public TResult Result
        {
            get
            {
                // TODO: Потокобезопасность
                // Добавить ожидание главного потока при вызове, когда функция
                // вычисляется в threadpool (см первую работу)
                if (!IsCompleted)
                {
                    this.result = this.func();
                    this.IsCompleted = true;

                    AddActionsToPool();

                    this.func = null;
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
