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
        private List<MethodTestHandler> afterClassMethods;
        private List<MethodTestHandler> beforeClassMethods;

        private List<MethodTestHandler> afterMethods;
        private List<MethodTestHandler> beforeMethods;

        private List<MethodTestHandler> testsMethods;

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
                foreach (var attr in testMethod.Info.GetCustomAttributes())
                {
                    var attrType = attr.GetType();

                    if (attrType == typeof(Attributes.AfterClass))
                    {
                        afterClassMethods.Add(testMethod);
                    }
                    else if (attrType == typeof(Attributes.BeforeClass))
                    {
                        beforeClassMethods.Add(testMethod);
                    }
                    else if (attrType == typeof(Attributes.After))
                    {
                        afterMethods.Add(testMethod);
                    }
                    else if (attrType == typeof(Attributes.Before))
                    {
                        beforeMethods.Add(testMethod);
                    }
                    else if (attrType == typeof(Attributes.Test))
                    {
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
        public void RunTests()
        {
            RunMethods(afterClassMethods);

            RunMethods(afterMethods);

            RunMethods(testsMethods);

            RunMethods(beforeMethods);

            RunMethods(beforeClassMethods);
        }

        /// <summary>
        /// Запускает лист методов.
        /// </summary>
        /// <param name="methods"></param>
        private void RunMethods(List<MethodTestHandler> methods)
        {
            Task[] tasks = new Task[methods.Count];

            for (int i = 0; i < methods.Count; i++)
            {
                tasks[i] = new Task(() => afterClassMethods[i].Run(instance));
                tasks[i].Start();
            }

            Task.WaitAll(tasks);
        }
    }
}
