using Ascon.Pilot.DataClasses;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin_HelloApp.Models;

namespace Xamarin_HelloApp.AppContext
{
    static class Global
    {
        /// <summary>
        /// Подключенный пользователь
        /// </summary>
        public static DPerson CurrentPerson;


        /// <summary>
        /// Контекст доступа к данным
        /// </summary>
        public static Context DALContext;


        /// <summary>
        /// Настройки подключения
        /// </summary>
        public static Credentials Credentials;


        /// <summary>
        /// Получение прав доступа к файлам для приложения
        /// </summary>
        /// <returns>возвращает TRUE, если все необходимые права получены</returns>
        public async static Task<bool> GetFilesPermissions()
        {
            // Получение прав на запись файлов
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
            if(status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.StorageWrite>();

                if (status != PermissionStatus.Granted)
                    return false;
            }

            // Получение прав на доступ к файлам
            status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.StorageRead>();

                if (status != PermissionStatus.Granted)
                    return false;
            }

            return true;
        }


        /// <summary>
        /// Получение метаданных БД Pilot
        /// </summary>
        public static void GetMetaData()
        {
            // Получение подключенного пользователя
            CurrentPerson = DALContext.Repository.CurrentPerson();

            // Получение типов БД Pilot
            IEnumerable<MType> types = DALContext.Repository.GetTypes();
            TypeFabrique.ClearTypes();
            foreach (MType mType in types)
                TypeFabrique.GetType(mType.Id);
        }
    }
}