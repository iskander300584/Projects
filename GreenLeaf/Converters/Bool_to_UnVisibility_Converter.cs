using System;
using System.Windows.Data;
using System.Windows;

namespace GreenLeaf.Converters
{
    /// <summary>
    /// Преобразование TRUE в Collapsed и FALSE в Visible
    /// </summary>
    public class Bool_to_UnVisibility_Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool temp = (bool)value;

            return (temp) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
