using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin_HelloApp;

namespace PilotMobile.Pages
{
    /// <summary>
    /// Карусель главных окон
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainCarrouselPage : CarouselPage
    {
        /// <summary>
        /// Карусель главных окон
        /// </summary>
        public MainCarrouselPage()
        {
            InitializeComponent();

            Children.Add(new MainPage()); // Добавление главного окна
            Children.Add(new TasksPage()); // Добавление окна заданий
        }
    }
}