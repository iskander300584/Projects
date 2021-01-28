using PilotMobile.ViewContexts;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.DeviceOrientation;


namespace PilotMobile.Pages
{
    /// <summary>
    /// Окно справки
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HelpPage : ContentPage
    {
        /// <summary>
        /// Контекст данных окна справки
        /// </summary>
        private HelpPage_Context context;


        /// <summary>
        /// Окно справки
        /// </summary>
        /// <param name="showOnlyUpdates">отображать только информацию об изменениях</param>
        public HelpPage(bool showOnlyUpdates = false)
        {
            InitializeComponent();

            context = new HelpPage_Context(this, showOnlyUpdates);

            this.BindingContext = context;
        }

        #region Методы класса

        /// <summary>
        /// Загрузка окна справки
        /// </summary>
        private void OnAppearing(object sender, EventArgs e)
        {
            // Принудительная установка портретной ориентации
            CrossDeviceOrientation.Current.LockOrientation(Plugin.DeviceOrientation.Abstractions.DeviceOrientations.Portrait);
        }


        /// <summary>
        /// Закрытие окна справки
        /// </summary>
        private void OnDisappearing(object sender, EventArgs e)
        {
            // Разблокировка ориентации
            CrossDeviceOrientation.Current.UnlockOrientation();
        }


        /// <summary>
        /// Прокрутка изображений
        /// </summary>
        private void OnSwipe(object sender, SwipedEventArgs e)
        {
            if (e.Direction == SwipeDirection.Left)
                context.Next_Execute();
            else if (e.Direction == SwipeDirection.Right)
                context.Previous_Execute();
        }

        #endregion
    }
}