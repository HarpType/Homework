using System;
using System.Windows.Data;

namespace GUIForFTP
{
    /// <summary>
    /// Класс-конвертер типа-перечисления в строку.
    /// </summary>
    public class EnumToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string enumString;
            try
            {
                enumString = Enum.GetName((value.GetType()), value);
                return enumString;
            }
            catch
            {
                return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
