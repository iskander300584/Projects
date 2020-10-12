using Ascon.Pilot.DataClasses;
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
        /// <param name="pilotTreeItem">объект документ в дереве Pilot</param>
        public DocumentCarrousel(PilotTreeItem pilotTreeItem)
        {
            InitializeComponent();

            // Добавление окон предварительного просмотра и списка файлов
            Children.Add(new XpsPage(pilotTreeItem));
            Children.Add(new DocsPage(pilotTreeItem));
        }
    }
}