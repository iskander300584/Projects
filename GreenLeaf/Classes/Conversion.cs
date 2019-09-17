using System;

namespace GreenLeaf.Classes
{
    /// <summary>
    /// Преобразование данных
    /// </summary>
    public static class Conversion
    {
        /// <summary>
        /// Преобразование в строку
        /// </summary>
        /// <param name="value">дробное значение</param>
        /// <returns>возвращает дробное число, преобразованное в строку, допустимую для записи в БД</returns>
        public static string ToString(double value)
        {
            return value.ToString().Replace(',', '.');
        }

        /// <summary>
        /// Преобразование в строку
        /// </summary>
        /// <param name="value">логическое значение</param>
        /// <returns>возвращает логическое значение, преобразованное в строку, допустимую для записи в БД</returns>
        public static string ToString(bool value)
        {
            return (value) ? "1" : "0";
        }

        /// <summary>
        /// Преобразование в строку ДАТЫ
        /// </summary>
        /// <param name="value">значение даты-времени</param>
        /// <returns>возвращает дату, преобразованное в строку, допустимую для записи в БД</returns>
        public static string ToString(DateTime value)
        {
            return String.Format("{0}-{1}-{2}", value.Year, value.Month, value.Day); ;
        }

        /// <summary>
        /// Преобразование в целое число
        /// </summary>
        /// <param name="value">строка</param>
        /// <returns>возвращает целое число, или 0, если преобразование не удалось</returns>
        public static int ToInt(string value)
        {
            int temp = 0;
            if (int.TryParse(value, out temp))
                return temp;
            else
                return 0;
        }

        /// <summary>
        /// Преобразование в дробное число
        /// </summary>
        /// <param name="value">строка</param>
        /// <returns>возвращает дробное число, или 0, если преобразование не удалось</returns>
        public static double ToDouble(string value)
        {
            double temp = 0;
            if (double.TryParse(value.Replace('.', ','), out temp))
                return temp;
            else
                return 0;
        }

        /// <summary>
        /// Преобразование в логическое значение
        /// </summary>
        /// <param name="value">строка</param>
        /// <returns>возвращает логическое значение, или FALSE, если преобразование не удалось</returns>
        public static bool ToBool(string value)
        {
            bool temp = false;
            if (bool.TryParse(value, out temp))
                return temp;
            else
                return false;
        }

        /// <summary>
        /// Преобразование в дату-время
        /// </summary>
        /// <param name="value">строка</param>
        /// <returns>возвращает значение даты-времени, или DateTime.MinValue, если преобразование не удалось</returns>
        public static DateTime ToDateTime(string value)
        {
            DateTime temp = DateTime.MinValue;
            if (DateTime.TryParse(value, out temp))
                return temp;
            else
                return DateTime.MinValue;
        }

        /// <summary>
        /// Преобразование в расшифрованную строку
        /// </summary>
        /// <param name="value">зашифрованная строка</param>
        /// <returns>возвращает расшифрованную строку, или пустую строку, если преобразование не удалось</returns>
        public static string ToUncriptString(string value)
        {
            string temp = string.Empty;

            if (value != "")
                try
                {
                    temp = Criptex.UnCript(value);
                }
                catch
                {
                    temp = string.Empty;
                }

            return temp;
        }
    }
}
