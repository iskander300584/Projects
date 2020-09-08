using System;

namespace GreenLeaf.Classes
{
    [Serializable]
    public class SaveConnectionData
    {
        /// <summary>
        /// Зашифрованная строка подключения
        /// </summary>
        public string ConnectionString = string.Empty;

        /// <summary>
        /// Владелец ключа
        /// </summary>
        public string Owner = string.Empty;

        //public string Server = string.Empty;

        //public string DB = string.Empty;

        //public string AdminLogin = string.Empty;

        //public string AdminPassword = string.Empty;
    }
}
