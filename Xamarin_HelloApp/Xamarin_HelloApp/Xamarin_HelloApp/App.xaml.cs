using Xamarin.Forms;
using Xamarin_HelloApp.AppContext;
using Xamarin_HelloApp.Models;
using Xamarin_HelloApp.Pages;

namespace Xamarin_HelloApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            Global.DALContext = new Context();

            Credentials credentials = TryGetCredentials();
            if(credentials != null)
            {
                Global.DALContext.Connect(credentials);
            }

            if (Global.DALContext.IsInitialized)
                MainPage = new MainPage();
            else
                MainPage = new AuthorizePage();
        }

        protected override void OnStart()
        {
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
        /// <returns></returns>
        private Credentials TryGetCredentials()
        {
            object temp = "";
            string server = "", db = "", login = "", password = "";
            if (Current.Properties.TryGetValue("server", out temp))
            {
                server = (string)temp;
            }
            else
                return null;

            if (Current.Properties.TryGetValue("db", out temp))
            {
                db = (string)temp;
            }
            else
                return null;

            if (Current.Properties.TryGetValue("login", out temp))
            {
                login = (string)temp;
            }
            else
                return null;

            if (Current.Properties.TryGetValue("password", out temp))
            {
                password = (string)temp;
            }
            else
                return null;

            return Credentials.GetProtectedCredentials(server, db, login, password);
        }
    }
}
