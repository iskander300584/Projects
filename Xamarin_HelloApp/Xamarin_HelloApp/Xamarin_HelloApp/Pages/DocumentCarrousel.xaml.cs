using PilotMobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin_HelloApp.Pages;


namespace PilotMobile.Pages
{
    /// <summary>
    /// Карусель окон документа
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DocumentCarrousel : CarouselPage
    {
        /// <summary>
        /// Карусель окон документа
        /// </summary>
        /// <param name="pilotItem">объект документ или задание в дереве Pilot</param>
        public DocumentCarrousel(IPilotObject pilotItem)
        {
            InitializeComponent();

            // Добавление окон предварительного просмотра, карточки и списка файлов
            XpsPage xpsPage = new XpsPage(pilotItem);
            Children.Add(xpsPage);
            Children.Add(new CardPage(pilotItem, xpsPage));
            Children.Add(new DocsPage(pilotItem, xpsPage));

            //Children.Add(new CardPage(pilotItem, null));
            //Children.Add(new DocsPage(pilotItem, null));
        }
    }
}