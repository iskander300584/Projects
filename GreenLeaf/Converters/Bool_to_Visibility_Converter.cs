﻿using System;
using System.Windows.Data;
using System.Windows;

namespace GreenLeaf.Converters
{
    /// <summary>
    /// Преобразование TRUE в Visible и FALSE в Collapsed 
    /// </summary>
    public class Bool_to_Visibility_Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool temp = (bool)value;

            return (temp) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}