using PilotMobile.AppContext;
using PilotMobile.ViewContexts;
using PilotMobile.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin_HelloApp.AppContext;

namespace PilotMobile.Pages
{
    /// <summary>
    /// Окно списка заданий
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TasksPage : ContentPage
    {
        /// <summary>
        /// Контекст данных окна
        /// </summary>
        private TasksPage_Context context;


        /// <summary>
        /// Окно списка заданий
        /// </summary>
        public TasksPage()
        {
            InitializeComponent();

            context = new TasksPage_Context(this);

            this.BindingContext = context;
        }


        /// <summary>
        /// Нажатие на задание
        /// </summary>
        private void Task_Tapped(object sender, ItemTappedEventArgs e)
        {
            // Получение выбранного задания
            PilotTask task = e.Item as PilotTask;
            task.GetStateMachineData(this);

            Navigation.PushModalAsync(new TaskCarrousel(task));
        }


        /// <summary>
        /// Вывести сообщение
        /// </summary>
        /// <param name="caption">заголовок</param>
        /// <param name="message">текст сообщения</param>
        /// <param name="isQuestion">сообщение является вопросом</param>
        /// <returns>возвращает TRUE, если ответ "Да" или сообщение не является вопросом</returns>
        public async Task<bool> DisplayMessage(string caption, string message, bool isQuestion)
        {
            if (!isQuestion)
            {
                await DisplayAlert(caption, message, StringConstants.Ok);

                return true;
            }
            else
            {
                return await DisplayAlert(caption, message, StringConstants.Yes, StringConstants.No);
            }
        }


        /// <summary>
        /// Нажатие кнопки Назад
        /// </summary>
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                bool result = await DisplayMessage("Внимание!", "Закрыть программу?", true);
                if (result)
                    System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
            });

            return true;
        }


        /// <summary>
        /// Копирование ссылки на объект
        /// </summary>
        private async void Copy_Link(object sender, System.EventArgs e)
        {
            if (sender is MenuItem)
            {
                MenuItem menuitem = sender as MenuItem;
                if (menuitem != null)
                {
                    IPilotObject pilotObject = menuitem.BindingContext as IPilotObject;

                    bool result = await Global.CreateLink(pilotObject.DObject);
                }
            }
        }
    }
}