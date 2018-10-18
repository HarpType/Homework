using System;

namespace MyThreadPool
{
    public class MyTask<TResult> : IMyTask<TResult>
    {
        public bool IsCompleted { get; private set; }

        private TResult result;
        private readonly Func<TResult> func;

        /// <summary>
        /// Конструктор класса задач.
        /// </summary>
        /// <param name="func">Функция для вычисления.</param>
        public MyTask(Func<TResult> func)
        {
            this.func = func;
            this.IsCompleted = false;
        }

        public TResult Result
        {
            get
            {
                // TODO: Потокобезопасность
                if (!IsCompleted)
                {
                    this.result = this.func();
                    this.IsCompleted = true;
                }

                return this.result;
            }
        }
    }
}
