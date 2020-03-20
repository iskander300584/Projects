using System;
using System.Windows.Data;

namespace GreenLeaf.Converters
{
    /// <summary>
    /// Преобразование "TRUE" в "1", "FALSE" в "0"
    /// </summary>
    public class Bool_to_Digit_Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool temp = (bool)value;

            return (temp) ? 1 : 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int temp = (int)value;

            return (temp == 1);
        }
    }
}