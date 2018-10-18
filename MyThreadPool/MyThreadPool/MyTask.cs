using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyThreadPool
{
    public class MyTask<TResult> : IMyTask<TResult>
    {
        public bool IsCompleted { get; private set; }

        private TResult result;
        private Func<TResult> func;

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
                this.result = this.func();

                return this.result;
            }
        }
    }
}
