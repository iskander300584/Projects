using System.Windows;
using System.Windows.Input;


namespace KeyGenerator
{
    /// <summary>
    /// Окно генератора ключа
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Контекст данных окна
        /// </summary>
        Context context = new Context();

        private static RoutedUICommand _verify = new RoutedUICommand("Проверить", "Verify", typeof(MainWindow));
        /// <summary>
        /// Команда проверки подключения
        /// </summary>
        public static RoutedUICommand Verify
        {
            get => _verify;
        }

        private static RoutedUICommand _generate = new RoutedUICommand("Генерировать", "Generate", typeof(MainWindow));
        /// <summary>
        /// Команда генерирования ключа
        /// </summary>
        public static RoutedUICommand Generate
        {
            get => _generate;
        }


        /// <summary>
        /// Окно генератора ключа
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = context;
        }

        /// <summary>
        /// Проверка возможности проверки подключения
        /// </summary>
        private void Verify_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (context != null && !context.IsVerified && context.Server.Trim() != "" && context.DB.Trim() != "" && context.Password.Trim() != "");
        }

        /// <summary>
        /// Проверка подключения
        /// </summary>
        private void Verify_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            context.DoVerify();
        }

        /// <summary>
        /// Проверка возможности генерации ключа
        /// </summary>
        private void Generate_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (context != null && context.IsVerified);
        }

        /// <summary>
        /// Генерация ключа
        /// </summary>
        private void Generate_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            context.DoGenerateFile();
        }
    }
}