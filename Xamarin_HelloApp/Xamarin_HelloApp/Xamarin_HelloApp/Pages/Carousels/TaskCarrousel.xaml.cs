using PilotMobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin_HelloApp;

namespace PilotMobile.Pages
{
    /// <summary>
    /// Карусель окон задания
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaskCarrousel : CarouselPage
    {
        /// <summary>
        /// Карусель окон задания
        /// </summary>
        /// <param name="pilotItem">объект задание в дереве Pilot</param>
        public TaskCarrousel(IPilotObject pilotItem)
        {
            InitializeComponent();

            Children.Add(new CardPage(pilotItem, null));
            Children.Add(new MainPage(pilotItem));
        }
    }
}