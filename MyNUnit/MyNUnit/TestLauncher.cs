using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace MyNUnit
{
    /// <summary>
    /// Класс, запускающий сеанс тестирования.
    /// </summary>
    public class TestLauncher
    {
        /// <summary>
        /// Запускает тестирование.
        /// </summary>
        /// <param name="path">Путь до директории, в которой находятся сборки.</param>
        /// <returns>Лист с информацией о каждом выполненном тесте.</returns>
        static public List<TestInfo> Launch(string path)
        {
            if (path == null)
                throw new ArgumentNullException();

            var dir = new DirectoryInfo(path);
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("Каталог не найден");
            }

            FileInfo[] assmFiles= GetAssemblyFiles(dir);

            List<TestInfo> testsInfo = RunAssembly(assmFiles);

            return testsInfo;
        }

        /// <summary>
        /// Находит и возвращает файлы сборок в заданной директории.
        /// </summary>
        /// <param name="dir">Директория, в которой необходимо выполнить поиск.</param>
        /// <returns>Массив названий сборок.</returns>
        static private FileInfo[] GetAssemblyFiles(DirectoryInfo dir) 
            => dir.GetFiles("*.dll");

        /// <summary>
        /// Запускает процесс тестирования для разных файлов сборок в разных потоках.
        /// </summary>
        /// <param name="assmFiles">Сборки, в которых необходимо запустить тестирование.</param>
        /// <returns>Информация о каждом выполненном тесте в каждой сборке.</returns>
        static private List<TestInfo> RunAssembly(FileInfo[] assmFiles)
        {
            var tasks = new Task<List<TestInfo>>[assmFiles.Length];
            for (int i = 0; i < assmFiles.Length; ++i)
            {
                int j = i;
                tasks[i] = new Task<List<TestInfo>>(() => { return RunTestFile(assmFiles[j]); });
                tasks[i].Start();
            }

            var assemblyTestsInfo = new List<TestInfo>();
            foreach (var task in tasks)
            {
                assemblyTestsInfo.AddRange(task.Result);
            }

            return assemblyTestsInfo;
        }

        /// <summary>
        /// Обнаруживает типы исходного файла и для каждого запускает 
        /// процесс тестирования в новом потоке.
        /// </summary>
        /// <param name="testPath">Файл сборки.</param>
        static private List<TestInfo> RunTestFile(FileInfo fileName)
        {
            var types = Assembly.LoadFile(fileName.FullName).GetExportedTypes();

            var tasks = new Task<List<TestInfo>>[types.Length];
            for (int i = 0; i < types.Length; ++i)
            {
                int j = i;
                tasks[i] = new Task<List<TestInfo>>(() => { return RunType(types[j]); });
                tasks[i].Start();
            }

            var typesTestInfo = new List<TestInfo>();
            foreach (var type in types)
            {
                typesTestInfo.AddRange(RunType(type));
            }

            typesTestInfo.ForEach((test) => test.FileName = fileName.Name);

            return typesTestInfo;
        }

        /// <summary>
        /// Обнаруживает методы типа и для каждого запускает процесс тестирования.
        /// </summary>
        /// <param name="type">Тип, который необходимо обработать.</param>
        static private List<TestInfo> RunType(Type type)
        {
            var typeHandler = new Handlers.TypeTestHandler(type);

            var testsInfo = new List<TestInfo>();
            testsInfo = typeHandler.RunTests();

            return testsInfo;
        }
    }
}
