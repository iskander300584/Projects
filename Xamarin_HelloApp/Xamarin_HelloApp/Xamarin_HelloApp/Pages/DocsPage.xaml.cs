using Ascon.Pilot.DataClasses;
using PilotMobile.ViewContexts;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin_HelloApp.ViewModels;

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
        /// <param name="pilotTreeItem">объект Pilot</param>
        public DocsPage(PilotTreeItem pilotTreeItem)
        {
            InitializeComponent();

            context = new DocsPage_Context(pilotTreeItem, this);

            this.BindingContext = context;
        }


        /// <summary>
        /// Нажат файл
        /// </summary>
        private void File_Tapped(object sender, ItemTappedEventArgs e)
        {
            // Получение выбранного файла
            DFile dFile = e.Item as DFile;

            context.ItemTapped(dFile);
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