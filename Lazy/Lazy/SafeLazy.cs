using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lazy
{
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
                        }
                    }
                }

                return this.result;
            }
        }
    }
}
