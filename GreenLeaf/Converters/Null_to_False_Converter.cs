using System;
using System.Windows.Data;

namespace GreenLeaf.Converters
{
    /// <summary>
    /// Преобразование NULL в FALSE
    /// </summary>
    public class Null_to_False_Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool? temp = (bool?)value;

            if (temp != null)
                return (bool)temp;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}