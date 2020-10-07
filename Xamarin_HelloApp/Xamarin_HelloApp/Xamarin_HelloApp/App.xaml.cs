using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin_HelloApp.Pages;

namespace Xamarin_HelloApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AuthorizePage();

            //MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
