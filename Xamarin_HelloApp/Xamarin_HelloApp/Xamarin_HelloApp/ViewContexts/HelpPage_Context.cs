using Plugin.Settings;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;


namespace PilotMobile.ViewContexts
{
    /// <summary>
    /// Контекст данных окна справки
    /// </summary>
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

                    SetHintText();
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


        private string hintText = string.Empty;
        /// <summary>
        /// Текст счетчика подсказок
        /// </summary>
        public string HintText
        {
            get => hintText;
            private set
            {
                if (hintText != value)
                {
                    hintText = value;
                    OnPropertyChanged();
                }
            }
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

            SetHintText();
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
                ImagesCollection.Add(@"help_00_thanks.jpg"); // экран приветствия
                ImagesCollection.Add(@"help_01_update_data.jpg");
                ImagesCollection.Add(@"help_02_structure.jpg");
                ImagesCollection.Add(@"help_03_card_button.jpg");
                ImagesCollection.Add(@"help_04_home_button.jpg");
                ImagesCollection.Add(@"help_05_link.jpg");
                ImagesCollection.Add(@"help_06_search_button.jpg");
                ImagesCollection.Add(@"help_07_search.jpg");
                ImagesCollection.Add(@"help_08_tasks.jpg");
                ImagesCollection.Add(@"help_09_task_state.jpg");
                ImagesCollection.Add(@"help_10_task_documents.jpg");
                ImagesCollection.Add(@"help_11_document.jpg");
                ImagesCollection.Add(@"help_12_main_menu.jpg");
                ImagesCollection.Add(@"help_13_clear_cache.jpg");
            }
            else
            {
                // Отображать изменения
                ImagesCollection.Add(@"help_00_update.jpg"); // экран приветствия при просмотре обновлений
                ImagesCollection.Add(@"help_01_update_data.jpg");
                ImagesCollection.Add(@"help_02_structure.jpg");
                ImagesCollection.Add(@"help_03_card_button.jpg");
                ImagesCollection.Add(@"help_04_home_button.jpg");
                ImagesCollection.Add(@"help_05_link.jpg");
                ImagesCollection.Add(@"help_06_search_button.jpg");
                ImagesCollection.Add(@"help_07_search.jpg");
                ImagesCollection.Add(@"help_08_tasks.jpg");
                ImagesCollection.Add(@"help_09_task_state.jpg");
                ImagesCollection.Add(@"help_10_task_documents.jpg");
                ImagesCollection.Add(@"help_11_document.jpg");
                ImagesCollection.Add(@"help_12_main_menu.jpg");
                ImagesCollection.Add(@"help_13_clear_cache.jpg");
            }

            ImageSourceName = ImagesCollection[CurrentImage];

            CheckNextAvaliable();
        }


        /// <summary>
        /// Установка значения счетчика подсказок
        /// </summary>
        private void SetHintText()
        {
            HintText = $"Подсказка\n{CurrentImage + 1} / {ImagesCollection.Count}";
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
        public void Next_Execute()
        {
            if (CanNext)
            {
                ImageSourceName = ImagesCollection[++CurrentImage];
            }
            else
                Skip_Execute();
        }


        /// <summary>
        /// Возврат к предыдущей подсказке
        /// </summary>
        public void Previous_Execute()
        {
            if (CurrentImage > 0)
                ImageSourceName = ImagesCollection[--CurrentImage];
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