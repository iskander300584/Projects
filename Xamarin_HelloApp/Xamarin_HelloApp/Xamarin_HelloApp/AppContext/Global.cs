﻿using Ascon.Pilot.DataClasses;
using Plugin.Toast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin_HelloApp.Models;
using Plugin.Messaging;
using Xamarin.Forms;
using PilotMobile.AppContext;
using Plugin.Share;


namespace Xamarin_HelloApp.AppContext
{
    /// <summary>
    /// Общие данные приложения
    /// </summary>
    static class Global
    {
        #region Поля класса

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
        /// Список машин состояний
        /// </summary>
        public static List<MUserStateMachine> StateMachines;


        /// <summary>
        /// Признак отображенного сообщения пробной версии
        /// </summary>
        public static bool TrialMessageShown = false;


        /// <summary>
        /// Дата окончания пробного периода
        /// </summary>
        public static readonly DateTime TrialExitDate = new DateTime(2021, 5, 31);


        /// <summary>
        /// Признак локализованной версии
        /// </summary>
        public static LocalizedVersion Localized = LocalizedVersion.Trial;


        /// <summary>
        /// Минимальная версия, при которой не отображается подсказка
        /// </summary>
        public const int DoNotShowHelp_Version = 10519;


        /// <summary>
        /// Минимальная версия, при которой отображается подсказка об обновлениях
        /// </summary>
        public const int ShowUpdate_Version = 10519;
             

        #endregion


        #region Методы класса

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

            // Получение машин состояний
            StateMachines = DALContext.Repository.GetStateMachines();
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
                if (DALContext == null)
                    throw new Exception("Не определен класс \'DALContext\'");
                else if(Credentials == null)
                    throw new Exception("Не определен класс \'Credentials\'");

                // Переподключение к БД
                Exception except = DALContext.Connect(Credentials);
                return except;
            }
        }


        /// <summary>
        /// Копировать ссылку в буфер обмена
        /// </summary>
        /// <param name="dObject">объект Pilot</param>
        /// <returns>возвращает True, по окончании операции, False в случае ошибки</returns>
        public static async Task<bool> CopyLink(DObject dObject)
        {
            try
            {
                await Clipboard.SetTextAsync(GetLink(dObject));

                ShowToast("Ссылка скопирована");

                return true;
            }
            catch
            {
                ShowToast("Ошибка копирования ссылки", true);

                return false;
            }
        }


        /// <summary>
        /// Поделиться ссылкой
        /// </summary>
        /// <param name="dObject">объект Pilot</param>
        /// <returns>возвращает True, по окончании операции, False в случае ошибки</returns>
        public static async Task<bool> ShareLink(DObject dObject)
        {
            try
            {
                await CrossShare.Current.Share(new Plugin.Share.Abstractions.ShareMessage { Url = GetLink(dObject) });

                return true;
            }
            catch
            {
                ShowToast("Ошибка формирования ссылки", true);

                return false;
            }
        }


        /// <summary>
        /// Отобразить всплывающее сообщение
        /// </summary>
        /// <param name="message">текст сообщения</param>
        /// <param name="error">признак сообщения об ошибке</param>
        public static void ShowToast(string message, bool error = false)
        {
            if(!error)
            {
                CrossToastPopUp.Current.ShowCustomToast(message, UIConstants.PilotInterpriceColor, UIConstants.WhiteColor, Plugin.Toast.Abstractions.ToastLength.Short);
            }
            else
            {
                CrossToastPopUp.Current.ShowToastError(message);
            }
        }


        /// <summary>
        /// Получить ссылку на объект
        /// </summary>
        /// <param name="dObject">объект Pilot</param>
        /// <returns></returns>
        private static string GetLink(DObject dObject)
        {
            return Credentials.ServerUrl + "/url?id=" + dObject.Id.ToString();
        }


        /// <summary>
        /// Отправка сообщения об ошибке
        /// </summary>
        /// <param name="ex">ошибка</param>
        public static async Task<bool> SendErrorReport(Exception ex)
        {
            try
            {
                string message = "";

                // Информация об ошибке
                message += "Информация об ошибке:\n\n";
                message += ex.Message + "\n\n";
                message += "Код ошибки: " + ex.HResult + "\n\n";

                if(ex.InnerException != null)
                {
                    message += "Внутренние ошибки:\n\n";

                    message += GetInnerError(ex.InnerException) + "\n";
                }

                if(ex.Source != null && ex.Source != "")
                {
                    message += "Источник ошибки: " + ex.Source + "\n\n";
                }

                if(ex.StackTrace != null && ex.StackTrace != "")
                {
                    message += "Стек вызова:\n" + ex.StackTrace + "\n\n";
                }

                // Отправка сообщения
                var Builder = new EmailMessageBuilder()
                .To("ryapolov_an@ascon.ru")
                .Cc("aldr046@mail.ru")
                .Subject("Отчет об ошибке Pilot-FLY")
                .Body(message);
                var email = Builder.Build();

                CrossMessaging.Current.EmailMessenger.SendEmail(email);
                
                return true;
            }
            catch
            {
                ShowToast("Ошибка отправки отчета", true);

                return false;
            }
        }


        /// <summary>
        /// Рекурсивное получение информации о внутренних ошибках
        /// </summary>
        /// <param name="ex">ошибка</param>
        private static string GetInnerError(Exception ex)
        {
            string error = "";

            if (ex.InnerException != null)
                error = GetInnerError(ex.InnerException);

            error = ex.Message + "\n" + error;

            return error;
        }


        /// <summary>
        /// Вывести сообщение
        /// </summary>
        /// <param name="page">окно приложения</param>
        /// <param name="caption">заголовок</param>
        /// <param name="message">текст сообщения</param>
        /// <param name="isQuestion">сообщение является вопросом</param>
        /// <returns>возвращает TRUE, если ответ "Да" или сообщение не является вопросом</returns>
        public static async Task<bool> DisplayMessage(ContentPage page, string caption, string message, bool isQuestion)
        {
            if (!isQuestion)
            {
                await page.DisplayAlert(caption, message, StringConstants.Ok);

                return true;
            }
            else
            {
                return await page.DisplayAlert(caption, message, StringConstants.Yes, StringConstants.No);
            }
        }


        /// <summary>
        /// Вывести сообщение об ошибке
        /// </summary>
        /// <param name="page">окно приложения</param>
        /// <param name="message">текст сообщения об ошибке</param>
        /// <param name="caption">заголовок ошибки</param>
        /// <returns>возвращает TRUE, если необходимо отправить отчет об ошибке</returns>
        public static async Task<bool> DisplayError(ContentPage page, string message, string caption = "Ошибка")
        {
            return await page.DisplayAlert(caption, message + StringConstants.SendErrorMessage, StringConstants.Send, StringConstants.DontSend);
        }

        #endregion
    }
}