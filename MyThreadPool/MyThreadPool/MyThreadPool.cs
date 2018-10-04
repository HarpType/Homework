using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyThreadPool
{
    public class MyThreadPool
    {
        Queue<Object> que;

        public MyThreadPool()
        {
            // начинают работу n потоков
        }

        public MyTask<TResult> AddTask<TResult>(Func<TResult> func)
        {
            MyTask<TResult> myTask = new MyTask<TResult>();
            que.Enqueue(myTask);
            return myTask;
        }
    }
}
