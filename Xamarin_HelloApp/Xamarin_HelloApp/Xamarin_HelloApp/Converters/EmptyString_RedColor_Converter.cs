using System;
using System.Globalization;
using Xamarin.Forms;

namespace PilotMobile.Converters
{
    /// <summary>
    /// Возвращает красный цвет, если переданная строка пустая
    /// </summary>
    public class EmptyString_RedColor_Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = (string)value;

            return (str != null && str.Trim() == "") ? Color.Red : Color.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}