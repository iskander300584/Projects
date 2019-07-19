using System;
using System.Text;

namespace GreenLeaf.Classes
{
    /// <summary>
    /// Класс шифрования данных
    /// </summary>
    public static class Criptex
    {
        private const string closedKey = "xMNBmwMH*4R2rD5n9T~YgmYE~V}XWi8k"; // закрытый ключ для шифрования

        private const int startSymbols = 20; // количество символов, вставляемых в начало строки

        private const int endSymbols = 30;   // количество символов, вставляемых в конец строки

        private const bool doOverflow = true; // флаг использования дополнительного переполнения

        /// <summary>
        /// Шифрование строки
        /// </summary>
        /// <param name="s">Строка для шифрования</param>
        /// <returns></returns>
        public static string Cript(string s)
        {
            StringBuilder str = new StringBuilder(s);
            Random rnd = new Random();

            // Шифрование сдвигом
            for (int i = 0; i < str.Length; i++)
            {
                if (i % 2 == 0)
                    str[i] = (char)((int)str[i] + 3);
                else
                    str[i] = (char)((int)str[i] + 5);

                if (i % 5 == 0)
                    str[i] = (char)((int)str[i] + 1);

                if (i % 11 == 0)
                    str[i] = (char)((int)str[i] + 7);

                if (i % 8 == 0)
                    str[i] = (char)((int)str[i] + 2);
            }

            // Внутреннее переполнение
            if (doOverflow)
            {
                int i = 1;
                while (i < str.Length)
                {
                    char symbol = (char)rnd.Next(65, 122);
                    str = str.Insert(i, symbol);
                    i += 2;
                }
            }

            // Переполнение строки
            string temp = string.Empty;

            // startSymbols символов в начало
            for (int i = 0; i < startSymbols; i++)
                temp += (char)rnd.Next(65, 122);
            str = new StringBuilder(temp + str.ToString());

            temp = string.Empty;
            // endSymbols символов в конец
            for (int i = 0; i < endSymbols; i++)
                temp += (char)rnd.Next(65, 122);
            str = new StringBuilder(str.ToString() + temp);

            // Шифрование ключем
            int k = 0;
            for (int i = 0; i < str.Length; i++)
            {
                str[i] = (char)((int)str[i] + (int)closedKey[k++]);
                if (k == closedKey.Length)
                    k = 0;
            }

            return str.ToString();
        }

        /// <summary>
        /// Расшифровка строки
        /// </summary>
        /// <param name="s">Строка для расшифровки</param>
        /// <returns></returns>
        public static string UnCript(string s)
        {
            StringBuilder str = new StringBuilder(s);

            // Дешифрование ключем
            int k = 0;
            for (int i = 0; i < str.Length; i++)
            {
                str[i] = (char)((int)str[i] - (int)closedKey[k++]);
                if (k == closedKey.Length)
                    k = 0;
            }

            // Дешифрование переполнения
            str = str.Remove(0, startSymbols);
            str = str.Remove(str.Length - endSymbols, endSymbols);

            // Дешифрование внутреннего переполнения
            if (doOverflow)
            {
                int i = 1;
                while (i < str.Length)
                    str = str.Remove(i++, 1);
            }

            // Дешифрование сдвигом
            for (int i = 0; i < str.Length; i++)
            {
                if (i % 2 == 0)
                    str[i] = (char)((int)str[i] - 3);
                else
                    str[i] = (char)((int)str[i] - 5);

                if (i % 5 == 0)
                    str[i] = (char)((int)str[i] - 1);

                if (i % 11 == 0)
                    str[i] = (char)((int)str[i] - 7);

                if (i % 8 == 0)
                    str[i] = (char)((int)str[i] - 2);
            }

            return str.ToString();
        }
    }
}

