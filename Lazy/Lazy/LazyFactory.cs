using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazy
{
    public static class LazyFactory
    {
        public static SimpleLazy<T> CreateSimpleLazy<T>(Func<T> supplier)
        {
            return new SimpleLazy<T>(supplier);
        }

        public static SafeLazy<T> CreateSafeLazy<T>(Func<T> supplier)
        {
            return new SafeLazy<T>(supplier);
        }
    }
}
