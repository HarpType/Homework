using System;

namespace Lazy
{
    /// <summary>
    /// Класс-создатель объектов ленивых вычислений.
    /// </summary>
    public static class LazyFactory
    {
        /// <summary>
        /// Создаёт объект класса ленивых вычислений, не гарантирующий корректную работу  
        /// в многопоточном режиме.
        /// </summary>
        /// <typeparam name="T">Тип возвращаемого ленивой функцией объекта.</typeparam>
        /// <param name="supplier">Ленивая функция.</param>
        /// <returns>Объект класса ленивых вычислений.</returns>
        public static SimpleLazy<T> CreateSimpleLazy<T>(Func<T> supplier)
            => new SimpleLazy<T>(supplier);

        /// <summary>
        /// Создаёт объект класса ленивых вычислений, гарантирующий корректную работу 
        /// в многопоточном режиме.
        /// </summary>
        /// <typeparam name="T">Тип возвращаемого ленивой функцией объекта. </typeparam>
        /// <param name="supplier">Ленивая функция.</param>
        /// <returns>Объект класса ленивых вычислений.</returns>
        public static SafeLazy<T> CreateSafeLazy<T>(Func<T> supplier)
            => new SafeLazy<T>(supplier);
    }
}
