using System;

namespace Lazy
{
    /// <summary>
    /// Класс SafeLazy служит для безопасной работы с ленивыми вычислениями в многопоточном режиме.
    /// </summary>
    /// <typeparam name="T">Тип результата вычисления.</typeparam>
    public class SafeLazy<T> : ILazy<T>
    {
        private Func<T> supplier;
        private T result;
        private volatile bool hasValue;

        private readonly Object lockObject = new object();

        /// <summary>
        /// Инициализирует объект класса SafeLazy. 
        /// </summary>
        /// <param name="supplier">
        /// Ленивая функция. Такая функция вызывается только один раз в момент необходимости.
        /// </param>
        public SafeLazy(Func<T> supplier)
        {
            this.supplier = supplier;
            this.hasValue = false;
        }

        /// <summary>
        /// Инициализирует и возвращает результат ленивого вычисления. 
        /// Защищено от записи и служит лишь для представления результатов вычисления.
        /// Гарантирует корректность работы в многопоточном режиме.
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
