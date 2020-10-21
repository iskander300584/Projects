using PilotMobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin_HelloApp.Pages;
using Xamarin_HelloApp.ViewModels;

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
            //if (pilotItem is PilotTreeItem)
            //{
            //    Children.Add(new XpsPage(pilotItem));
            //    Children.Add(new CardPage(pilotItem));
            //}
            //else
            //{
            //    Children.Add(new CardPage(pilotItem));
            //    Children.Add(new XpsPage(pilotItem));
            //}

            Children.Add(new XpsPage(pilotItem));
            Children.Add(new CardPage(pilotItem));

            Children.Add(new DocsPage(pilotItem));
        }
    }
}