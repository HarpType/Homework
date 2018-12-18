using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyNUnit
{

    /// <summary>
    /// Класс, хронящий в себе информацию о типе тестируемого объекта
    /// </summary>
    public class TypeTestHandler
    {
        List<MethodInfo> AfterClassMethods;
        List<MethodInfo> BeforeClassMethods;

        List<MethodInfo> AfterMethods;
        List<MethodInfo> BeforeMethods;

        List<MethodInfo> TestsMethods;

        public TypeTestHandler(Type type)
        {
            foreach (var method in type.GetMethods())
            {
                foreach (var attr in method.GetCustomAttributes())
                {

                }
            }
        }
    }
}
