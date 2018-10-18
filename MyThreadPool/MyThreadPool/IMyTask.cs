using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyThreadPool
{
    /// <summary>
    /// Представляет задачу, принятую к исполнению потоком.
    /// </summary>
    /// <typeparam name="TResult">Тип, возвращаемый задачей.</typeparam>
    interface IMyTask<TResult>
    {
        bool IsCompleted { get; }
        TResult Result { get; }
    }
}
