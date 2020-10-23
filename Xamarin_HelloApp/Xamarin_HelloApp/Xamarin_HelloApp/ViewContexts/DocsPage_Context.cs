using Ascon.Pilot.DataClasses;
using PilotMobile.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
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
        private ObservableCollection<PilotFile> items = new ObservableCollection<PilotFile>();
        /// <summary>
        /// Список элементов
        /// </summary>
        public ObservableCollection<PilotFile> Items
        {
            get => items;
        }


        /// <summary>
        /// Объект Pilot
        /// </summary>
        private IPilotObject pilotItem;


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
        /// Страница XPS
        /// </summary>
        private XpsPage xpsPage;


        /// <summary>
        /// Контекст данных страницы списка файлов
        /// </summary>
        /// <param name="pilotItem">объект Pilot</param>
        /// <param name="page">страница списка документов</param>
        public DocsPage_Context(IPilotObject pilotItem, DocsPage page, XpsPage xpsPage)
        {
            this.pilotItem = pilotItem;
            this.page = page;
            this.xpsPage = xpsPage;

            updateCommand = new Command(GetFiles);
            upCommand = new Command(Up_Execute);

            items = pilotItem.Files;

            if (items.Count == 0)
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
            // Получение списка файлов для документа
            if (pilotItem is PilotTreeItem)
            {
                foreach (DChild dChild in pilotItem.DObject.Children)
                {
                    DObject child = Global.DALContext.Repository.GetObjects(new Guid[] { dChild.ObjectId }).FirstOrDefault();

                    if (child != null)
                    {
                        foreach (DFile file in child.ActualFileSnapshot.Files)
                            Items.Add(new PilotFile(file));
                    }
                }
            }
            // Получение списка файлов для задачи
            else if (pilotItem is PilotTask)
            {
                // получение ссылки на документ XPS
                DRelation relation = pilotItem.DObject.Relations.FirstOrDefault();
                if (relation.TargetId == null)
                    return;
                
                DObject xps = Global.DALContext.Repository.GetObjects(new Guid[] { relation.TargetId }).FirstOrDefault();
                
                if (xps == null || xps.Children == null)
                    return;

                // получение вложенных файлов
                foreach (DChild dChild in xps.Children)
                {
                    DObject child = Global.DALContext.Repository.GetObjects(new Guid[] { dChild.ObjectId }).FirstOrDefault();

                    if (child != null)
                    {
                        Regex regex = new Regex(@"pilotthumbnail$"); // костыль - исключение системных файлов

                        foreach (DFile file in child.ActualFileSnapshot.Files)
                        {
                            if (!regex.IsMatch(file.Name.ToLower()))
                                Items.Add(new PilotFile(file));
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Команда Наверх
        /// </summary>
        private void Up_Execute()
        {
            if (xpsPage != null)
                xpsPage.UnLoadDocument(true);

            page.NavigateToMainPage();
        }


        /// <summary>
        /// Нажатие на файл
        /// </summary>
        /// <param name="pilotFile">файл в Pilot</param>
        public async void ItemTapped(PilotFile pilotFile)
        {
            // Проверка прав доступа
            if (await (Global.GetFilesPermissions()) != true)
                return;

            // Получение файла
            byte[] array = Global.DALContext.Repository.GetFileChunk(pilotFile.DFile.Body.Id, 0, (int)pilotFile.DFile.Body.Size);

            string fileName = Path.Combine(@"/storage/emulated/0/Download", pilotFile.DFile.Name);

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