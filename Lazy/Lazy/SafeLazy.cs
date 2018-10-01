using System;

namespace Lazy
{
    /// <summary>
    /// Класс SafeLazy служит для безопасной работы с ленивыми вычислениями в многопоточном режиме.
    /// </summary>
    public class SafeLazy<T> : ILazy<T>
    {
        private Func<T> supplier;
        private T result;
        private volatile bool hasValue;

        private Object lockObject = new object();

        public SafeLazy(Func<T> supplier)
        {
            this.supplier = supplier;
            this.hasValue = false;
        }
        
        /// <summary>
        /// Инициализирует и возвращает результат ленивого вычисления.
        /// </summary>
        public T Get
        {
            get
            {

                if (!this.hasValue)
                {
                    lock (this.lockObject)
                    {
                        if (!this.hasValue)
                        {
                            this.result = this.supplier();
                            this.hasValue = true;

                            this.supplier = null;
                        }
                    }
                }

                return this.result;
            }
        }
    }
}
