using System;
using System.Windows.Data;

namespace GreenLeaf.Converters
{
    /// <summary>
    /// Преобразование даты в строку
    /// </summary>
    public class Date_To_String_Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTime temp = (DateTime)value;

            string tempS = (temp != DateTime.MinValue) ? temp.ToShortDateString() : "--.--.----";

            return tempS;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
