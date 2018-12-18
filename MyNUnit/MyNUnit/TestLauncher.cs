using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyNUnit
{
    class TestLauncher
    {
        public TestLauncher(string path)
        {
            var dir = new DirectoryInfo(path);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("Каталог не найден");
            }

            var filesPath = GetAssemblyFiles(dir);

            RunAssembly(filesPath);
        }

        private string[] GetAssemblyFiles(DirectoryInfo dir)
        {
            var dllFiles = dir.GetFiles("*.dll");
            var exeFiles = dir.GetFiles("*.exe");

            string[] assemblyFiles = new string[dllFiles.Length + exeFiles.Length];

            for (int i = 0; i < dllFiles.Length; i++)
            {
                assemblyFiles[i] = dllFiles[i].ToString();
            }

            for (int i = dllFiles.Length; i < dllFiles.Length + exeFiles.Length; i++)
            {
                assemblyFiles[i] = exeFiles[i].ToString();
            }

            return assemblyFiles;
        }

        private void RunAssembly(string[] assmPath)
        {
            foreach (var path in assmPath)
            {
                RunTestFile(path);
            }
        }

        /// <summary>
        /// Обнаруживает типы исходного файла и для каждого запускает 
        /// обработчик
        /// </summary>
        /// <param name="testPath">Путь до файла</param>
        private void RunTestFile(string testPath)
        {
            var types = Assembly.LoadFile(testPath).GetExportedTypes();

            foreach (var type in types)
            {
                RunType(type);
            }
        }

        /// <summary>
        /// Обнаруживает методы типа и тестирует их
        /// </summary>
        /// <param name="type">Тип, который необходимо обработать</param>
        private void RunType(Type type)
        {

        }
    }
}
