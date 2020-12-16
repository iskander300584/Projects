using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Xamarin_HelloApp.Droid
{
    [Activity(Label = "SplashScreen", MainLauncher = true, Theme = "@style/SplashTheme", NoHistory =true)]
    public class SplashScreen : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            Thread.Sleep(500);

            StartActivity(typeof(MainActivity));
        }
    }
}