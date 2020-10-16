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
            XpsPage xps = new XpsPage(pilotItem);
            CardPage card = new CardPage(pilotItem);

            if(pilotItem is PilotTreeItem)
            {
                Children.Add(xps);
                Children.Add(card);
            }
            else
            {
                Children.Add(card);
                Children.Add(xps);
            }
            
            Children.Add(new DocsPage(pilotItem));
        }
    }
}