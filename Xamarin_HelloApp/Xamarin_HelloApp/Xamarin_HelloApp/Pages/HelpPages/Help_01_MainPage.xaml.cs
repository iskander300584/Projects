using PilotMobile.ViewContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PilotMobile.Pages.HelpPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Help_01_MainPage : ContentPage
    {
        #region Поля класса
        /// <summary>
        /// Поток анимации
        /// </summary>
        private Thread _animationThread;


        /// <summary>
        /// Текущее положение руки по X
        /// </summary>
        private double _handX = 0;


        /// <summary>
        /// Текущее положение руки по Y
        /// </summary>
        private double _handY = 0;


        /// <summary>
        /// Пауза при работе с текстом
        /// </summary>
        private int _textSleep = 300;


        /// <summary>
        /// Короткая пауза
        /// </summary>
        private int _fastSleep = 600;


        /// <summary>
        /// Длинная пауза
        /// </summary>
        private int _slowSleep = 3200;


        private HelpPage_Context context;

        #endregion

        /// <summary>
        /// Справка для главного окна
        /// </summary>
        /// <param name="showOnlyUpdates">отображать только информацию об изменениях</param>
        public Help_01_MainPage(bool showOnlyUpdates = false)
        {
            InitializeComponent();

            context = new HelpPage_Context(this, showOnlyUpdates);

            this.BindingContext = context;
        }

        /*
        /// <summary>
        /// Воспроизведение анимации
        /// </summary>
        private async void AnimationStart()
        {
            do
            {
                MoveHand(200, 150, true);
                Thread.Sleep(_fastSleep);

                SetText("Для обновления данных проведите сверху вниз");
                ShowText();

                TapHand();
                MoveHand(_handX, _handY + 300);
                Thread.Sleep(_slowSleep);
                UntapHand();
                MoveHand(_handX, _handY - 300, true);
                Thread.Sleep(_fastSleep);
                TapHand();
                MoveHand(_handX, _handY + 300);
                Thread.Sleep(_slowSleep);
                UntapHand();

                HideText();


            }
            while (true);
        }*/


        /// <summary>
        /// Загрузка окна справки
        /// </summary>
        private void OnAppearing(object sender, EventArgs e)
        {
            //textLabel.MinimumWidthRequest = mainLayout.Width;
            //textLabel.WidthRequest = mainLayout.Width;

            //_animationThread = new Thread(AnimationStart);
            //_animationThread.Start();
        }


        /// <summary>
        /// Закрытие окна справки
        /// </summary>
        private void OnDisappearing(object sender, EventArgs e)
        {
            //_animationThread.Abort();
        }

        /*
        /// <summary>
        /// Смещение руки в абсолютных координатах
        /// </summary>
        /// <param name="x">координата X</param>
        /// <param name="y">координата Y</param>
        /// <param name="fast">быстрое перемещение</param>
        private void MoveHand(double x, double y, bool fast = false)
        {
            uint delay = (fast) ? (uint)500 : (uint)3000;

            Device.BeginInvokeOnMainThread(async () =>
            {
                await handImage.TranslateTo(x, y, delay);
            });

            _handX = x;
            _handY = y;
        }


        /// <summary>
        /// Имитировать нажатие
        /// </summary>
        private void TapHand()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await handImage.ScaleTo(0.9);
            });

            Thread.Sleep(_textSleep);
        }


        /// <summary>
        /// Имитировать отпускание
        /// </summary>
        private void UntapHand()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await handImage.ScaleTo(1.111);
            });

            Thread.Sleep(_textSleep);
        }

        
        /// <summary>
        /// Отобразить текст
        /// </summary>
        private void ShowText()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                textLabel.FadeTo(1);
            });

            Thread.Sleep(_textSleep);
        }


        /// <summary>
        /// Скрыть текст
        /// </summary>
        private void HideText()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                textLabel.FadeTo(0);
            });

            Thread.Sleep(_textSleep);
        }


        /// <summary>
        /// Задать текст
        /// </summary>
        /// <param name="text">текст</param>
        private void SetText(string text)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                textLabel.Text = text;
            });

            Thread.Sleep(10);
        }*/
    }
}