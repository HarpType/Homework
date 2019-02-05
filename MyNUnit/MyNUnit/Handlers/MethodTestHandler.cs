using System;
using System.Diagnostics;
using System.Reflection;

namespace MyNUnit.Handlers
{
    /// <summary>
    /// Класс-обработчик тестовых методов.
    /// </summary>
    class MethodTestHandler
    {
        /// <summary>
        /// Содержит информацию о методе.
        /// </summary>
        public MethodInfo Info { get; }

        /// <summary>
        /// Содержит информацию о аттрибуте, если таковая имеется.
        /// </summary>
        public Attributes.TestAttribute TestAttribute { get; set; } = null;

        /// <summary>
        /// Конструктор класса. 
        /// </summary>
        /// <param name="method">Метод, для которого необходимо запустить тестирование</param>
        public MethodTestHandler(MethodInfo method)
        {
            Info = method;            
        }

        /// <summary>
        /// Запускает тестирование статического метода.
        /// </summary>
        /// <param name="type">Тип, которому принадлжит метод.</param>
        /// <param name="beforeFailed">Инфомрация о ходе выполнения предыдущего теста.
        /// Если true, то текущий тест должен быть неуспешным, если false, то
        /// запускает тестирование.</param>
        /// <returns>Информация о тесте.</returns>
        public TestInfo RunStatic(Type type, bool beforeFailed)
        {
            var testInfo = new TestInfo();

            testInfo.Name = Info.Name;
            testInfo.TypeName = type.GetType().FullName;

            if (beforeFailed)
            {
                testInfo.Successfull = false;

                return testInfo;
            }

            var stopwatch = Stopwatch.StartNew();
            try
            {
                Info.Invoke(null, null);

                testInfo.Successfull = true;
            }
            catch (TargetInvocationException ex)
            {
                stopwatch.Stop();

                var item = ex.InnerException;

                testInfo.Successfull = false;
                testInfo.Result = item.GetType();
            }
            finally
            {
                stopwatch.Stop();
                testInfo.Milliseconds = stopwatch.ElapsedMilliseconds;
            }

            return testInfo;
        }

        /// <summary>
        /// Запускает тестирование метода.
        /// </summary>
        /// <param name="instance">Объект, которому принадлежит метод.</param>
        /// <returns>Информация о тесте.</returns>
        public TestInfo Run(object instance, bool beforeFailed)
        {
            var testInfo = new TestInfo();

            testInfo.Name = Info.Name;
            testInfo.TypeName = instance.GetType().FullName;

            if (beforeFailed)
            {
                testInfo.Successfull = false;

                return testInfo;
            }


            if (TestAttribute != null)
            {
                if (TestAttribute.Ignore != null)
                {
                    testInfo.isIgnored = true;
                    testInfo.IgnoreReason = TestAttribute.Ignore;
                    return testInfo;
                }
            }

            var stopwatch = Stopwatch.StartNew();
            try
            {
                Info.Invoke(instance, null);

                testInfo.Successfull = true;

                if (TestAttribute != null)
                    if (TestAttribute.Expected != null)
                        testInfo.Successfull = false;
            }
            catch (TargetInvocationException ex)
            {
                stopwatch.Stop();

                var item = ex.InnerException;
                if (TestAttribute != null)
                {
                    if (TestAttribute.Expected == item.GetType())
                    {
                        testInfo.Successfull = true;
                    }
                }
                else
                {
                    testInfo.Successfull = false;
                }

                testInfo.Result = item.GetType();
            }
            finally
            {
                stopwatch.Stop();
                testInfo.Milliseconds = stopwatch.ElapsedMilliseconds;
            }

            return testInfo;
        }
    }
}
