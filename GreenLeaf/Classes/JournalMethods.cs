using System;

namespace GreenLeaf.Classes
{
    /// <summary>
    /// Общие методы для работы с журналом событий
    /// </summary>
    public static class JournalMethods
    {
        /// <summary>
        /// Текущая дата в формате для SQL
        /// </summary>
        /// <returns></returns>
        public static string CurrentDate()
        {
            DateTime dt = DateTime.Now;
            string date = String.Format("{0}-{1}-{2} {3}:{4}:{5}", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);

            return date;
        }

        /// <summary>
        /// Начало фразы для записи в журнал событий
        /// </summary>
        /// <param name="verb">глагол выполненного действия</param>
        /// <returns></returns>
        public static string JournalItemHeader(string verb)
        {
            return String.Format("{0} {1} {2} {3} ", ConnectSetting.CurrentUser.PersonalData.Surname, ConnectSetting.CurrentUser.PersonalData.Name, ConnectSetting.CurrentUser.PersonalData.Patronymic, (ConnectSetting.CurrentUser.PersonalData.Sex) ? verb : verb + "а");
        }
    }
}
