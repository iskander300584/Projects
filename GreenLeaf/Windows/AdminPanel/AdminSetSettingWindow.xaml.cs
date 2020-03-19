using System.Windows;

namespace GreenLeaf.Windows.AdminPanel
{
    /// <summary>
    /// Окно изменения настройки
    /// </summary>
    public partial class AdminSetSettingWindow : Window
    {
        private string _setting = string.Empty;
        /// <summary>
        /// Значение настройки
        /// </summary>
        public string Setting
        {
            get { return _setting; }
        }

        /// <summary>
        /// Окно изменения настройки
        /// </summary>
        /// <param name="title">заголовок окна</param>
        /// <param name="value">значение настройки</param>
        public AdminSetSettingWindow(string title, string value)
        {
            InitializeComponent();

            this.Title = title;

            _setting = value;
            tbSetting.Text = value;
        }

        /// <summary>
        /// Применить
        /// </summary>
        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            _setting = tbSetting.Text;
            this.DialogResult = true;
        }
    }
}