using System.Windows;

namespace GreenLeaf.Windows.Dialogs
{
    /// <summary>
    /// Информационное окно
    /// </summary>
    public partial class InfoView : Window
    {
        /// <summary>
        /// Информационное окно
        /// </summary>
        /// <param name="message">текст сообщения</param>
        /// <param name="title">текст заголовка</param>
        public InfoView(string message, string title="")
        {
            InitializeComponent();

            if (title.Trim() != "")
                this.Title = title.Trim();

            tbMessage.Text = message;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
