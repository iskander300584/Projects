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

namespace Xamarin_HelloApp
{
    
    public partial class MainPage : ContentPage
    {
        private ObservableCollection<PilotTreeItem> Items = new ObservableCollection<PilotTreeItem>();

        private int iterator = 1;

        private PilotTreeItem parent;

        public MainPage()
        {
            InitializeComponent();

            try
            {
                Global.DALContext = new Context();
                Global.DALContext.Build();

                if (Global.DALContext.IsInitialized)
                    labelConnect.Text = "Подключено";
            }
            catch { }

            if (Global.DALContext != null && Global.DALContext.IsInitialized)
            {
                Global.CurrentPerson = Global.DALContext.Repository.CurrentPerson();

                DObject root = Global.DALContext.Repository.GetObjects(new[] { DObject.RootId}).First();              

                var objects = root.Children;

                foreach (DChild dchild in objects)
                {
                    PilotTreeItem item = new PilotTreeItem(dchild, null);
                    if (item.HasAccess)
                        Items.Add(item);
                }
            }

            parent = null;

            listView.ItemsSource = Items;

        }

        /// <summary>
        /// Нажатие на элемент Pilot
        /// </summary>
        private void PilotItem_Tapped(object sender, ItemTappedEventArgs e)
        {
            listView.ItemsSource = null;

            PilotTreeItem pilotItem = e.Item as PilotTreeItem;

            Items = new ObservableCollection<PilotTreeItem>();

            foreach (DChild dchild in pilotItem.DObject.Children)
            {
                PilotTreeItem item = new PilotTreeItem(dchild, pilotItem);
                if (item.HasAccess)
                    Items.Add(item);
            }

            parent = pilotItem;

            listView.ItemsSource = Items;
        }

        /// <summary>
        /// Кнопка Вверх
        /// </summary>
        private void Up_Click(object sender, EventArgs e)
        {
            listView.ItemsSource = null;

            Items = new ObservableCollection<PilotTreeItem>();

            if (parent != null)
            {
                foreach (DChild dchild in parent.DObject.Children)
                {
                    PilotTreeItem item = new PilotTreeItem(dchild, parent);
                    if (item.HasAccess)
                        Items.Add(item);
                }

                parent = parent.Parent;
            }
            else
            {
                DObject root = Global.DALContext.Repository.GetObjects(new[] { DObject.RootId }).First();

                var objects = root.Children;

                foreach (DChild dchild in objects)
                {
                    PilotTreeItem item = new PilotTreeItem(dchild, null);
                    if (item.HasAccess)
                        Items.Add(item);
                }

                parent = null;
            }

            listView.ItemsSource = Items;
        }

        /// <summary>
        /// Кнопка Домой
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Home_Click(object sender, EventArgs e)
        {
            listView.ItemsSource = null;

            Items = new ObservableCollection<PilotTreeItem>();

            DObject root = Global.DALContext.Repository.GetObjects(new[] { DObject.RootId }).First();

            var objects = root.Children;

            foreach (DChild dchild in objects)
            {
                PilotTreeItem item = new PilotTreeItem(dchild, null);
                if (item.HasAccess)
                    Items.Add(item);
            }

            parent = null;

            listView.ItemsSource = Items;
        }
    }
}