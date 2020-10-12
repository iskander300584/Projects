using Ascon.Pilot.DataClasses;
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
        /// <param name="dObject">объект документ</param>
        public DocumentCarrousel(DObject dObject)
        {
            InitializeComponent();

            // Добавление окон предварительного просмотра и списка файлов
            Children.Add(new XpsPage(dObject));
            Children.Add(new DocsPage(dObject));
        }
    }
}