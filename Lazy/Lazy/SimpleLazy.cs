using System;

namespace Lazy
{
    /// <summary>
    /// Реализация ленивых вычислений. Гарантирует корректную работу только в однопоточном режиме.
    /// </summary>
    public class SimpleLazy<T> : ILazy<T>
    {
        private Func<T> supplier;
        private T result;
        private bool hasValue;

        public SimpleLazy(Func<T> supplier)
        {
            this.supplier = supplier;
            this.hasValue = false;
        }

        /// <summary>
        /// Свойство Get инициализирует и возвращает результат ленивых вычислений.
        /// </summary>
        public T Get
        {
            get
            {
                if (!hasValue)
                {
                    this.result = this.supplier();
                    this.hasValue = true;

                    this.supplier = null;
                }

                return this.result;
            }
        }
    }
}
