using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNUnit
{
    class TestConsoleInfo
    {
        static public void WriteTestInfo(List<TestInfo> testsInfo)
        {
            foreach (var info in testsInfo)
            {
                Console.WriteLine($"Test Name: {info.Path + " " + info.TypeName + " " + info.Name}");
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
