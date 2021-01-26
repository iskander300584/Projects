using PilotMobile.AppContext;
using PilotMobile.ViewContexts;
using PilotMobile.ViewModels;
using System;
using System.Threading.Tasks;
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
        /// Вывести сообщение
        /// </summary>
        /// <param name="caption">заголовок</param>
        /// <param name="message">текст сообщения</param>
        /// <param name="isQuestion">сообщение является вопросом</param>
        /// <returns>возвращает TRUE, если ответ "Да" или сообщение не является вопросом</returns>
        public async Task<bool> DisplayMessage(string caption, string message, bool isQuestion)
        {
            if (!isQuestion)
            {
                await DisplayAlert(caption, message, StringConstants.Ok);

                return true;
            }
            else
            {
                return await DisplayAlert(caption, message, StringConstants.Yes, StringConstants.No);
            }
        }


        /// <summary>
        /// Получение выбранного пользователем состояния
        /// </summary>
        public async Task<string> GetStateAction(string[] states)
        {
            return await DisplayActionSheet(StringConstants.StateChoose, StringConstants.Cancel, null, states);
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
            bool result = await Global.CopyLink(context.PilotObject.DObject);
        }


        /// <summary>
        /// Поделиться ссылкой
        /// </summary>
        private async void ShareLink(object sender, EventArgs e)
        {
            bool result = await Global.ShareLink(context.PilotObject.DObject);
        }
    }
}