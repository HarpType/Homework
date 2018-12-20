using System;
using System.Collections.Generic;

namespace MyThreadPool
{
    /// <summary>
    /// Реализация безопасной очереди с минимальными возможностями.
    /// </summary>
    public class SafeQueue<T>
    {
        private Queue<T> queue = new Queue<T>();

        private readonly object lockObject = new object();

        /// <summary>
        /// Безопасное занесение элемента в очередь.
        /// </summary>
        /// <param name="el">Новый элемент для занесения в очередь.</param>
        public void Enqueue(T el)
        {
            lock (lockObject)
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
            lock (lockObject)
            {
                return queue.Dequeue();
            }
        }

        /// <summary>
        /// Свойство, в котором хранится размер очереди.
        /// </summary>
        public int Size => queue.Count;
    }
}
