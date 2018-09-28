using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazy
{
    public class SimpleLazy<T> : ILazy<T>
    {
        private Func<T> supplier;
        private T result;
        private bool hasValue;

        // Constructor
        public SimpleLazy(Func<T> supplier)
        {
            this.supplier = supplier;
            this.hasValue = false;
        }

        public T Get
        {
            get
            {
                if (!hasValue)
                {
                    this.result = this.supplier();
                    this.hasValue = true;
                }

                return this.result;
            }
        }
    }
}
