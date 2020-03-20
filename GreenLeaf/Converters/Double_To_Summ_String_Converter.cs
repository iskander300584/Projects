using System;
using System.Windows.Data;

namespace GreenLeaf.Converters
{
    /// <summary>
    /// Преобразование дробного значения в сумму с 2 знаками после запятой
    /// </summary>
    public class Double_To_Summ_String_Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double temp = (double)value;
            string tempS = String.Format("{0:.##}", temp).Replace(',', '.');

            if (tempS == "")
                tempS = "0.00";
            else if(!tempS.Contains("."))
            {
                tempS += ".00";
            }
            else
            {
                int index = tempS.LastIndexOf('.');
                if (index == tempS.Length - 2)
                    tempS += "0";
            }

            return tempS;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}