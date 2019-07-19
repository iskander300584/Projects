using System.Windows;

namespace GreenLeaf.Windows.Dialogs
{
    /// <summary>
    /// Предупреждающее окно
    /// </summary>
    public partial class WarningView : Window
    {
        /// <summary>
        /// Предупреждающее окно
        /// </summary>
        /// <param name="message">текст сообщения</param>
        /// <param name="title">текст заголовка</param>
        public WarningView(string message, string title = "")
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
