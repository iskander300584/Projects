using Ascon.Pilot.DataClasses;
using System;
using System.Collections.Generic;
using System.Text;
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
    }
}