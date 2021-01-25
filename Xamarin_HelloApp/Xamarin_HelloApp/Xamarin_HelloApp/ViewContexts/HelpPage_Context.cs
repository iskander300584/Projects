using Plugin.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace PilotMobile.ViewContexts
{
    public class HelpPage_Context : INotifyPropertyChanged
    {
        #region Поля класса

        /// <summary>
        /// Окно справки
        /// </summary>
        private ContentPage page;


        /// <summary>
        /// Коллекция имен файлов изображений
        /// </summary>
        private List<string> ImagesCollection = new List<string>();


        private byte currentImage = 0;
        /// <summary>
        /// Номер текущего изображения
        /// </summary>
        public byte CurrentImage
        {
            get => currentImage;
            private set
            {
                if(currentImage != value)
                {
                    currentImage = value;
                    OnPropertyChanged();

                    CheckNextAvaliable();
                }
            }
        }


        private string imageSourceName = string.Empty;
        /// <summary>
        /// Имя источника изображения
        /// </summary>
        public string ImageSourceName
        {
            get => imageSourceName;
            private set
            {
                if (imageSourceName != value)
                {
                    imageSourceName = value;
                    OnPropertyChanged();
                }
            }
        }


        private bool canNext = true;
        /// <summary>
        /// Доступен переход к следующему изображению
        /// </summary>
        public bool CanNext
        {
            get => canNext;
            private set
            {
                if(canNext != value)
                {
                    canNext = value;
                    OnPropertyChanged();
                }
            }
        }


        private string nextButtonText = "Дальше";
        /// <summary>
        /// Текст кнопки Дальше
        /// </summary>
        public string NextButtonText
        {
            get => nextButtonText;
            private set
            {
                if (nextButtonText != value)
                {
                    nextButtonText = value;
                    OnPropertyChanged();
                }
            }
        }


        private ICommand nextCommand = null;
        /// <summary>
        /// Команда Дальше
        /// </summary>
        public ICommand NextCommand
        {
            get => nextCommand;
        }


        private ICommand skipCommand = null;
        /// <summary>
        /// Команда Пропустить
        /// </summary>
        public ICommand SkipCommand
        {
            get => skipCommand;
        }

        #endregion


        /// <summary>
        /// Контекст данных окна справки
        /// </summary>
        /// <param name="page">окно справки</param>
        /// <param name="showOnlyUpdates">отображать только изменения</param>
        public HelpPage_Context(ContentPage page, bool showOnlyUpdates)
        {
            this.page = page;

            SetImageCollection(showOnlyUpdates);

            nextCommand = new Command(Next_Execute);
            skipCommand = new Command(Skip_Execute);
        }


        #region Методы класса

        /// <summary>
        /// Установить коллекцию изображений
        /// </summary>
        private void SetImageCollection(bool showOnlyUpdates)
        {
            if (!showOnlyUpdates)
            {
                // Отображать все подсказки
                ImagesCollection.Add("Screen_01.png");
                ImagesCollection.Add("tap.png");
            }
            else
            {
                // Отображать изменения
                ImagesCollection.Add("Screen_01.png");
            }

            ImageSourceName = ImagesCollection[CurrentImage];

            CheckNextAvaliable();
        }


        /// <summary>
        /// Проверка доступности выбора следующего изображения
        /// </summary>
        private void CheckNextAvaliable()
        {
            CanNext = (CurrentImage + 1 < ImagesCollection.Count);
            if(!CanNext)
            {
                NextButtonText = "Завершить";
            }
        }


        /// <summary>
        /// Выполнение команды Дальше
        /// </summary>
        private void Next_Execute()
        {
            if (CanNext)
            {
                //page.BackgroundImageSource = ImagesCollection[++CurrentImage];
                ImageSourceName = ImagesCollection[++CurrentImage];
            }
            else
                Skip_Execute();
        }


        /// <summary>
        /// Выполнение команды Пропустить
        /// </summary>
        private void Skip_Execute()
        {
            string _currVersion = Version.Plugin.CrossVersion.Current.Version;

            CrossSettings.Current.AddOrUpdateValue("helpVersion", _currVersion);

            page.Navigation.PopModalAsync(true);
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        #endregion
    }
}