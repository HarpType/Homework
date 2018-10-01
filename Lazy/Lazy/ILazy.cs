namespace Lazy
{
    /// <summary>
    /// Интерфейс ILazy представляет ленивые вычисления.
    /// </summary>
    /// <typeparam name="T">Тип вычисления, возвращаемый ленивой функцией.</typeparam>
    public interface ILazy<T>
    {
        T Get { get; }  
    }
}
