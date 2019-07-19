using System.Windows;

namespace PasswordCriptor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Criptex criptex = new Criptex();
            tbResult.Text = criptex.Cript(tbPassword.Text.Trim());
            criptex.Dispose();
        }
    }
}
