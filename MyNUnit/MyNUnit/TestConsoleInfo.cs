using System;
using System.Collections.Generic;

namespace MyNUnit
{
    /// <summary>
    /// Класс, предоставляющий интерфейс для вывода информации о тестах на экран.
    /// </summary>
    public class TestConsoleInfo
    {
        /// <summary>
        /// Метод выводит информацию о тестах на экран.
        /// </summary>
        /// <param name="testsInfo">Информация о тестах.</param>
        static public void WriteTestInfo(List<TestInfo> testsInfo)
        {
            foreach (var info in testsInfo)
            {
                Console.WriteLine($"Test Name: {info.FileName + " " + info.TypeName + " " + info.Name}");
                if (info.Successfull)
                {
                    Console.WriteLine("Test has been completed successfully");

                    if (info.Result != null)
                    {
                        Console.WriteLine($"With exception: {info.Result}");
                    }

                    Console.WriteLine($"Test time: {info.Milliseconds}");
                }
                else
                {
                    if (info.isIgnored)
                    {
                        Console.WriteLine($"Test has been ignored. Reason: {info.IgnoreReason}");
                    }
                    else
                    {
                        Console.WriteLine("Test has not been completed successfully");
                        if (info.Result != null)
                        {
                            Console.WriteLine($"With exception: {info.Result}");
                        }
                    }
                }

                Console.WriteLine();
            }
        }
    }
}
