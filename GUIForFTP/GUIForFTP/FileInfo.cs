using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUIForFTP
{
    public enum FileItemType
    {
        File,
        Directory,
        Upper
    }


    /// <summary>
    /// Класс, содержащий свойства файлов.
    /// </summary>
    public class FileInfo
    {
        public string Name { get; set; }

        public FileItemType itemType { get; set; }

        public FileInfo(string name, FileItemType itemType)
        {
            Name = name;
            this.itemType = itemType;
        }
    }
}
