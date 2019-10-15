using System;
using System.Windows.Data;

namespace GreenLeaf.Converters
{
    /// <summary>
    /// Преобразование номера накладной в строку
    /// </summary>
    public class Invoice_Number_To_String_Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int temp = (int)value;

            return (temp != 0) ? temp.ToString() : "-";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
