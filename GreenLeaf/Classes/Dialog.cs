using GreenLeaf.Windows.Dialogs;
using System.Windows;
using System.Threading;

namespace GreenLeaf.Classes
{
    public static class Dialog
    {
        /// <summary>
        /// Информационное сообщение
        /// </summary>
        /// <param name="sender">окно, вызвавшее сообщение</param>
        /// <param name="message">текст сообщения</param>
        /// <param name="title">заголовок окна</param>
        public static void InfoMessage(Window sender, string message, string title = "")
        {
            InfoView view = new InfoView(message, title);

            try
            {
                if (sender != null)
                    view.Owner = sender;
                else
                    view.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
            catch
            {
                view.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            view.ShowDialog();
        }

        /// <summary>
        /// Сообщение об ошибке
        /// </summary>
        /// <param name="sender">окно, вызвавшее сообщение</param>
        /// <param name="message">текст сообщения</param>
        /// <param name="error">текст ошибки</param>
        /// <param name="title">заголовок окна</param>
        public static void ErrorMessage(Window sender, string message, string error = "", string title = "")
        {
            ErrorView view = new ErrorView(message, error, title);

            try
            {
                if (sender != null)
                    view.Owner = sender;
                else
                    view.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
            catch
            {
                view.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            view.ShowDialog();
        }

        /// <summary>
        /// Вопрос
        /// </summary>
        /// <param name="sender">окно, вызвавшее сообщение</param>
        /// <param name="message">текст сообщения</param>
        /// <param name="title">заголовок окна</param>
        public static MessageBoxResult QuestionMessage(Window sender, string message, string title = "")
        {
            QuestionView view = new QuestionView(message, title);

            try
            {
                if (sender != null)
                    view.Owner = sender;
                else
                    view.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
            catch
            {
                view.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            bool? result = view.ShowDialog();
            view.Close();

            if (result == true)
                return MessageBoxResult.Yes;
            else if (result == false)
                return MessageBoxResult.No;
            else
                return MessageBoxResult.Cancel;
        }

        /// <summary>
        /// Предупреждение
        /// </summary>
        /// <param name="sender">окно, вызвавшее сообщение</param>
        /// <param name="message">текст сообщения</param>
        /// <param name="title">заголовок окна</param>
        public static void WarningMessage(Window sender, string message, string title = "")
        {
            WarningView view = new WarningView(message, title);

            try
            {
                if (sender != null)
                    view.Owner = sender;
                else
                    view.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
            catch
            {
                view.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            view.ShowDialog();
        }

        /// <summary>
        /// Полупрозрачное информационное окно
        /// </summary>
        /// <param name="sender">окно, вызвавшее сообщение</param>
        /// <param name="message">текст сообщения</param>
        /// <param name="ms">время отображения окна (мкс)</param>
        public static void TransparentMessage(Window sender, string message, int ms = 1500)
        {
            TransparentView view = new TransparentView(message);

            try
            {
                if (sender != null)
                    view.Owner = sender;
                else
                    view.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
            catch
            {
                view.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            view.Show();

            Thread.Sleep(ms);

            view.Close();
        }
    }
}
