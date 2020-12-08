using System;
using System.Globalization;
using Xamarin.Forms;

namespace Xamarin_HelloApp.Converters
{
    /// <summary>
    /// Инвертирование значения BOOL
    /// </summary>
    public class BooleanReverse_Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}