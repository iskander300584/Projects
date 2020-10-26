using Ascon.Pilot.Common;

namespace Xamarin_HelloApp.Models
{
    /// <summary>
    /// Настройки подключения
    /// </summary>
    public class Credentials
    {
        /// <summary>
        /// Сервер БД
        /// </summary>
        public string ServerUrl { get; private set; }
        //public Uri ServerUrl { get; private set; }
        
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string Username { get; private set; }

        /// <summary>
        /// Зашифрованный пароль
        /// </summary>
        public string ProtectedPassword { get; private set; }

        /// <summary>
        /// Признак использования аутентификации Windows
        /// </summary>
        public bool UseWindowsAuth
        {
            get { return !string.IsNullOrEmpty(Username) && (Username.Contains("\\") || Username.Contains("@")); }
        }

        /// <summary>
        /// Имя БД
        /// </summary>
        public string DatabaseName { get; private set; }

        /// <summary>
        /// Тип лицензии
        /// </summary>
        public int License { get; private set; }

        public static Credentials GetConnectionCredentials(string database, string username, string password)
        {
            var credentials = new Credentials
            {
                ServerUrl = @"http://ecm.ascon.ru:5545/",
                //ServerUrl = new Uri(@"http://ecm.ascon.ru:5545/"),
                Username = username,
                ProtectedPassword = password,
                DatabaseName = database,
            };

            return credentials;
        }


        /// <summary>
        /// Получить настройки подключения
        /// </summary>
        /// <param name="server">имя сервера</param>
        /// <param name="database">имя БД</param>
        /// <param name="username">имя пользователя</param>
        /// <param name="password">пароль</param>
        /// <param name="license">тип лицензии</param>
        /// <returns>возвращает соответствующие настройки подключения</returns>
        public static Credentials GetConnectionCredentials(string server, string database, string username, string password, int license)
        {
            var credentials = new Credentials
            {
                ServerUrl = server,
                Username = username,
                ProtectedPassword = password.EncryptAes(),
                DatabaseName = database,
                License = license
            };

            return credentials;
        }


        /// <summary>
        /// Получить настройки подключения без преобразования пароля
        /// </summary>
        /// <param name="server">имя сервера</param>
        /// <param name="database">имя БД</param>
        /// <param name="username">имя пользователя</param>
        /// <param name="password">пароль</param>
        /// <param name="license">тип лицензии</param>
        /// <returns>возвращает соответствующие настройки подключения</returns>
        public static Credentials GetProtectedCredentials(string server, string database, string username, string password, int license)
        {
            var credentials = new Credentials
            {
                ServerUrl = server,
                Username = username,
                ProtectedPassword = password,
                DatabaseName = database,
                License = license
            };

            return credentials;
        }
    }
}