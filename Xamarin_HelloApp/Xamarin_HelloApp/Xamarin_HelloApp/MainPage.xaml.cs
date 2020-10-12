using System;
using Xamarin.Forms;
using Xamarin_HelloApp.ViewModels;
using Xamarin_HelloApp.AppContext;
using Xamarin_HelloApp.ViewContexts;
using PilotMobile.Pages;

namespace Xamarin_HelloApp
{
    /// <summary>
    /// Главное окно
    /// </summary>
    public partial class MainPage : ContentPage
    {
        /// <summary>
        /// Контекст данных окна
        /// </summary>
        private MainPage_Context context = new MainPage_Context();


        /// <summary>
        /// Главное окно
        /// </summary>
        public MainPage()
        {
            InitializeComponent();

            if (Global.DALContext != null && Global.DALContext.IsInitialized)
            {
                Global.CurrentPerson = Global.DALContext.Repository.CurrentPerson();
            }

            this.BindingContext = context;
        }


        /// <summary>
        /// Нажатие на элемент Pilot
        /// </summary>
        private void PilotItem_Tapped(object sender, ItemTappedEventArgs e)
        {
            PilotTreeItem pilotItem = e.Item as PilotTreeItem;

            // Отображение вложенных объектов
            if (!pilotItem.Type.IsDocument)
            {
                context.ItemTapped(pilotItem);
            }
            // Отображение окон документа
            else
            {
                Navigation.PushModalAsync(new DocumentCarrousel(pilotItem.DObject));
            }
        }


        /// <summary>
        /// Кнопка Вверх
        /// </summary>
        private void Up_Click(object sender, EventArgs e)
        {
            context.Up_Execute();
        }


        /// <summary>
        /// Кнопка Домой
        /// </summary>
        private void Home_Click(object sender, EventArgs e)
        {
            context.Home_Execute();
        }


        /// <summary>
        /// Кнопка Список документов
        /// </summary>
        private void Docs_Click(object sender, EventArgs e)
        {
            if (context.Parent == null)
                return;
        }
    }
}