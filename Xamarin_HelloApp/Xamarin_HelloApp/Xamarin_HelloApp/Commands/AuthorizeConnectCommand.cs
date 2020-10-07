using System;
using System.Windows.Input;
using Xamarin_HelloApp.ViewContexts;

namespace Xamarin_HelloApp.Commands
{ 
    /// <summary>
    /// Команда подключения при авторизации
    /// </summary>
    class AuthorizeConnectCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;


        /// <summary>
        /// Контекст данных окна авторизации
        /// </summary>
        private AuthorizePage_Context context;


        /// <summary>
        /// Команда подключения при авторизации
        /// </summary>
        /// <param name="context">контекст данных окна авторизации</param>
        public AuthorizeConnectCommand(AuthorizePage_Context context)
        {
            this.context = context;
        }


        /// <summary>
        /// Проверка возможности выполнения команды
        /// </summary>
        public bool CanExecute(object parameter)
        {
            //return (context.Server.Trim() != "" && context.DB.Trim() != "" && context.Login.Trim() != "" && context.Password.Trim() != "");

            return true;
        }


        /// <summary>
        /// Выполнение команды
        /// </summary>
        public void Execute(object parameter)
        {
            if (CanExecute(parameter))
                context.CheckConnection();
        }
    }
}