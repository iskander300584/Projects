/// <summary>
/// Используемые перечисления
/// </summary>
namespace PilotMobile.AppContext
{
    /// <summary>
    /// Режим работы страницы
    /// </summary>
    public enum PageMode
    {
        /// <summary>
        /// Просмотр
        /// </summary>
        View,

        /// <summary>
        /// Создание
        /// </summary>
        Create,

        /// <summary>
        /// Редактирование
        /// </summary>
        Edit,

        /// <summary>
        /// Поиск
        /// </summary>
        Search,

        /// <summary>
        /// Подчиненное окно с ограниченным функционалом
        /// </summary>
        Slave
    }
}