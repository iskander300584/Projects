using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using Xamarin_HelloApp.Models;
using Ascon.Pilot.DataClasses;
using Xamarin_HelloApp.ViewModels;
using Xamarin_HelloApp.AppContext;
using Xamarin_HelloApp.ViewContexts;
using Xamarin_HelloApp.Pages;
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

            DObject root = Global.DALContext.Repository.GetObjects(new[] { DObject.RootId }).First();

            context.Parent = null;

            AsyncGetChildren(root);

            //var objects = root.Children;

            //foreach (DChild dchild in objects)
            //{
            //    PilotTreeItem item = new PilotTreeItem(dchild, null);
            //    if (item.HasAccess && !item.Type.IsDocument && !item.Type.IsSystem)
            //        context.Items.Add(item);
            //}

            
        }


        private async void AsyncGetChildren(DObject dObject)
        {
            foreach (DChild dchild in dObject.Children)
            {
                await GetChild(dchild);

                //PilotTreeItem item = new PilotTreeItem(dchild, null);
                //if (item.HasAccess && !item.Type.IsDocument && !item.Type.IsSystem)
                //    context.Items.Add(item);
            }
        }


        private async Task<bool> GetChild(DChild dchild)
        {
            PilotTreeItem item = new PilotTreeItem(dchild, null);
            if (item.HasAccess && !item.Type.IsDocument && !item.Type.IsSystem)
                context.Items.Add(item);

            return true;
        }


        /// <summary>
        /// Нажатие на элемент Pilot
        /// </summary>
        private void PilotItem_Tapped(object sender, ItemTappedEventArgs e)
        {
            PilotTreeItem pilotItem = e.Item as PilotTreeItem;

            if (!pilotItem.Type.IsDocument)
            {
                context.Items = new ObservableCollection<PilotTreeItem>();

                //foreach (DChild dchild in pilotItem.DObject.Children)
                //{
                //    PilotTreeItem item = new PilotTreeItem(dchild, pilotItem);
                //    if (item.HasAccess)
                //        context.Items.Add(item);
                //}

                context.Parent = pilotItem;

                AsyncGetChildren(pilotItem.DObject);
            }
            else
            {
                //DFile file = null;
                //var snapshot = pilotItem.DObject.ActualFileSnapshot;

                //if(snapshot != null)
                //{
                //    file = snapshot.Files.FirstOrDefault();
                //}

                //Navigation.PushModalAsync(new XpsPage(file));

                //Navigation.PushModalAsync(new DocsPage(pilotItem.DObject));

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
                foreach (DChild dchild in context.Parent.DObject.Children)
                {
                    PilotTreeItem item = new PilotTreeItem(dchild, context.Parent);
                    if (item.HasAccess)
                        context.Items.Add(item);
                }

                context.Parent = context.Parent.Parent;
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