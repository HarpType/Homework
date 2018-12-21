using System;

namespace MyNUnit.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class TestAttribute : Attribute
    {
        /// <summary>
        /// Аннотация хранит тип исключения, который ожидается тестом.
        /// </summary>
        public Type Expected { get; set; }

        /// <summary>
        /// Аннотация хранит причину отмены запуска теста.
        /// </summary>
        public string Ignore { get; set; }
    }
}
