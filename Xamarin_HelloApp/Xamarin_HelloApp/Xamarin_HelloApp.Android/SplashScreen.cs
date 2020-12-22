using Android.App;
using Android.Content;
using Android.Content.PM;
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
    [Activity(Label = "Pilot-FLY", MainLauncher = true, Theme = "@style/SplashTheme", LaunchMode = LaunchMode.SingleTask, NoHistory = true )]
    [IntentFilter(new[] { Intent.ActionView },
        Categories = new[] {
        Intent.CategoryBrowsable, Intent.CategoryDefault},
        DataSchemes = new[] { "http", "https" },
        DataHost = "*",
        DataPathPrefix = "/url"
        )]
    public class SplashScreen : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            string url = "";
            if (this.Intent != null && this.Intent.Data != null)
                url = this.Intent.Data.ToString();

            Intent intent = new Intent(this, typeof(MainActivity));
            intent.PutExtra("url", url);
            StartActivity(intent);

            //StartActivity(typeof(MainActivity));
        }
    }
}