using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNUnit.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    class Test : Attribute
    {
        public Exception Expected { get; set; }
        public string Ignore { get; set; }
    }
}
