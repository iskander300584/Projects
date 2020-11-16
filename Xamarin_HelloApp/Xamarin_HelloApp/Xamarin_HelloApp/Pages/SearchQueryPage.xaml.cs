using PilotMobile.AppContext;
using PilotMobile.ViewContexts;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PilotMobile.Pages
{
    /// <summary>
    /// Страница поиска
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchQueryPage : ContentPage
    {
        private SearchQueryPage_Context context;
        /// <summary>
        /// Контекст данных страницы поиска
        /// </summary>
        public SearchQueryPage_Context SearchContext
        {
            get => context;
        }
        

        /// <summary>
        /// Строка поискового запроса
        /// </summary>
        public string SearchQuery
        {
            get => context.Query;
        }


        /// <summary>
        /// Страница поиска
        /// </summary>
        /// <param name="searchContext">контекст поиска</param>
        public SearchQueryPage(SearchQueryPage_Context searchContext)
        {
            InitializeComponent();

            if (searchContext != null)
            {
                context = searchContext;
                context.SetPage(this);
            }
            else
                context = new SearchQueryPage_Context(this);

            this.BindingContext = context;
        }


        /// <summary>
        /// Возврат к предыдущей странице
        /// </summary>
        public void NavigationToMain()
        {
            Navigation.PopModalAsync();
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
    }
}