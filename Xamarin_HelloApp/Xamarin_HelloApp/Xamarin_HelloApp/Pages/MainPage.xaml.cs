using System;
using Xamarin.Forms;
using Xamarin_HelloApp.ViewModels;
using Xamarin_HelloApp.AppContext;
using Xamarin_HelloApp.ViewContexts;
using PilotMobile.Pages;
using System.Threading.Tasks;
using PilotMobile.AppContext;
using PilotMobile.ViewModels;
using System.Threading;

namespace Xamarin_HelloApp
{
    /// <summary>
    /// Главное окно
    /// </summary>
    public partial class MainPage : ContentPage
    {
        #region Поля класса

        /// <summary>
        /// Контекст данных окна
        /// </summary>
        private MainPage_Context context;


        /// <summary>
        /// Окно карусели
        /// </summary>
        public MainCarrouselPage carrouselPage;

        #endregion


        /// <summary>
        /// Главное окно
        /// </summary>
        /// <param name="rootObject">головной объект для подчиненного окна</param>
        public MainPage(MainCarrouselPage carrousel, string url, IPilotObject rootObject = null)
        {
            InitializeComponent();

            carrouselPage = carrousel;

            try
            {
                if (Global.DALContext != null && Global.DALContext.IsInitialized && Global.CurrentPerson == null)
                {
                    Global.CurrentPerson = Global.DALContext.Repository.CurrentPerson();
                }

                //if (url != null && url != "")
                //{
                //    var res = DisplayMessage("URL", url, false);
                //}


                context = new MainPage_Context(this, rootObject, url);

                this.BindingContext = context;
            }
            catch (Exception ex)
            {
                DisplayMessage("Ошибка", ex.Message, false);
            }
        }


        #region Методы класса

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
        /// Вывести сообщение об ошибке
        /// </summary>
        /// <param name="message">текст сообщения об ошибке</param>
        /// <param name="caption">заголовок ошибки</param>
        /// <returns>возвращает TRUE, если необходимо отправить отчет об ошибке</returns>
        public async Task<bool> DisplayError(string message, string caption = "Ошибка")
        {
            return await DisplayAlert(caption, message + StringConstants.SendErrorMessage, StringConstants.Send, StringConstants.DontSend);
        }
        

        /// <summary>
        /// Нажатие кнопки Назад
        /// </summary>
        protected override bool OnBackButtonPressed()
        {
            if(context.Mode == PageMode.Slave)
            {
                if (context.UpCanExecute)
                    context.Up_Execute();
                else 
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        NavigationToMain();
                    });
            }
            else if (context.Parent != context.Root)
            {
                if (context.UpCanExecute)
                    context.Up_Execute();
                else if (context.HomeCanExecute)
                    context.Home_Execute();
                else if (context.BackVisible)
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        NavigationToMain();
                    });
            }
            else
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    bool result = await DisplayMessage("Внимание!", "Закрыть программу?", true);
                    if (result)
                        System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
                });
            }

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
        public void HandleModalPopping(object sender, ModalPoppingEventArgs e)
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
            else if(context.Mode == PageMode.Url && context.Items == null || context.Items.Count == 0)
            {
                try
                {
                    carrouselPage.CurrentPage = carrouselPage.Children[1];
                }
                catch { }

                App.Current.ModalPopping -= HandleModalPopping;

                // Открыть корневую структуру
                if (context.UrlItem == null)
                    context.GetRootObjects(true);
                // Открыть структуру головного объекта
                else
                    context.OpenParentStructure();
            }
        }


        /// <summary>
        /// Возврат к предыдущей странице
        /// </summary>
        public void NavigationToMain()
        {
            Navigation.PopModalAsync();
        }


        /// <summary>
        /// Копирование ссылки на объект
        /// </summary>
        private async void Copy_Link(object sender, EventArgs e)
        {
            try
            {
                var menuitem = sender as MenuItem;
                if (menuitem != null)
                {
                    IPilotObject pilotObject = menuitem.BindingContext as IPilotObject;

                    bool result = await Global.CreateLink(pilotObject.DObject);
                }
            }
            catch (Exception ex)
            {
                var res = await DisplayError(ex.Message);

                if (res)
                    await Global.SendErrorReport(ex);
            }
        }


        /// <summary>
        /// Запуск главного окна
        /// </summary>
        private void OnAppearing(object sender, EventArgs e)
        {
            if (Global.IsTrial && !Global.TrialMessageShown)
            {
                Thread thread = new Thread(TrialExit);
                thread.Start();
            }

            if (!context.FirstLaunch)
            {
                object url_temp = "";
                if (App.Current.Properties.TryGetValue("url", out url_temp))
                {
                    string url = (string)url_temp;
                    App.Current.Properties.Remove("url");

                    if (url != null && url != "")
                        context.GetRootObjects(false, url);
                }
            }
            else
                context.FirstLaunch = false;
        }


        /// <summary>
        /// Проверка окончания пробного периода
        /// </summary>
        private async void TrialExit()
        {
            double lastDays = (Global.TrialExitDate - DateTime.Today).TotalDays;

            Device.BeginInvokeOnMainThread(async () =>
            {
                Global.TrialMessageShown = true;
            });

            if (lastDays < 0)
            {
                var res = await DisplayMessage("Внимание!", "Срок действия пробной версии истек.\nОбратитесь в АСКОН для приобретения коммерческой версии", false);

                Device.BeginInvokeOnMainThread(async () =>
                {
                    Environment.Exit(0);
                });
            }
            else if (lastDays <= 14)
            {
                int value = (int)lastDays;
                string days = " дней";
                string last = " осталось ";
                if (value == 1)
                {
                    days = " день";
                    last = " остался ";
                }
                else if (value > 1 && value < 5)
                    days = " дня";

                var res = await DisplayMessage("Внимание!", "До окончания пробного периода" + last + value + days + ".\nОбратитесь в АСКОН для приобретения коммерческой версии", false);
            }
        }

        #endregion
    }
}