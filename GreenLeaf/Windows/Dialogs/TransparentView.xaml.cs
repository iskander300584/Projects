using System.Windows;

namespace GreenLeaf.Windows.Dialogs
{
    /// <summary>
    /// Полупрозрачное окно
    /// </summary>
    public partial class TransparentView : Window
    {
        /// <summary>
        /// Полупрозрачное окно
        /// </summary>
        /// <param name="message">текст сообщения</param>
        public TransparentView(string message)
        {
            InitializeComponent();
            tbMessage.Text = message;
        }
    }
}
