using System.Reflection;

namespace MyNUnit.Handlers
{
    /// <summary>
    /// Класс-обработчик тестовых методов.
    /// </summary>
    class MethodTestHandler
    {
        public MethodInfo Info { get; }

        public MethodTestHandler(MethodInfo method)
        {
            Info = method;
        }

        public void Run(object instance)
        {

        }
    }
}
