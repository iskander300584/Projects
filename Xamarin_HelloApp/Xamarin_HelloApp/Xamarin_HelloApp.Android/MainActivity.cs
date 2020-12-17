using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using FFImageLoading.Forms.Platform;
using FFImageLoading.Svg.Forms;
using Android.Content;

namespace Xamarin_HelloApp.Droid
{
    [Activity(Label = "Pilot-FLY", Icon = "@drawable/pilot_icon", Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    /*[IntentFilter (new[] { Intent.ActionView },
        Categories = new[] { 
        Intent.CategoryBrowsable, Intent.CategoryDefault},
        DataSchemes = new[] { "http", "https" },
        //DataHost = "ecm.ascon.ru",
        //DataPort = "5545",

        //DataScheme = ".*",
        //DataHost = ".*",
        //DataPort = ".*",
        //DataPathPrefix = "/url"

        DataHost = "*",
        DataPathPrefix = "/url"
       // DataPathPattern = ".*\\\\.*\\url.*"
        )]*/
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            CachedImageRenderer.Init(true);
            var ignore = typeof(SvgCachedImage);

            //string? url = null;
            //if(this.Intent != null && this.Intent.Data != null)
            //    url = this.Intent.Data.ToString();

            this.Window.AddFlags(WindowManagerFlags.Fullscreen);

            string url = "";
            
            try
            {
                url = this.Intent.GetStringExtra("url").ToString();
            }
            catch { }

            LoadApplication(new App(url));
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}