using System;

namespace MyThreadPool
{
    /// <summary>
    /// Интерфейс описывает структуру для управления задачей.
    /// </summary>
    /// <typeparam name="TResult">Тип, возвращаемый задачей.</typeparam>
    interface IMyTask<TResult>
    {
        /// <summary>
        /// Возвращает true в случае, если задача выполнена.
        /// </summary>
        bool IsCompleted { get; }

        /// <summary>
        /// Свойство предоставляет результат вычисления задачи.
        /// </summary>
        TResult Result { get; }

        /// <summary>
        /// Формирует и возвращает новую задачу, решение которой опирается на результатах текущей задачи.
        /// </summary>
        /// <typeparam name="TNewResult">Тип, возвращаемый новой задачей</typeparam>
        /// <param name="func">Функция, описывающая задачу.</param>
        MyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> func);
    }
}
