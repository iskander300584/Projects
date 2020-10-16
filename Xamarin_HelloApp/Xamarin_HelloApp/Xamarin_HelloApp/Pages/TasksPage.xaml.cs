using PilotMobile.ViewContexts;
using PilotMobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PilotMobile.Pages
{
    /// <summary>
    /// Окно списка заданий
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TasksPage : ContentPage
    {
        private TasksPage_Context context;

        /// <summary>
        /// Окно списка заданий
        /// </summary>
        public TasksPage()
        {
            InitializeComponent();

            context = new TasksPage_Context();

            this.BindingContext = context;
        }


        /// <summary>
        /// Нажатие на задание
        /// </summary>
        private void Task_Tapped(object sender, ItemTappedEventArgs e)
        {
            // Получение выбранного задания
            PilotTask task = e.Item as PilotTask;

            Navigation.PushModalAsync(new DocumentCarrousel(task));
        }
    }
}