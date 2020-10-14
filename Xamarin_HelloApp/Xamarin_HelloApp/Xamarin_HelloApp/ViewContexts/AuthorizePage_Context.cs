using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
        /// <summary>
        /// Страница авторизации
        /// </summary>
        private AuthorizePage page;


        private string server = @"http://ecm.ascon.ru:5545";
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


        private string db = "pilot_ascon";
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


        private string login = "ryapolov_an";// string.Empty;
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


        private string password = "sSR4mzCQ"; // string.Empty;
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


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        /// <summary>
        /// Контекст данных окна авторизации
        /// </summary>
        public AuthorizePage_Context(AuthorizePage authorizePage)
        {
            page = authorizePage;
            connectCommand = new Command(CheckConnection);
            CheckConnection_CanExecute();
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
        /// <returns>возвращает TRUE в случае успешного подключения</returns>
        public void CheckConnection()
        {
            Error = string.Empty;

            Credentials credentials = Credentials.GetConnectionCredentials(server.Trim(), db.Trim(), login.Trim(), password.Trim());
            Exception ex = Global.DALContext.Connect(credentials);
            if (ex != null)
            {
                Error = ex.Message;
                return;
            }

            Global.Credentials = credentials;

            // Сохранение настроек
            object temp = "";
            if (App.Current.Properties.TryGetValue("server", out temp))
            {
                App.Current.Properties["server"] = credentials.ServerUrl;
            }
            else
                App.Current.Properties.Add("server", credentials.ServerUrl);

            if (App.Current.Properties.TryGetValue("db", out temp))
            {
                App.Current.Properties["db"] = credentials.DatabaseName;
            }
            else
                App.Current.Properties.Add("db", credentials.DatabaseName);

            if (App.Current.Properties.TryGetValue("login", out temp))
            {
                App.Current.Properties["login"] = credentials.Username;
            }
            else
                App.Current.Properties.Add("login", credentials.Username);

            if (App.Current.Properties.TryGetValue("password", out temp))
            {
                App.Current.Properties["password"] = credentials.ProtectedPassword;
            }
            else
                App.Current.Properties.Add("password", credentials.ProtectedPassword);

            Global.GetMetaData();

            page.NavigateToMainPage();
        }
    }
}