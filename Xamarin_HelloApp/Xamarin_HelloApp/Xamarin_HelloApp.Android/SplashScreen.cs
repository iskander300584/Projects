﻿using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Xamarin_HelloApp.Droid
{
    [Activity(Label = "Pilot-FLY DEMO", MainLauncher = true, Theme = "@style/SplashTheme", LaunchMode = LaunchMode.SingleInstance, NoHistory = true )]
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

            // Подключение службы уведомлений
            NotificationCenter.CreateNotificationChannel();

            // Задание портретной ориентации
            RequestedOrientation = ScreenOrientation.Portrait;

            // Получение ссылки на объект
            string url = "";
            if (this.Intent != null && this.Intent.Data != null)
                url = this.Intent.Data.ToString();

            Intent intent = new Intent(this, typeof(MainActivity));
            intent.PutExtra("url", url);

            // Сохранение ссылки в настройки, если приложение было запущено ранее
            if (App.Current != null)
            {
                object url_temp = "";
                if (App.Current.Properties.TryGetValue("url", out url_temp))
                {
                    App.Current.Properties["url"] = url;
                }
                else
                    App.Current.Properties.Add("url", url);
            }

            Thread.Sleep(1000);
            StartActivity(intent);

            NotificationCenter.NotifyNotificationTapped(Intent);
        }


        /// <summary>
        /// Обработка получения уведомления
        /// </summary>
        /// <param name="intent">уведомление</param>
        protected override void OnNewIntent(Intent intent)
        {
            NotificationCenter.NotifyNotificationTapped(intent);
            base.OnNewIntent(intent);
        }
    }
}