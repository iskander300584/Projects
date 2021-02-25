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
using Plugin.LocalNotification;

namespace Xamarin_HelloApp.Droid
{

    [Activity(Label = "Pilot-FLY Main", MainLauncher = false, Icon = "@drawable/pilot_icon", Theme = "@style/MainTheme", LaunchMode = LaunchMode.SingleTask, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            // Регистрация плагина для получения ориентации (кажется)
            Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity = this;

            base.OnCreate(savedInstanceState);

            // Установка портретной ориентации
            RequestedOrientation = ScreenOrientation.Portrait;

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            // Регистрация данных для отображения SVG
            CachedImageRenderer.Init(true);
            var ignore = typeof(SvgCachedImage);

            this.Window.AddFlags(WindowManagerFlags.Fullscreen);

            // Получение ссылки на объект
            string url = "";
            
            try
            {
                if (this.Intent != null)
                    url = this.Intent.GetStringExtra("url").ToString();
            }
            catch { }

            // Подключение службы уведомлений
            NotificationCenter.CreateNotificationChannel();

            LoadApplication(new App(url));

            NotificationCenter.NotifyNotificationTapped(Intent);
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }


        /// <summary>
        /// Обработка получения уведомления
        /// </summary>
        /// <param name="intent"></param>
        protected override void OnNewIntent(Intent intent)
        {
            NotificationCenter.NotifyNotificationTapped(intent);
            base.OnNewIntent(intent);
        }
    }
}