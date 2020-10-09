using System;
using System.Linq;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using Ascon.Pilot.DataClasses;
using Xamarin_HelloApp.ViewModels;
using Xamarin_HelloApp.AppContext;
using Xamarin_HelloApp.ViewContexts;
using PilotMobile.Pages;
using System.Threading;

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

                GetRootObjects();
            }

            this.BindingContext = context;
        }


        /// <summary>
        /// Получить список головных объектов
        /// </summary>
        private void GetRootObjects()
        {
            context.Items.Clear();

            context.Parent = null;

            AsyncGetChildren(null);
        }


        /// <summary>
        /// Асинхронный метод получения вложенных объектов
        /// </summary>
        /// <param name="dObject">объект Pilot</param>
        private void AsyncGetChildren(PilotTreeItem pilotItem)
        {
            Thread thread = new Thread(new ParameterizedThreadStart(GetChildren));
            thread.Start(pilotItem);
        }


        /// <summary>
        /// Метод получения вложенных объектов для параллельного потока
        /// </summary>
        /// <param name="pilotItem">объект Pilot</param>
        private void GetChildren(object pilotItem)
        {
            // Получение вложенных объектов
            if (pilotItem != null)
            {
                PilotTreeItem _item = (PilotTreeItem)pilotItem;

                foreach (DChild dchild in _item.DObject.Children)
                {
                    PilotTreeItem item = new PilotTreeItem(dchild, _item);
                    // item.HasAccess
                    if (item.VisibleName.Trim() != "" && !item.Type.IsSystem)
                        context.Items.Add(item);
                }
            }
            // Получение корневых объектов
            else
            {
                DObject root = Global.DALContext.Repository.GetObjects(new[] { DObject.RootId }).First();

                foreach (DChild dchild in root.Children)
                {
                    PilotTreeItem item = new PilotTreeItem(dchild, null);
                    if (item.VisibleName.Trim() != "" && !item.Type.IsSystem)
                        context.Items.Add(item);
                }
            }
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
                context.Items = new ObservableCollection<PilotTreeItem>();

                context.Parent = pilotItem;

                AsyncGetChildren(pilotItem);
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
            context.Items = new ObservableCollection<PilotTreeItem>();

            if (context.Parent != null)
            {
                context.Parent = context.Parent.Parent;

                AsyncGetChildren(context.Parent);

                /*foreach (DChild dchild in context.Parent.DObject.Children)
                {
                    PilotTreeItem item = new PilotTreeItem(dchild, context.Parent);
                    if (item.HasAccess)
                        context.Items.Add(item);
                }*/

                
            }
            else
            {
                GetRootObjects();
            }
        }


        /// <summary>
        /// Кнопка Домой
        /// </summary>
        private void Home_Click(object sender, EventArgs e)
        {
            GetRootObjects();
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