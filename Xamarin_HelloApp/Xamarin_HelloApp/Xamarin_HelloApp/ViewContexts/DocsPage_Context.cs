using Ascon.Pilot.DataClasses;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin_HelloApp.AppContext;
using Xamarin_HelloApp.Pages;
using Xamarin_HelloApp.ViewModels;

namespace PilotMobile.ViewContexts
{
    /// <summary>
    /// Контекст данных страницы списка файлов
    /// </summary>
    class DocsPage_Context : INotifyPropertyChanged
    {
        private ObservableCollection<DFile> items = new ObservableCollection<DFile>();
        /// <summary>
        /// Список элементов
        /// </summary>
        public ObservableCollection<DFile> Items
        {
            get => items;
        }


        /// <summary>
        /// Объект Pilot
        /// </summary>
        private PilotTreeItem pilotTreeItem;


        /// <summary>
        /// Страница списка файлов
        /// </summary>
        private DocsPage page;


        private ICommand updateCommand;
        /// <summary>
        /// Команда обновления данных
        /// </summary>
        public ICommand UpdateCommand
        {
            get => updateCommand;
        }


        private ICommand upCommand;
        /// <summary>
        /// Команда Наверх
        /// </summary>
        public ICommand UpCommand
        {
            get => upCommand;
        }


        /// <summary>
        /// Контекст данных страницы списка файлов
        /// </summary>
        /// <param name="pilotTreeItem">объект Pilot</param>
        /// <param name="page">страница списка документов</param>
        public DocsPage_Context(PilotTreeItem pilotTreeItem, DocsPage page)
        {
            this.pilotTreeItem = pilotTreeItem;
            this.page = page;

            updateCommand = new Command(GetFiles);
            upCommand = new Command(Up_Execute);

            GetFiles();
        }


        /// <summary>
        /// Получение списка файлов
        /// </summary>
        private void GetFiles()
        {
            Items.Clear();

            Thread thread = new Thread(AsyncGetFiles);
            thread.Start();
        }


        /// <summary>
        /// Метод получения списка фалов в отдельном потоке
        /// </summary>
        private void AsyncGetFiles()
        {
            foreach (DChild dChild in pilotTreeItem.DObject.Children)
            {
                DObject child = Global.DALContext.Repository.GetObjects(new Guid[] { dChild.ObjectId }).FirstOrDefault();

                if (child != null)
                {
                    DFile file = child.ActualFileSnapshot.Files.FirstOrDefault();
                    if (file != null)
                        Items.Add(file);
                }
            }
        }


        /// <summary>
        /// Команда Наверх
        /// </summary>
        private void Up_Execute()
        {
            page.NavigateToMainPage();
        }


        /// <summary>
        /// Нажатие на файл
        /// </summary>
        /// <param name="dFile">файл в Pilot</param>
        public async void ItemTapped(DFile dFile)
        {
            // Проверка прав доступа
            if (await (Global.GetFilesPermissions()) != true)
                return;

            // Получение файла
            byte[] array = Global.DALContext.Repository.GetFileChunk(dFile.Body.Id, 0, (int)dFile.Body.Size);

            string fileName = Path.Combine(@"/storage/emulated/0/Download", dFile.Name);

            File.WriteAllBytes(fileName, array);

            // Открытие файла средствами ОС
            if (File.Exists(fileName))
            {
                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(fileName)
                });
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}