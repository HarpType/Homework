using System;

namespace Lazy
{
    /// <summary>
    /// Класс-создатель объектов ленивых вычислений.
    /// </summary>
    public static class LazyFactory
    {
        public static SimpleLazy<T> CreateSimpleLazy<T>(Func<T> supplier)
            => new SimpleLazy<T>(supplier);

        public static SafeLazy<T> CreateSafeLazy<T>(Func<T> supplier)
            => new SafeLazy<T>(supplier);
    }
}
