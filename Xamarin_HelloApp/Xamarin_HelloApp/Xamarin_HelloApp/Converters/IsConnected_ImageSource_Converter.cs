using System;
using System.Globalization;
using Xamarin.Forms;

namespace Xamarin_HelloApp.Converters
{
    /// <summary>
    /// Преобразование состояния подключения в пиктограмму
    /// </summary>
    public class IsConnected_ImageSource_Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool _isConnected = (bool)value;
            string fileName = (_isConnected) ? "connect.png" : "noconnect.png";

            return ImageSource.FromFile(fileName);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}