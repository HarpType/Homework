using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyNUnit.Handlers
{

    /// <summary>
    /// Класс, хронящий в себе информацию о типе тестируемого объекта
    /// </summary>
    public class TypeTestHandler
    {
        private List<MethodTestHandler> afterClassMethods = new List<MethodTestHandler>();
        private List<MethodTestHandler> beforeClassMethods = new List<MethodTestHandler>();

        private List<MethodTestHandler> afterMethods = new List<MethodTestHandler>();
        private List<MethodTestHandler> beforeMethods = new List<MethodTestHandler>();

        private List<MethodTestHandler> testsMethods = new List<MethodTestHandler>();

        private object instance;

        /// <summary>
        /// Конструктор класса. Создаёт объект типа и распределяет его методы 
        /// по массивам в зависимости от значений аттрибутов.
        /// </summary>
        /// <param name="type">Исходный тип.</param>
        public TypeTestHandler(Type type)
        {
            instance = Activator.CreateInstance(type);

            foreach (var method in type.GetMethods())
            {
                var testMethod = new MethodTestHandler(method);
                foreach (var attr in method.GetCustomAttributes())
                {
                    var attrType = attr.GetType();

                    if (attrType == typeof(Attributes.AfterClassAttribute))
                    {
                        afterClassMethods.Add(testMethod);
                    }
                    else if (attrType == typeof(Attributes.BeforeClassAttribute))
                    {
                        beforeClassMethods.Add(testMethod);
                    }
                    else if (attrType == typeof(Attributes.AfterAttribute))
                    {
                        afterMethods.Add(testMethod);
                    }
                    else if (attrType == typeof(Attributes.BeforeAttribute))
                    {
                        beforeMethods.Add(testMethod);
                    }
                    else if (attrType == typeof(Attributes.TestAttribute))
                    {
                        testMethod.TestAttribute = (Attributes.TestAttribute)attr;
                        testsMethods.Add(testMethod);
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// По порядку запускает тестовые методы.
        /// </summary>
        public List<TestInfo> RunTests()
        {
            var testsInfo = new List<TestInfo>();

            testsInfo.AddRange(RunInTasks(beforeClassMethods));

            testsInfo.AddRange(RunWithInTasks(beforeMethods, afterMethods, testsMethods));

            testsInfo.AddRange(RunInTasks(afterClassMethods));

            return testsInfo;
        }

        /// <summary>
        /// Запускает лист методов.
        /// </summary>
        /// <param name="methods"></param>
        private List<TestInfo> RunInTasks(List<MethodTestHandler> methods)
        {
            Task[] tasks = new Task[methods.Count];

            for (int i = 0; i < methods.Count; ++i)
            {
                int j = i;
                tasks[i] = new Task<TestInfo>(methods[j].Run, instance);
                tasks[i].Start();
            }

            var testsInfo = new List<TestInfo>();

            foreach (Task<TestInfo> task in tasks)
            {
                testsInfo.Add(task.Result);
            }

            return testsInfo;
        }

        private List<TestInfo> RunWithInTasks(List<MethodTestHandler> beforeMethods,
            List<MethodTestHandler> afterMethods,
            List<MethodTestHandler> testMethods)
        {
            Task[] testTasks = new Task[testMethods.Count];

            var globalTestsInfo = new List<TestInfo>();

            for (int i = 0; i < testMethods.Count; ++i)
            {
                int j = i;
                testTasks[i] = new Task<List<TestInfo>>(() =>
                {
                    var localTestsInfo = new List<TestInfo>();

                    localTestsInfo.AddRange(RunInTasks(beforeMethods));

                    localTestsInfo.Add(testMethods[j].Run(instance));

                    localTestsInfo.AddRange(RunInTasks(afterMethods));

                    return localTestsInfo;
                });

                testTasks[i].Start();
            }

            foreach (Task<List<TestInfo>> task in testTasks)
            {
                globalTestsInfo.AddRange(task.Result);
            }

            return globalTestsInfo;
        }
    }
}
