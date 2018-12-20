using System;

namespace MyNUnit
{
    /// <summary>
    /// Информация о тесте.
    /// </summary>
    public class TestInfo
    {
        public bool Successfull { get; set; } = false;

        /// <summary>
        /// Время выполнения теста.
        /// </summary>
        public long Milliseconds { get; set; } = 0;

        /// <summary>
        /// Тип исключения при выполнении теста.
        /// </summary>
        public Type Result { get; set; } = null;

        public bool isIgnored { get; set; } = false;

        public string IgnoreReason { get; set; } = null;
        
        /// <summary>
        /// Название метода.
        /// </summary>
        public string Name { get; set; } = null;

        /// <summary>
        /// Название Типа, для которого исполняется тест.
        /// </summary>
        public string TypeName { get; set; } = null;

        /// <summary>
        /// Название файла, в котором исполняется тест.
        /// </summary>
        public string FileName { get; set; } = null;
    }
}
