using System;
using System.Text;

namespace PasswordCriptor
{
    /// <summary>
    /// Класс шифрования данных для безопасного хранения данных авторизации
    /// Разработал: Ряполов А.Н.
    ///             Аскон-Волга
    ///             2018 г
    ///             
    /// В случае, если предполагается использовать одни и те же настройки подключения при работе разных плагинов,
    /// рекомендуется использовать конструктор без параметров
    /// </summary>
    public class Criptex : IDisposable
    {
        public static string closedKey { get; set; } // закрытый ключ для шифрования

        public static int startSymbols { get; set; } // количество символов, вставляемых в начало строки

        public static int endSymbols { get; set; } // количество символов, вставляемых в конец строки

        public static bool doOverflow { get; set; } // флаг использования дополнительного переполнения

        /// <summary>
        /// Создание объекта для шифрования/расшифровки данных
        /// </summary>
        public Criptex()
        {
            closedKey = "xMNBmwMH*4R2rD5n9T~YgmYE~V}XWi8k";
            startSymbols = 20;
            endSymbols = 30;
            doOverflow = true;
        }

        /// <summary>
        /// Создание объекта для шифрования/расшифровки данных
        /// </summary>
        /// <param name="key">Закрытый ключ для шифрования</param>
        public Criptex(string key)
        {
            closedKey = key;
            startSymbols = 20;
            endSymbols = 30;
            doOverflow = true;
        }

        /// <summary>
        /// Создание объекта для шифрования/расшифровки данных
        /// </summary>
        /// <param name="key">Закрытый ключ для шифрования</param>
        /// <param name="start">Количество случайных символов, добавляемых в начале строки</param>
        /// <param name="finish">Количество случайных символов, добавляемых в конце строки</param>
        public Criptex(string key, int start, int finish)
        {
            closedKey = key;
            startSymbols = start;
            endSymbols = finish;
            doOverflow = true;
        }

        /// <summary>
        /// Создание объекта для шифрования/расшифровки данных
        /// </summary>
        /// <param name="key">Закрытый ключ для шифрования</param>
        /// <param name="start">Количество случайных символов, добавляемых в начале строки</param>
        /// <param name="finish">Количество случайных символов, добавляемых в конце строки</param>
        /// <param name="overflow">Флаг использования дополнительного переполнения строки (помимо случайных символов в начале и в конце)</param>
        public Criptex(string key, int start, int finish, bool overflow)
        {
            closedKey = key;
            startSymbols = start;
            endSymbols = finish;
            doOverflow = overflow;
        }

        /// <summary>
        /// Шифрование строки
        /// </summary>
        /// <param name="s">Строка для шифрования</param>
        /// <returns></returns>
        public string Cript(string s)
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
        public string UnCript(string s)
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

        /// <summary>
        /// Освобождение объекта
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}

