using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace MyNUnit.Handlers
{

    /// <summary>
    /// Класс, хронящий в себе информацию о типе тестируемого объекта.
    /// </summary>
    public class TypeTestHandler
    {
        /// <summary>
        /// Листы, которые хранят методы в зависимости от значений аттрибутов.
        /// </summary>
        private List<MethodTestHandler> afterClassMethods = new List<MethodTestHandler>();
        private List<MethodTestHandler> beforeClassMethods = new List<MethodTestHandler>();

        private List<MethodTestHandler> afterMethods = new List<MethodTestHandler>();
        private List<MethodTestHandler> beforeMethods = new List<MethodTestHandler>();

        private List<MethodTestHandler> testsMethods = new List<MethodTestHandler>();

        private readonly object instance;

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
                foreach (var attr in method.GetCustomAttributes())
                {
                    var attrType = attr.GetType();

                    if (attrType == typeof(Attributes.AfterClassAttribute))
                    {
                        var testMethod = new MethodTestHandler(method);
                        afterClassMethods.Add(testMethod);

                        break;
                    }
                    else if (attrType == typeof(Attributes.BeforeClassAttribute))
                    {
                        var testMethod = new MethodTestHandler(method);
                        beforeClassMethods.Add(testMethod);

                        break;
                    }
                    else if (attrType == typeof(Attributes.AfterAttribute))
                    {
                        var testMethod = new MethodTestHandler(method);
                        afterMethods.Add(testMethod);

                        break;
                    }
                    else if (attrType == typeof(Attributes.BeforeAttribute))
                    {
                        var testMethod = new MethodTestHandler(method);
                        beforeMethods.Add(testMethod);

                        break;
                    }
                    else if (attrType == typeof(Attributes.TestAttribute))
                    {
                        var testMethod = new MethodTestHandler(method);
                        testMethod.TestAttribute = (Attributes.TestAttribute)attr;
                        testsMethods.Add(testMethod);

                        break;
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
        /// Независимо запускает заданный лист методов.
        /// </summary>
        /// <param name="methods">Набор методов, которые необходимо протестировать.</param>
        /// <returns>Информация о выполненных тестах.</returns>        
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

        /// <summary>
        /// Для каждого теста запускает набор тестов, которые необходимо 
        /// выполнять до и после.
        /// </summary>
        /// <param name="beforeMethods">Тесты, которые необходимо выполнять перед каждым основным тестом.</param>
        /// <param name="afterMethods">Тесты, которые необходимо выполнять после каждого основного теста</param>
        /// <param name="testMethods">Набор основных тестов.</param>
        /// <returns>Информация о выполненных тестах.</returns>
        private List<TestInfo> RunWithInTasks(List<MethodTestHandler> beforeMethods,
            List<MethodTestHandler> afterMethods,
            List<MethodTestHandler> testMethods)
        {
            Task[] testTasks = new Task[testMethods.Count];

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

            var globalTestsInfo = new List<TestInfo>();
            foreach (Task<List<TestInfo>> task in testTasks)
            {
                globalTestsInfo.AddRange(task.Result);
            }

            return globalTestsInfo;
        }
    }
}
