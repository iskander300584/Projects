using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin_HelloApp.AppContext;
using Xamarin_HelloApp.Commands;
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
                }
            }
        }


        private string login = string.Empty;
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
                }
            }
        }


        private string password = string.Empty;
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
            connectCommand = new AuthorizeConnectCommand(this);
        }


        /// <summary>
        /// Получение признака видимости сообщения об ошибке
        /// </summary>
        private void GetIsErrorVisible()
        {
            IsErrorVisible = (error != "");
        }


        /// <summary>
        /// Проверка соединения с БД 
        /// TODO сохранение настроек
        /// </summary>
        /// <returns>возвращает TRUE в случае успешного подключения</returns>
        public void CheckConnection()
        {
            Credentials credentials = Credentials.GetConnectionCredentials(server.Trim(), db.Trim(), login.Trim(), password.Trim());
            Exception ex = Global.DALContext.Connect(credentials);
            if (ex != null)
            {
                Error = ex.Message;
                return;
            }

            // TODO сохранение настроек

            page.NavigateToMainPage();
        }


    }
}