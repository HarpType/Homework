using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNUnit
{
    public class TestInfo
    {
        public bool Successfull { get; set; } = false;

        public long Milliseconds { get; set; } = 0;

        public Type Result { get; set; } = null;

        public bool isIgnored { get; set; } = false;

        public string IgnoreReason { get; set; } = null;

        public string Name { get; set; } = null;

        public string TypeName { get; set; } = null;

        public string Path { get; set; } = null;
    }
}
