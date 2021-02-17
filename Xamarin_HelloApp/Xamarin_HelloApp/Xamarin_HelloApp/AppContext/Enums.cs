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
        Slave,

        /// <summary>
        /// Запуск по ссылке
        /// </summary>
        Url
    }


    /// <summary>
    /// Состояние страницы
    /// </summary>
    public enum PageStatus
    {
        /// <summary>
        /// Свободна для выполнения операции
        /// </summary>
        Free,

        /// <summary>
        /// Страница занята
        /// </summary>
        Busy
    }


    /// <summary>
    /// Тип локализованной версии программы
    /// </summary>
    public enum LocalizedVersion
    {
        /// <summary>
        /// Без локализации
        /// </summary>
        None,

        /// <summary>
        /// Демонстрационная БД
        /// </summary>
        Demo,

        /// <summary>
        /// Пробный режим
        /// </summary>
        Trial,

        /// <summary>
        /// Пробный режим для тестирования через эмулятор
        /// </summary>
        TrialTestEmulator,

        /// <summary>
        /// АСКОН
        /// </summary>
        Ascon,

        /// <summary>
        /// АСКОН северо-запад
        /// </summary>
        SeveroZapad,

        /// <summary>
        /// Я
        /// </summary>
        TestMe
    }
}