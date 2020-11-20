using System;
using Xamarin.Forms;
using Xamarin_HelloApp.ViewModels;
using Xamarin_HelloApp.AppContext;
using Xamarin_HelloApp.ViewContexts;
using PilotMobile.Pages;
using System.Threading.Tasks;
using PilotMobile.AppContext;
using PilotMobile.ViewModels;

namespace Xamarin_HelloApp
{
    /// <summary>
    /// Главное окно
    /// </summary>
    public partial class MainPage : ContentPage
    {
        /// <summary>
        /// Контекст данных окна
        /// </summary>
        private MainPage_Context context;


        /// <summary>
        /// Главное окно
        /// </summary>
        /// <param name="rootObject">головной объект для подчиненного окна</param>
        public MainPage(string url, IPilotObject rootObject = null)
        {
            InitializeComponent();

            if (Global.DALContext != null && Global.DALContext.IsInitialized && Global.CurrentPerson == null)
            {
                Global.CurrentPerson = Global.DALContext.Repository.CurrentPerson();
            }

            context = new MainPage_Context(this, rootObject);

            this.BindingContext = context;

            if (url != null)
                DisplayMessage("URL", "В ближайшее время функционал перехода по ссылке будет реализован =)" + (string)url, false);
        }


        /// <summary>
        /// Нажатие на элемент Pilot
        /// </summary>
        private void PilotItem_Tapped(object sender, ItemTappedEventArgs e)
        {
            PilotTreeItem pilotItem = e.Item as PilotTreeItem;

            // Отображение вложенных объектов
            if (!pilotItem.Type.IsDocument)
            {
                context.ItemTapped(pilotItem);
            }
            // Отображение окон документа
            else
            {
                Navigation.PushModalAsync(new DocumentCarrousel(pilotItem));
            }
        }


        /// <summary>
        /// Нажатие кнопки Карточка
        /// </summary>
        private void Card_Click(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new CardPage(context.Parent, null));
        }


        /// <summary>
        /// Получение выбранного пользователем пункта в главном меню
        /// </summary>
        public async Task<string> GetAction()
        {
            return await DisplayActionSheet(StringConstants.ActionChoose, StringConstants.Cancel, null, StringConstants.Authentificate, StringConstants.ClearCache, StringConstants.Exit);
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
        /// Нажатие кнопки Назад
        /// </summary>
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                bool result = await DisplayMessage("Внимание!", "Закрыть программу?", true);
                if (result)
                    System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
            });

            return true;
        }


        /// <summary>
        /// Нажатие кнопки Поиск
        /// </summary>
        private async void Search_Click(object sender, EventArgs e)
        {
            SearchQueryPage page = new SearchQueryPage(context.SearchContext);

            await Navigation.PushModalAsync(page, true);

            App.Current.ModalPopping += HandleModalPopping;
        }


        /// <summary>
        /// Перехват возврата из модальной страницы
        /// </summary>
        private void HandleModalPopping(object sender, ModalPoppingEventArgs e)
        {
            if(e.Modal is SearchQueryPage)
            {
                SearchQueryPage page = e.Modal as SearchQueryPage;

                if (page.SearchQuery != string.Empty)
                {
                    context.SearchContext = page.SearchContext;
                    context.DoSearch(page.SearchQuery);
                }

                App.Current.ModalPopping -= HandleModalPopping;
            }
        }


        /// <summary>
        /// Возврат к предыдущей странице
        /// </summary>
        public void NavigationToMain()
        {
            Navigation.PopModalAsync();
        }
    }
}