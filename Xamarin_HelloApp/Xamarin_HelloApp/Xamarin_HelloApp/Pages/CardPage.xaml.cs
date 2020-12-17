using PilotMobile.ViewContexts;
using PilotMobile.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin_HelloApp.AppContext;
using Xamarin_HelloApp.Pages;

namespace PilotMobile.Pages
{
    /// <summary>
    /// Страница карточки объекта
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CardPage : ContentPage
    {
        /// <summary>
        /// Контекст данных страницы
        /// </summary>
        private CardPage_Context context;


        /// <summary>
        /// Страница карточки объекта
        /// </summary>
        /// <param name="pilotItem">элемент Pilot</param>
        public CardPage(IPilotObject pilotItem, XpsPage xpsPage)
        {
            InitializeComponent();

            context = new CardPage_Context(pilotItem, this, xpsPage);

            this.BindingContext = context;
        }


        /// <summary>
        /// Переход на основную страницу приложения
        /// </summary>
        public void NavigateToMainPage()
        {
            Navigation.PopModalAsync();
        }


        /// <summary>
        /// Копирование ссылки на объект
        /// </summary>
        private async void CopyLink(object sender, EventArgs e)
        {
            bool result = await Global.CreateLink(context.PilotObject.DObject);
        }
    }
}