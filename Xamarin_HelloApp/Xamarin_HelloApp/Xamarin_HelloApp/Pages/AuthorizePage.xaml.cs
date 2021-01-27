using PilotMobile.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin_HelloApp.ViewContexts;


namespace Xamarin_HelloApp.Pages
{
    /// <summary>
    /// Окно авторизации
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AuthorizePage : ContentPage
    {
        /// <summary>
        /// Контекст данных окна авторизации
        /// </summary>
        private AuthorizePage_Context context;


        /// <summary>
        /// Окно авторизации
        /// </summary>
        /// <param name="reconnect">признак смены учетной записи</param>
        public AuthorizePage(bool reconnect = false)
        {
            InitializeComponent();

            context = new AuthorizePage_Context(this, reconnect);

            this.BindingContext = context;
        }


        /// <summary>
        /// Переход на основную страницу приложения
        /// </summary>
        public void NavigateToMainPage()
        {
            App.Current.MainPage = new MainCarrouselPage(null);
        }


        /// <summary>
        /// Активация индикатора
        /// </summary>
        /// <param name="value">значение активации</param>
        public void ActiveIndicator(bool value)
        {
            indicator.IsEnabled = value;
            indicator.IsRunning = value;
            indicator.IsVisible = value;
        }
    }
}