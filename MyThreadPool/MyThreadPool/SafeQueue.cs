using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyThreadPool
{
    /// <summary>
    /// Реализация безопасной очереди.
    /// </summary>
    public class SafeQueue<T>
    {
        private Queue<T> queue = new Queue<T>();

        private Object lockObject = new object();

        /// <summary>
        /// Безопасное занесение элемента в очередь.
        /// </summary>
        /// <param name="el">Новый элемент для занесения в очередь.</param>
        public void Enqueue(T el)
        {
            lock (this.lockObject)
            {
                queue.Enqueue(el);
            }
        }

        /// <summary>
        /// Безопасное извлечение из очереди.
        /// </summary>
        public T Dequeue()
        {
            T el;
            lock (this.lockObject)
            {
                el = queue.Dequeue();
            }
            return el;
        }

        /// <summary>
        /// Свойство, в котором хранится размер очереди.
        /// </summary>
        public int Size
        {
            get
            {
                return queue.Count;
            }
        }
    }
}
