using PilotMobile.ViewContexts;
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
        /// <summary>
        /// Контекст данных страницы поиска
        /// </summary>
        private SearchQueryPage_Context context;


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
        public SearchQueryPage()
        {
            InitializeComponent();

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
    }
}