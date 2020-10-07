using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
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
        public AuthorizePage()
        {
            InitializeComponent();

            context = new AuthorizePage_Context(this);

            this.BindingContext = context;
        }


        /// <summary>
        /// Переход на основную страницу приложения
        /// </summary>
        public void NavigateToMainPage()
        {
            Navigation.PushModalAsync(new MainPage());
        }

        private void Connect_Click(object sender, EventArgs e)
        {
            context.CheckConnection();
        }
    }
}