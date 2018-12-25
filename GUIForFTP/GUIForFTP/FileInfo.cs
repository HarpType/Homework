﻿using System;
using System.Collections.Generic;
using System.IO;
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
        /// <summary>
        /// Полное имя файла или директории (начинается с корня сервера).
        /// </summary>
        public string FullName { get; }

        /// <summary>
        /// Возвращает название файла или папки в том виде, в котором он будет
        /// отображаться в приложении.
        /// </summary>
        public string Name {
            get
            {
                if (itemType == FileItemType.Directory)
                    return Path.GetFileName(Path.GetDirectoryName(FullName));
                else if (itemType == FileItemType.File)
                    return Path.GetFileName(FullName);
                else
                    return "...";
            }}

        /// <summary>
        /// Тип элемента.
        /// </summary>
        public FileItemType itemType { get; }

        /// <summary>
        /// Возвращает название типа элемента.
        /// </summary>
        public string TypeName
        {
            get
            {
                if (itemType == FileItemType.Directory)
                    return "Directory";
                else if (itemType == FileItemType.File)
                    return "File";
                else
                    return "";
            }
        }

        public FileInfo(string fullName, FileItemType itemType)
        {
            FullName = fullName;
            this.itemType = itemType;
        }
    }
}
