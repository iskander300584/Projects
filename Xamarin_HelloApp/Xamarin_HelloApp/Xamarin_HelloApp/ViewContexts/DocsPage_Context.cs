using Ascon.Pilot.DataClasses;
using PilotMobile.AppContext;
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
using Xamarin_HelloApp.Models;
using Xamarin_HelloApp.Pages;
using Xamarin_HelloApp.ViewModels;

namespace PilotMobile.ViewContexts
{
    /// <summary>
    /// Контекст данных страницы списка файлов
    /// </summary>
    class DocsPage_Context : INotifyPropertyChanged
    {
        #region Поля класса


        private PageStatus status = PageStatus.Free;
        /// <summary>
        /// Состояние страницы
        /// </summary>
        public PageStatus Status
        {
            get => status;
            private set
            {
                if (status != value)
                {
                    status = value;
                    OnPropertyChanged();
                }
            }
        }


        /// <summary>
        /// Наименование приложения
        /// </summary>
        public string AppName
        {
            get => StringConstants.ApplicationName;
        }


        private ObservableCollection<PilotFile> items = new ObservableCollection<PilotFile>();
        /// <summary>
        /// Список элементов
        /// </summary>
        public ObservableCollection<PilotFile> Items
        {
            get => items;
        }


        private IPilotObject pilotItem;
        /// <summary>
        /// Объект Pilot
        /// </summary>
        public IPilotObject PilotItem
        {
            get => pilotItem;
        }


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


        private bool isRefreshing = false;
        /// <summary>
        /// Выполняется обновление данных
        /// </summary>
        public bool IsRefreshing
        {
            get => isRefreshing;
            set
            {
                if(isRefreshing != value)
                {
                    isRefreshing = value;
                    OnPropertyChanged();
                }
            }
        }


        // костыль - исключение системных файлов
        private Regex regex1 = new Regex(@"pilotthumbnail$"); 
        private Regex regex2 = new Regex(@"^annotation");
        private Regex regex3 = new Regex(@"^note_chat_message");
        private Regex regex4 = new Regex(@"pilottextlabels");


        #endregion


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


        #region Методы класса


        /// <summary>
        /// Получение списка файлов
        /// </summary>
        private void GetFiles()
        {
            if (Status == PageStatus.Busy)
                return;

            Items.Clear();

            Thread thread = new Thread(AsyncGetFiles);
            thread.Start();
        }


        /// <summary>
        /// Метод получения списка фалов в отдельном потоке
        /// </summary>
        private void AsyncGetFiles()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                IsRefreshing = true;
                Status = PageStatus.Busy;
            });

            try
            {
                Exception ex = Global.Reconnect();
                if (ex != null)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        var res = await Global.DisplayError(page, ex.Message);

                        if (res)
                            await Global.SendErrorReport(ex);
                    });

                    return;
                }

                // Получение списка файлов для документа
                if (pilotItem is PilotTreeItem)
                {
                    GetFilesForItem();
                }
                // Получение списка файлов для задачи
                else if (pilotItem is PilotTask)
                {
                    GetFilesForTask();
                }
            }
            catch (Exception ex)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    var res = await Global.DisplayError(page, ex.Message);

                    if (res)
                        await Global.SendErrorReport(ex);
                });
            }

            Device.BeginInvokeOnMainThread(async () =>
            {
                IsRefreshing = false;
                Status = PageStatus.Free;
            });
        }


        /// <summary>
        /// Получение списка файлов для документа
        /// </summary>
        private void GetFilesForItem()
        {
            foreach (DChild dChild in pilotItem.DObject.Children)
            {
                DObject child = Global.DALContext.Repository.GetObjects(new Guid[] { dChild.ObjectId }).FirstOrDefault();

                if (child != null)
                {
                    foreach (DFile file in child.ActualFileSnapshot.Files)
                    {
                        AddFile(file);
                    }
                }
            }
        }


        /// <summary>
        /// Получение списка файлов для задания
        /// </summary>
        private void GetFilesForTask()
        {
            // получение ссылки на документ XPS
            foreach (DRelation relation in pilotItem.DObject.Relations)
            {
                if (relation.TargetId == null)
                    continue;

                DObject xps = Global.DALContext.Repository.GetObjects(new Guid[] { relation.TargetId }).FirstOrDefault();

                if (xps == null || xps.Children == null)
                    continue;

                PType type = TypeFabrique.GetType(xps.TypeId);

                // Проверка, что связанный объект является документом
                if (type.IsDocument)
                {
                    // получение вложенных файлов
                    foreach (DChild dChild in xps.Children)
                    {
                        DObject child = Global.DALContext.Repository.GetObjects(new Guid[] { dChild.ObjectId }).FirstOrDefault();

                        if (child != null)
                        {
                            foreach (DFile file in child.ActualFileSnapshot.Files)
                            {
                                AddFile(file);
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Добавление подходящих файлов в общий список
        /// </summary>
        /// <param name="file">файл</param>
        private void AddFile(DFile file)
        {
            string fName = file.Name.ToLower();
            // Проверка, что файл не является системным
            if (!regex1.IsMatch(fName) && !regex2.IsMatch(fName) && !regex3.IsMatch(fName) && !regex4.IsMatch(fName))
            {
                PilotFile _file = new PilotFile(file);

                int index = GetPositionIndex(_file);

                if (index < Items.Count)
                    Items.Insert(index, _file);
                else
                    Items.Add(new PilotFile(file));
            }
        }


        /// <summary>
        /// Получить сортировочный индекс добавляемого файла
        /// </summary>
        /// <param name="child">добавляемый файл</param>
        private int GetPositionIndex(PilotFile child)
        {
            int index = 0;
            while (index < Items.Count && Items[index].FileName.CompareTo(child.FileName) <= 0)
            {
                index++;
            }

            return index;
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
        /// <param name="pilotFile">файл в Pilot</param>
        public async void ItemTapped(PilotFile pilotFile)
        {
            try
            {
                Exception ex = Global.Reconnect();
                if (ex != null)
                {
                    var res = await Global.DisplayError(page, ex.Message);

                    if (res)
                        await Global.SendErrorReport(ex);

                    return;
                }

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
            catch (Exception ex)
            {
                var res = await Global.DisplayError(page, ex.Message);

                if (res)
                    await Global.SendErrorReport(ex);
            }
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