using System;

namespace GreenLeaf.Classes
{
    /// <summary>
    /// Класс данных ключа подключения к БД
    /// </summary>
    [Serializable]
    public class ConnectionKey
    {
        /// <summary>
        /// Строка подключения
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Дата создания ключа
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Владелец ключа
        /// </summary>
        public string Owner { get; set; }
    }
}