using System.Windows;

namespace GreenLeaf.Windows.Dialogs
{
    /// <summary>
    /// Окно вопроса
    /// </summary>
    public partial class QuestionView : Window
    {
        /// <summary>
        /// Окно вопроса
        /// </summary>
        /// <param name="message">текст сообщения</param>
        /// <param name="title">текст заголовка</param>
        public QuestionView(string message, string title = "")
        {
            InitializeComponent();

            if (title.Trim() != "")
                this.Title = title.Trim();

            tbMessage.Text = message;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
