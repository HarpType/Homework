using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyThreadPool
{
    /// <summary>
    /// This interface describes properties and methods of tasks used by thread pool.
    /// </summary>
    /// <typeparam name="TResult">Type of tasks result.</typeparam>
    interface IMyTask<TResult>
    {
        bool IsCompleted { get; }
        TResult Result { get; }
    }
}
