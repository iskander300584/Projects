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

namespace Xamarin_HelloApp
{
    
    public partial class MainPage : ContentPage
    {
        private ObservableCollection<string> Items = new ObservableCollection<string>();

        private Context context;

        private int iterator = 1;

        public MainPage()
        {
            InitializeComponent();

            try
            {
                context = new Context();
                context.Build();

                if (context.IsInitialized)
                    labelConnect.Text = "Connected";
            }
            catch { }

            if (context != null && context.IsInitialized)
            {
                DObject root = context.Repository.GetObjects(new[] { DObject.RootId}).First();

                var objects = root.Children;

                

                foreach (DChild dchild in objects)
                {
                    var type = context.Repository.GetType(dchild.TypeId);
                    
                    Items.Add(type.Title);
                }
            }
            else
            {
                for (int i = 0; i < 3; i++)
                    Items.Add(GetItem());
            }

            listView.ItemsSource = Items;

        }

        /// <summary>
        /// Получение элемента
        /// </summary>
        /// <returns></returns>
        private string GetItem()
        {
            return iterator++.ToString() + " - й элемент";
        }

        private void btn_Click(object sender, EventArgs e)
        {
            //Items.Add(GetItem());

            ///*try
            //{*/
            //    context = new Context();
            //    context.Build();

            //    if (context.IsInitialized)
            //        labelConnect.Text = "Connected";
            ///*}
            //catch(Exception ex)
            //{
            //    lblException.Text = ex.Message;
            //}*/
        }
    }
}