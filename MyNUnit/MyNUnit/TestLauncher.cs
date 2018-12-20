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

            var filesPath = GetAssemblyFiles(dir);

            List<TestInfo> testsInfo = RunAssembly(filesPath);

            return testsInfo;
        }

        /// <summary>
        /// Находит и возвращает файлы сборок в заданной директории.
        /// </summary>
        /// <param name="dir">Директория, в которой необходимо выполнить поиск.</param>
        /// <returns>Массив названий сборок.</returns>
        static private string[] GetAssemblyFiles(DirectoryInfo dir)
        {
            var dllFiles = dir.GetFiles("*.dll");

            string[] assemblyFiles = new string[dllFiles.Length];

            for (int i = 0; i < dllFiles.Length; i++)
            {
                assemblyFiles[i] = dllFiles[i].FullName;
            }

            return assemblyFiles;
        }

        static private List<TestInfo> RunAssembly(string[] assmPath)
        {
            var assemblyTestsInfo = new List<TestInfo>();

            var tasks = new Task<List<TestInfo>>[assmPath.Length];

            for (int i = 0; i < assmPath.Length; ++i)
            {
                int j = i;
                tasks[i] = new Task<List<TestInfo>>(() => { return RunTestFile(assmPath[j]); });
                tasks[i].Start();
            }

            foreach (var task in tasks)
            {
                assemblyTestsInfo.AddRange(task.Result);
            }

            return assemblyTestsInfo;
        }

        /// <summary>
        /// Обнаруживает типы исходного файла и для каждого запускает 
        /// обработчик
        /// </summary>
        /// <param name="testPath">Путь до файла</param>
        static private List<TestInfo> RunTestFile(string testPath)
        {
            var types = Assembly.LoadFile(testPath).GetExportedTypes();

            var typesTestInfo = new List<TestInfo>();

            var tasks = new Task<List<TestInfo>>[types.Length];

            for (int i = 0; i < types.Length; ++i)
            {
                int j = i;
                tasks[i] = new Task<List<TestInfo>>(() => { return RunType(types[j]); });
                tasks[i].Start();
            }

            foreach (var type in types)
            {
                typesTestInfo.AddRange(RunType(type));
            }

            typesTestInfo.ForEach((test) => test.Path = testPath);

            return typesTestInfo;
        }

        /// <summary>
        /// Обнаруживает методы типа и запускает процесс тестирования
        /// </summary>
        /// <param name="type">Тип, который необходимо обработать</param>
        static private List<TestInfo> RunType(Type type)
        {
            var testsInfo = new List<TestInfo>();

            var typeHandler = new Handlers.TypeTestHandler(type);

            testsInfo = typeHandler.RunTests();

            return testsInfo;
        }
    }
}
