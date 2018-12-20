using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyNUnit
{
    public class TestLauncher
    {
        static public List<TestInfo> Launch(string path)
        {
            var dir = new DirectoryInfo(path);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("Каталог не найден");
            }

            var filesPath = GetAssemblyFiles(dir);

            List<TestInfo> testsInfo = RunAssembly(filesPath);

            return testsInfo;
        }

        static private string[] GetAssemblyFiles(DirectoryInfo dir)
        {
            var dllFiles = dir.GetFiles("*.dll");
            //var exeFiles = dir.GetFiles("*.exe");

            string[] assemblyFiles = new string[dllFiles.Length];

            for (int i = 0; i < dllFiles.Length; i++)
            {
                assemblyFiles[i] = dllFiles[i].FullName;
            }

            //for (int i = dllFiles.Length; i < dllFiles.Length + exeFiles.Length; i++)
            //{
            //    assemblyFiles[i] = exeFiles[i].ToString();
            //}

            return assemblyFiles;
        }

        static private List<TestInfo> RunAssembly(string[] assmPath)
        {
            var assemblyTestsInfo = new List<TestInfo>();

            foreach (var path in assmPath)
            {
                assemblyTestsInfo.AddRange(RunTestFile(path));
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

            foreach (var type in types)
            {
                typesTestInfo.AddRange(RunType(type));
            }

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
