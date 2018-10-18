using System;
using System.Collections.Generic;
using System.Threading;

namespace MyThreadPool
{
    public class MyThreadPool
    {
        private Queue<Action> que = new Queue<Action>();
        private Thread[] threads;

        /// <summary>
        /// Инициализирует и запускает фиксированное количество потоков.
        /// </summary>
        /// <param name="n">Количество запускаемых потоков.</param>
        public MyThreadPool(int n)
        {
            threads = new Thread[n];

            for (int i = 0; i < n; ++i)
            {
                threads[i] = new Thread(Run);
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
            MyTask<TResult> newTask = new MyTask<TResult>(func);

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
                Console.WriteLine("HELLO YAR");
                TResult reusult = task.Result;
            }

            return action;
        }


        /// <summary>
        /// Метод, который исполняет один из потоков.
        /// </summary>
        private void Run()
        {
            // TODO: Потокобезопасность извлечения из очереди.
            while (true)
            {
                if (que.Count != 0)
                {
                    Action action = que.Dequeue();
                    action();
                }
            }
        }
    }
}
