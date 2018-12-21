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

        Type type;

        /// <summary>
        /// Конструктор класса. Создаёт объект типа и распределяет его методы 
        /// по массивам в зависимости от значений аттрибутов.
        /// </summary>
        /// <param name="type">Исходный тип.</param>
        public TypeTestHandler(Type type)
        {
            this.type = type;

            foreach (var method in type.GetMethods())
            {
                foreach (var attr in method.GetCustomAttributes())
                {
                    
                    var attrType = attr.GetType();

                    if (attrType == typeof(Attributes.AfterClassAttribute))
                    {
                        if (method.IsStatic)
                        {
                            var testMethod = new MethodTestHandler(method);
                            afterClassMethods.Add(testMethod);

                            break;
                        }
                    }
                    else if (attrType == typeof(Attributes.BeforeClassAttribute))
                    {
                        if (method.IsStatic)
                        {
                            var testMethod = new MethodTestHandler(method);
                            beforeClassMethods.Add(testMethod);

                            break;
                        }
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
            List<TestInfo> beforeClassInfo = RunBeforeClassInTasks(beforeClassMethods);
            bool beforeClassFailed = beforeClassInfo.Exists(IsNotSuccessfull);

            List<TestInfo> mainInfo = RunTests(beforeMethods, afterMethods, testsMethods, beforeClassFailed);
            bool mainFailed = mainInfo.Exists(IsNotSuccessfull);

            List<TestInfo> afterClassInfo = RunAfterClassInTasks(afterClassMethods, mainFailed);

            List<TestInfo> testsInfo = beforeClassInfo;
            testsInfo.AddRange(mainInfo);
            testsInfo.AddRange(afterClassInfo);

            return testsInfo;
        }

        /// <summary>
        /// Предикат для поиска неуспешно выполненных тестов.
        /// </summary>
        /// <param name="testInfo">Информация о тесте.</param>
        /// <returns>True, если тест выполнен успешно, false в противном случае.</returns>
        private bool IsNotSuccessfull(TestInfo testInfo) => !testInfo.Successfull ? true : false;

        /// <summary>
        /// Независимо запускает тестирование для заданного листа статических методов 
        /// с атрибутом BeforeClass.
        /// </summary>
        /// <param name="methods">Набор статических методов, которые необходимо протестировать.</param>
        /// <returns>Информация о выполненных тестах.</returns>        
        private List<TestInfo> RunBeforeClassInTasks(List<MethodTestHandler> methods)
        {
            Task[] tasks = new Task[methods.Count];

            for (int i = 0; i < methods.Count; ++i)
            {
                int j = i;
                tasks[i] = new Task<TestInfo>(() => methods[j].RunStatic(type, false));
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
        /// Независимо запускает тестирование для заданного листа статических методов
        /// с атрибутом AfterClass.
        /// </summary>
        /// <param name="methods">Искомые методы.</param>
        /// <param name="beforeFailed">Информация о успешности предыдущих тестов.</param>
        /// <returns>Информация о тестах.</returns>
        private List<TestInfo> RunAfterClassInTasks(List<MethodTestHandler> methods, bool beforeFailed)
        {
            Task[] tasks = new Task[methods.Count];

            for (int i = 0; i < methods.Count; ++i)
            {
                int j = i;
                tasks[i] = new Task<TestInfo>(() => methods[j].RunStatic(type, beforeFailed));
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
        /// Для каждого основного теста test запускает before и after тесты и его самого.
        /// </summary>
        /// <param name="beforeMethods">Тесты, которые необходимо выполнять перед каждым основным тестом.</param>
        /// <param name="afterMethods">Тесты, которые необходимо выполнять после каждого основного теста</param>
        /// <param name="testMethods">Набор основных тестов.</param>
        /// <param name="beforeClassFailed">Информация о успешности выполнения предыдущих тестов.</param>
        /// <returns>Информация о выполненных тестах.</returns>
        private List<TestInfo> RunTests(List<MethodTestHandler> beforeMethods,
            List<MethodTestHandler> afterMethods,
            List<MethodTestHandler> testMethods,
            bool beforeClassFailed)
        {
            Task[] testTasks = new Task[testMethods.Count];

            for (int i = 0; i < testMethods.Count; ++i)
            {
                var localInstance = Activator.CreateInstance(type);
                int j = i;
                testTasks[i] = new Task<List<TestInfo>>(() =>
                {
                    List<TestInfo> beforeInfo = RunBeforeAfter(beforeMethods, localInstance, beforeClassFailed);
                    bool beforeFailed = beforeInfo.Exists(IsNotSuccessfull);

                    TestInfo mainInfo = testMethods[j].Run(localInstance, beforeFailed);
                    bool mainFailed = IsNotSuccessfull(mainInfo);

                    List<TestInfo> afterInfo = RunBeforeAfter(afterMethods, localInstance, mainFailed);

                    var localTestsInfo = beforeInfo;
                    localTestsInfo.Add(mainInfo);
                    localTestsInfo.AddRange(afterInfo);

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

        /// <summary>
        /// Запускает заданный набор методов друг за другом.
        /// </summary>
        /// <param name="methods">Исходные методы, которые необходимо протестировать.</param>
        /// <param name="instance">Объект типа, в котором находится тест.</param>
        /// <param name="beforeClassFailed">Основные тесты считаются неуспешными, если 
        /// beforeClass тесты неуспешны.</param>
        /// <returns>Информация о тестах.</returns>
        private List<TestInfo> RunBeforeAfter(List<MethodTestHandler> methods, 
            object instance, 
            bool beforeFailed)
        {
            var testsInfo = new List<TestInfo>();

            foreach (var method in methods)
            {
                testsInfo.Add(method.Run(instance, beforeFailed));
            }

            return testsInfo;
        }
    }
}
