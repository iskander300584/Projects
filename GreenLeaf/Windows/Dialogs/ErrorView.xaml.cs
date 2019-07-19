using System.Windows;

namespace GreenLeaf.Windows.Dialogs
{
    /// <summary>
    /// Окно сообщения об ошибке
    /// </summary>
    public partial class ErrorView : Window
    {
        /// <summary>
        /// Окно сообщения об ошибке
        /// </summary>
        /// <param name="message">текст сообщения</param>
        /// <param name="error">текст ошибки</param>
        /// <param name="title">текст заголовка</param>
        public ErrorView(string message, string error = "", string title = "")
        {
            InitializeComponent();

            if (title.Trim() != "")
                this.Title = title.Trim();

            if(message.Trim() == "" && error.Trim() != "")
            {
                tbMessage.Text = error.Trim();
                tbError.Visibility = Visibility.Collapsed;
            }
            else
            {
                tbMessage.Text = message.Trim();

                if (error.Trim() != "")
                    tbError.Text = error.Trim();
                else
                    tbError.Visibility = Visibility.Collapsed;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
