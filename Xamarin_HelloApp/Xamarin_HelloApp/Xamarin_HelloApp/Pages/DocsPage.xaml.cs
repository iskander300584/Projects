using PilotMobile.ViewContexts;
using PilotMobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin_HelloApp.Pages
{
    /// <summary>
    /// Страница списка документов
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DocsPage : ContentPage
    {
        /// <summary>
        /// Контекст данных страницы списка документов
        /// </summary>
        private DocsPage_Context context;


        /// <summary>
        /// Страница списка документов
        /// </summary>
        /// <param name="pilotItem">объект Pilot</param>
        /// <param name="xpsPage">страница XPS</param>
        public DocsPage(IPilotObject pilotItem, XpsPage xpsPage)
        {
            InitializeComponent();

            context = new DocsPage_Context(pilotItem, this, xpsPage);

            this.BindingContext = context;
        }


        /// <summary>
        /// Нажат файл
        /// </summary>
        private void File_Tapped(object sender, ItemTappedEventArgs e)
        {
            // Получение выбранного файла
            PilotFile pilotFile = e.Item as PilotFile;

            context.ItemTapped(pilotFile);
        }


        /// <summary>
        /// Переход на основную страницу приложения
        /// </summary>
        public void NavigateToMainPage()
        {
            Navigation.PopModalAsync();
        }
    }
}