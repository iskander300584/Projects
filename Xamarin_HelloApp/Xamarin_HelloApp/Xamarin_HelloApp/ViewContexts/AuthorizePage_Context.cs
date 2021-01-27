using PilotMobile.AppContext;
using PilotMobile.Pages;
using Plugin.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin_HelloApp.AppContext;
using Xamarin_HelloApp.Models;
using Xamarin_HelloApp.Pages;


namespace Xamarin_HelloApp.ViewContexts
{
    /// <summary>
    /// Контекст данных окна авторизации
    /// </summary>
    class AuthorizePage_Context : INotifyPropertyChanged
    {
        #region Поля класса


        /// <summary>
        /// Наименование приложения
        /// </summary>
        public string AppName
        {
            get => StringConstants.ApplicationName;
        }


        /// <summary>
        /// Страница авторизации
        /// </summary>
        private AuthorizePage page;


        /// <summary>
        /// Признак смены учетной записи
        /// </summary>
        private bool reconnect;


        private string server = "";
        /// <summary>
        /// Имя сервера
        /// </summary>
        public string Server
        {
            get => server;
            set
            {
                if(server != value)
                {
                    server = value;
                    OnPropertyChanged();

                    CheckConnection_CanExecute();
                }
            }
        }


        private string db = "";
        /// <summary>
        /// Имя базы данных
        /// </summary>
        public string DB
        {
            get => db;
            set
            {
                if(db != value)
                {
                    db = value;
                    OnPropertyChanged();

                    CheckConnection_CanExecute();
                }
            }
        }


        private string login = "";
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string Login
        {
            get => login;
            set
            {
                if(login != value)
                {
                    login = value;
                    OnPropertyChanged();

                    CheckConnection_CanExecute();
                }
            }
        }


        private string password = "";
        /// <summary>
        /// Пароль
        /// </summary>
        public string Password
        {
            get => password;
            set
            {
                if(password != value)
                {
                    password = value;
                    OnPropertyChanged();

                    CheckConnection_CanExecute();
                }
            }
        }


        private List<string> licenses = new List<string>();
        /// <summary>
        /// Список доступных лицензий
        /// </summary>
        public List<string> Licenses
        {
            get => licenses;
        }


        private int selectedLicenseIndex = 0;
        /// <summary>
        /// Номер выбранного типа лицензии
        /// </summary>
        public int SelectedLicenseIndex
        {
            get => selectedLicenseIndex;
            set
            {
                if(selectedLicenseIndex != value)
                {
                    selectedLicenseIndex = value;
                    OnPropertyChanged();
                }
            }
        }


        private string error = string.Empty;
        /// <summary>
        /// Текст сообщения об ошибке
        /// </summary>
        public string Error
        {
            get => error;
            private set
            {
                if(error != value)
                {
                    error = value;
                    OnPropertyChanged();

                    GetIsErrorVisible();
                }
            }
        }


        private bool isErrorVisible = false;
        /// <summary>
        /// Признак отображения сообщения об ошибке
        /// </summary>
        public bool IsErrorVisible
        {
            get => isErrorVisible;
            private set
            {
                if(isErrorVisible != value)
                {
                    isErrorVisible = value;
                    OnPropertyChanged();
                }
            }
        }


        private bool isIndicatorActive = false;
        /// <summary>
        /// Активность индикатора подключения
        /// </summary>
        public bool IsIndicatorActive
        {
            get => isIndicatorActive;
            private set
            {
                if(isIndicatorActive != value)
                {
                    isIndicatorActive = value;
                    OnPropertyChanged();

                    page.ActiveIndicator(isIndicatorActive);
                }
            }
        }


        private ICommand connectCommand;
        /// <summary>
        /// Команда подключения
        /// </summary>
        public ICommand ConnectCommand
        {
            get => connectCommand;
        }


        private bool canConnectCommand = false;
        /// <summary>
        /// Признак возможности выполнения команды
        /// </summary>
        public bool CanConnectCommand
        {
            get => canConnectCommand;
            private set
            {
                if(canConnectCommand != value)
                {
                    canConnectCommand = value;
                    OnPropertyChanged();
                }
            }
        }


        private bool connectionChecked = false;
        /// <summary>
        /// Проверка подключения выполнена
        /// </summary>
        private bool ConnectionChecked
        {
            get => connectionChecked;
            set
            {
                connectionChecked = value;

                IsIndicatorActive = !connectionChecked;
            }
        }


        private bool isConnected = false;
        /// <summary>
        /// Соединение установлено
        /// </summary>
        private bool IsConnected
        {
            get => isConnected;
            set
            {
                if (isConnected != value)
                {
                    isConnected = value;

                    GoToProgram();
                }
            }
        }


        private bool isServerReadOnly = false;
        /// <summary>
        /// Доступность имени сервера для редактирования
        /// </summary>
        public bool IsServerReadOnly
        {
            get => isServerReadOnly;
            private set
            {
                if(isServerReadOnly != value)
                {
                    isServerReadOnly = value;
                    OnPropertyChanged();
                }
            }
        }


        private bool isDbReadOnly = false;
        /// <summary>
        /// Доступность имени БД для редактирования
        /// </summary>
        public bool IsDbReadOnly
        {
            get => isDbReadOnly;
            private set
            {
                if(isDbReadOnly != value)
                {
                    isDbReadOnly = value;
                    OnPropertyChanged();
                }
            }
        }


        private bool isAccountReadOnly = false;
        /// <summary>
        /// Доступность учетной записи для редактирования
        /// </summary>
        public bool IsAccountReadOnly
        {
            get => isAccountReadOnly;
            private set
            {
                if(isAccountReadOnly != value)
                {
                    isAccountReadOnly = value;
                    OnPropertyChanged();
                }
            }
        }


        #endregion
       

        /// <summary>
        /// Контекст данных окна авторизации
        /// </summary>
        public AuthorizePage_Context(AuthorizePage authorizePage, bool reconnect)
        {
            page = authorizePage;
            this.reconnect = reconnect;
            SetLicenses();

            if (Global.Localized == LocalizedVersion.Demo)
            {
                IsServerReadOnly = true;
                IsDbReadOnly = true;
                IsAccountReadOnly = true;

                Server = DemoConstants.DemoServer;
                DB = DemoConstants.DemoDB;
                Login = DemoConstants.DemoAccount;
                Password = DemoConstants.DemoPassword;
                SelectedLicenseIndex = 2;
            }
            else if (Global.Localized != LocalizedVersion.None && Global.Localized != LocalizedVersion.Trial)
            {
                IsServerReadOnly = true;
                IsDbReadOnly = true;

                string[] array = GetLocalizeData();
                Server = array[0];
                DB = array[1];
                Login = array[2];
                Password = array[3];
            }
            else if(Global.Localized == LocalizedVersion.Trial)
            {
                IsServerReadOnly = true;

                string[] array = GetLocalizeData();
                Server = array[0];
                DB = array[1];
                Login = array[2];

                if (Global.Credentials == null || Global.Credentials.ProtectedPassword == "")
                    Password = array[3];

                if (Global.Credentials != null && Global.Credentials.Username != "")
                {
                    DB = Global.Credentials.DatabaseName;
                    Login = Global.Credentials.Username;
                }
            }
            else
            {
                Server = Global.Credentials.ServerUrl;
                DB = Global.Credentials.DatabaseName;
                Login = Global.Credentials.Username;
            }

            connectCommand = new Command(CheckConnection);
            CheckConnection_CanExecute();
        }


        #region Методы класса


        /// <summary>
        /// Задать список лицензий
        /// </summary>
        private void SetLicenses()
        {
            licenses.Add("Pilot-ICE");
            licenses.Add("Pilot-ICE Enterprise");
            licenses.Add("Pilot-ECM");
            licenses.Add("Pilot-Storage");
            licenses.Add("Pilot-BIM");
        }

        /// <summary>
        /// Получение признака видимости сообщения об ошибке
        /// </summary>
        private void GetIsErrorVisible()
        {
            IsErrorVisible = (error != "");
        }


        /// <summary>
        /// Проверка возможности выполнения проверки подключения
        /// </summary>
        /// <returns></returns>
        public void CheckConnection_CanExecute()
        {
            CanConnectCommand = (Server.Trim() != "" && DB.Trim() != "" && Login.Trim() != "" && Password.Trim() != "");
        }


        /// <summary>
        /// Проверка соединения с БД
        /// </summary>
        public void CheckConnection()
        {
            ConnectionChecked = false;

            Error = string.Empty;

            Credentials credentials = Credentials.GetConnectionCredentials(server.Trim(), db.Trim(), login.Trim(), password.Trim(), GetLicenseType());

            Thread thread = new Thread(new ParameterizedThreadStart(AsyncCheckConnection));
            thread.Start(credentials);
        }


        /// <summary>
        /// Асинхронный метод подключения к БД
        /// </summary>
        /// <param name="_credentials">настойки авторизации</param>
        private void AsyncCheckConnection(object _credentials)
        {
            Credentials credentials = (Credentials)_credentials;

            Exception ex = Global.DALContext.Connect(credentials);
            if (ex != null)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    Error = ex.Message;
                    ConnectionChecked = true;
                });

                return;
            }

            Global.Credentials = credentials;

            // Сохранение настроек
            CrossSettings.Current.AddOrUpdateValue("server", credentials.ServerUrl);
            CrossSettings.Current.AddOrUpdateValue("db", credentials.DatabaseName);
            CrossSettings.Current.AddOrUpdateValue("login", credentials.Username);
            CrossSettings.Current.AddOrUpdateValue("password", credentials.ProtectedPassword);
            CrossSettings.Current.AddOrUpdateValue("license", credentials.License);


            Global.GetMetaData();

            Device.BeginInvokeOnMainThread(async () =>
            {
                ConnectionChecked = true;
                IsConnected = true;
            });
        }


        /// <summary>
        /// Возвращает код типа лицензии
        /// </summary>
        private int GetLicenseType()
        {

            switch(selectedLicenseIndex)
            {
                case 0:
                    return 100;

                case 1:
                    return 103;

                case 2:
                    return 101;

                case 3:
                    return 102;

                case 4:
                    return 90;
            }

            return 0;
        }


        /// <summary>
        /// Осуществить переход к программе
        /// </summary>
        private void GoToProgram()
        {
            if (!IsConnected)
                return;

            if (!reconnect)
                page.NavigateToMainPage();
            else
                App.Current.MainPage = new MainCarrouselPage(null);
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }


        /// <summary>
        /// Получить учетные данные локализованной версии
        /// </summary>
        public string[] GetLocalizeData()
        {
            string[] array = new string[4];

            switch (Global.Localized)
            {
                case LocalizedVersion.Ascon:
                    array[0] = @"http://ecm.ascon.ru:5545";
                    array[1] = "pilot_ascon";
                    array[2] = "";
                    array[3] = "";
                    break;

                case LocalizedVersion.SeveroZapad:
                    array[0] = @"http://213.33.246.218:6545";
                    array[1] = "demo";
                    array[2] = "";
                    array[3] = "";
                    break;

                case LocalizedVersion.TestMe:
                    array[0] = @"http://ecm.ascon.ru:5545";
                    array[1] = "pilot_ascon";
                    array[2] = "ryapolov_an";
                    array[3] = "sSR4mzCQ";
                    break;

                case LocalizedVersion.Trial:
                    array[0] = DemoConstants.DemoServer;
                    array[1] = DemoConstants.DemoDB;
                    array[2] = DemoConstants.DemoAccount;
                    array[3] = DemoConstants.DemoPassword;
                    break;

                default:
                    array[0] = "";
                    array[1] = "";
                    array[2] = "";
                    array[3] = "";
                    break;
            }

            return array;
        }


        #endregion
    }
}