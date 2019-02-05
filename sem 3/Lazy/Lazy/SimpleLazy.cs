using System;

namespace Lazy
{
    /// <summary>
    /// Реализация ленивых вычислений. Гарантирует корректную работу только в однопоточном режиме
    /// и не гарантирует в многопоточном.
    /// </summary>
    public class SimpleLazy<T> : ILazy<T>
    {
        private Func<T> supplier;
        private T result;
        private bool hasValue;

        /// <summary>
        /// Инициализирует объект класса SimpleLazy.
        /// </summary>
        /// <param name="supplier">
        /// Ленивая функция. Такая функция вызывается только один раз в момент необходимости.
        /// </param>
        public SimpleLazy(Func<T> supplier)
        {
            this.supplier = supplier;
            this.hasValue = false;
        }

        /// <summary>
        /// Свойство Get инициализирует и возвращает результат ленивых вычислений. 
        /// Свойство защищено от записи и служит лишь для представления результатов вычисления.
        /// Не гарантирует корректную работу в многопоточном режиме.
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
