using Ascon.Pilot.DataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
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


        /// <summary>
        /// Переподключиться к БД
        /// </summary>
        /// <returns>возвращает NULL в случае успешного подключения или ошибку</returns>
        public static Exception Reconnect()
        {
            try
            {
                // Проверка необходимости переподключения
                DObject rootObj = DALContext.Repository.GetObjects(new[] { DObject.RootId }).First();

                return null;
            }
            catch
            {
                // Переподключение к БД
                Exception except = DALContext.Connect(Credentials);
                return except;
            }
        }
    }
}