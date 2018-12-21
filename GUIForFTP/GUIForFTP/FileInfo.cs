﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUIForFTP
{
    public class FileInfo
    {
        public string FullName { get; set; }

        public string Name { get; set; }

        public bool IsDirectory { get; set; }

        public FileInfo(string fullName, string name, bool isDirectory)
        {
            FullName = fullName;
            Name = name;
            IsDirectory = isDirectory;
        }
    }
}
