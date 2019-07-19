using System;
using System.Windows.Data;

namespace GreenLeaf.Converters
{
    /// <summary>
    /// Преобразование "TRUE" в "Да", "FALSE" в "Нет"
    /// </summary>
    public class Bool_to_YesNo_Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool temp = (bool)value;

            return (temp) ? "Да" : "Нет";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
