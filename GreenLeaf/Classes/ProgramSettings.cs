using System.Collections.Generic;
using GreenLeaf.ViewModel;

namespace GreenLeaf.Classes
{
    /// <summary>
    /// Настройки подключения
    /// </summary>
    public static class ProgramSettings
    {
        /// <summary>
        /// Путь к рабочей папке
        /// </summary>
        public const string WorkFolder = @"C:\ProgramData\GreenLeaf\";

        private static string _server = string.Empty;
        /// <summary>
        /// Имя сервера
        /// </summary>
        public static string Server
        {
            get { return _server; }
            set
            {
                if(_server != value)
                {
                    _server = value;
                    BuildConnectionString();
                }
            }
        }

        private static string _db = string.Empty;
        /// <summary>
        /// Имя базы данных
        /// </summary>
        public static string DB
        {
            get { return _db; }
            set
            {
                if(_db != value)
                {
                    _db = value;
                    BuildConnectionString();
                }
            }
        }

        private static string _connectionString = string.Empty;
        /// <summary>
        /// Строка подключения к БД
        /// </summary>
        public static string ConnectionString
        {
            get { return _connectionString; }
        }

        private static string _adminLogin = string.Empty;
        /// <summary>
        /// Имя учетной записи администратора
        /// </summary>
        public static string AdminLogin
        {
            get { return _adminLogin; }
            set
            {
                if(_adminLogin != value)
                {
                    _adminLogin = value;
                    BuildConnectionString();
                }
            }
        }

        private static string _adminPassword = string.Empty;
        /// <summary>
        /// Пароль учетной записи администратора
        /// </summary>
        public static string AdminPassword
        {
            get { return _adminPassword; }
            set
            {
                if (_adminPassword != value)
                {
                    _adminPassword = value;
                    BuildConnectionString();
                }
            }
        }

        /// <summary>
        /// Подключенный пользователь
        /// </summary>
        public static Account CurrentUser = new Account();

        /// <summary>
        /// Коллекция настроек
        /// </summary>
        public static IDictionary<string, string> Settings = new Dictionary<string, string>();

        /// <summary>
        /// Построение строки подключения
        /// </summary>
        private static void BuildConnectionString()
        {
            if (AdminPassword != string.Empty)
            {
                try
                {
                    _connectionString = Criptex.Cript("SERVER=" + Server + ";" +
                                            "DATABASE=" + DB + ";" +
                                            "UID=" + AdminLogin + ";" +
                                            "Pwd=" + Criptex.UnCript(AdminPassword) + ";");
                }
                catch { }
            }
        }
    }
}
