using PilotMobile.Pages;
using Plugin.Settings;
using System;
using System.IO;
using Xamarin.Forms;
using Xamarin_HelloApp.AppContext;
using Xamarin_HelloApp.Models;
using Xamarin_HelloApp.Pages;

namespace Xamarin_HelloApp
{
    /// <summary>
    /// Класс запуска мобильного клиента Pilot
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Класс запуска мобильного клиента Pilot
        /// </summary>
        public App(string url)
        {
            InitializeComponent();

            Global.DALContext = new Context();

            Credentials credentials = TryGetCredentials();
            if(credentials != null)
            {
                if (Global.DALContext.Connect(credentials) == null)
                    Global.Credentials = credentials;
            }

            if (Global.DALContext.IsInitialized)
            {
                Global.GetMetaData();

                MainPage = new MainCarrouselPage(url);
            }
            else
                MainPage = new AuthorizePage();
        }

        protected override void OnStart()
        {
            // Очистка КЕШа файлов XPS
            string[] files = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            foreach(string fileName in files)
                if(fileName.Contains(".xps"))
                {
                    try
                    {
                        File.Delete(fileName);
                    }
                    catch { }
                }
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }


        /// <summary>
        /// Получение настроек подключения
        /// </summary>
        private Credentials TryGetCredentials()
        {
            object temp = "";
            string server = "", db = "", login = "", password = "";
            int license = 0;

            server = CrossSettings.Current.GetValueOrDefault("server", "");
            db = CrossSettings.Current.GetValueOrDefault("db", "");
            login = CrossSettings.Current.GetValueOrDefault("login", "");
            password = CrossSettings.Current.GetValueOrDefault("password", "");
            license = CrossSettings.Current.GetValueOrDefault("license", 0);

            if (server == "" || db == "" || login == "" || password == "" || license == 0)
                return null;

            return Credentials.GetProtectedCredentials(server, db, login, password, license);
        }
    }
}