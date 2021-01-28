using Ascon.Pilot.DataClasses;
using PilotMobile.AppContext;
using PilotMobile.Pages;
using PilotMobile.ViewContexts;
using PilotMobile.ViewModels;
using Plugin.Toast;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin_HelloApp.AppContext;
using Xamarin_HelloApp.Models;
using Xamarin_HelloApp.Pages;
using Xamarin_HelloApp.ViewModels;


namespace Xamarin_HelloApp.ViewContexts
{
    /// <summary>
    /// Контекст данных главного окна
    /// </summary>
    class MainPage_Context : INotifyPropertyChanged
    {
        #region Поля класса

        /// <summary>
        /// Наименование приложения
        /// </summary>
        public string AppName
        {
            get => StringConstants.ApplicationName;
        }


        private PageMode mode = PageMode.View;
        /// <summary>
        /// Режим работы окна
        /// </summary>
        public PageMode Mode
        {
            get => mode;
            private set
            {
                if(mode != value)
                {
                    mode = value;
                    OnPropertyChanged();
                }
            }
        }


        private PageStatus status = PageStatus.Free;
        /// <summary>
        /// Состояние страницы
        /// </summary>
        public PageStatus Status
        {
            get => status;
            private set
            {
                if(status != value)
                {
                    status = value;
                    OnPropertyChanged();
                }
            }
        }


        /// <summary>
        /// Соответствующая страница
        /// </summary>
        private MainPage page;


        private ObservableCollection<IPilotObject> items = new ObservableCollection<IPilotObject>();
        /// <summary>
        /// Список отображаемых элементов
        /// </summary>
        public ObservableCollection<IPilotObject> Items
        {
            get => items;
            private set
            {
                if(items != value)
                {
                    items = value;
                    OnPropertyChanged();
                }
            }
        }


        private IPilotObject parent;
        /// <summary>
        /// Головной элемент для элементов отображаемого списка
        /// <para>NULL, если отображаются корневые элементы</para>
        /// </summary>
        public IPilotObject Parent
        {
            get => parent;
            set
            {
                if(parent != value)
                {
                    parent = value;
                    OnPropertyChanged();
                }
            }
        }
        

        private PilotTreeItem root;
        /// <summary>
        /// Корневой элемент дерева Pilot
        /// </summary>
        public PilotTreeItem Root
        {
            get => root;
        }


        /// <summary>
        /// Соединение установлено
        /// </summary>
        public bool IsConnected
        {
            get => Global.DALContext.IsInitialized;
        }


        private bool upCanExecute = false;
        /// <summary>
        /// Признак возможности нажатия кнопки Вверх
        /// </summary>
        public bool UpCanExecute
        {
            get => upCanExecute;
            private set
            {
                if(upCanExecute != value)
                {
                    upCanExecute = value;
                    OnPropertyChanged();
                }
            }
        }


        private bool homeCanExecute = false;
        /// <summary>
        /// Признак возможности нажатия кнопки Домой
        /// </summary>
        public bool HomeCanExecute
        {
            get => homeCanExecute;
            private set
            {
                if(homeCanExecute != value)
                {
                    homeCanExecute = value;
                    OnPropertyChanged();
                }
            }
        }


        private bool updateCanExecute = false;
        /// <summary>
        /// Признак возможности нажатия кнопки Обновить
        /// </summary>
        public bool UpdateCanExecute
        {
            get => updateCanExecute;
            private set
            {
                if(updateCanExecute != value)
                {
                    updateCanExecute = value;
                    OnPropertyChanged();
                }
            }
        }


        private ICommand upCommand;
        /// <summary>
        /// Команда Вверх
        /// </summary>
        public ICommand UpCommand
        {
            get => upCommand;
        }


        private ICommand homeCommand;
        /// <summary>
        /// Команда Домой
        /// </summary>
        public ICommand HomeCommand
        {
            get => homeCommand;
        }


        private ICommand updateCommand;
        /// <summary>
        /// Команда Обновить
        /// </summary>
        public ICommand UpdateCommand
        {
            get => updateCommand;
        }


        private ICommand mainMenuCommand;
        /// <summary>
        /// Команда Главное меню
        /// </summary>
        public ICommand MainMenuCommand
        {
            get => mainMenuCommand;
        }


        private ICommand backCommand;
        /// <summary>
        /// Команда Назад
        /// </summary>
        public ICommand BackCommand
        {
            get => backCommand;
        }


        private bool backVisible = false;
        /// <summary>
        /// Признак видимости кнопки Назад
        /// </summary>
        public bool BackVisible
        {
            get => backVisible;
        }


        private ICommand testCommand;
        /// <summary>
        /// Тестовая команда
        /// </summary>
        public ICommand TestCommand
        {
            get => testCommand;
        }


        private SearchQueryPage_Context searchContext = null;
        /// <summary>
        /// Контекст данных окна поиска
        /// </summary>
        public SearchQueryPage_Context SearchContext
        {
            get => searchContext;
            set
            {
                if(searchContext != value)
                {
                    searchContext = value;
                    OnPropertyChanged();
                }
            }
        }


        private bool searchVisible = true;
        /// <summary>
        /// Отображение кнопки Поиск
        /// </summary>
        public bool SearchVisible
        {
            get => searchVisible;
        }


        private DObject urlItem = null;
        /// <summary>
        /// Объект, открытый по ссылке
        /// </summary>
        public DObject UrlItem
        {
            get => urlItem;
        }


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


        private bool firstLaunch = true;
        /// <summary>
        /// Первый запуск приложения
        /// </summary>
        public bool FirstLaunch
        {
            get => firstLaunch;
            set
            {
                if (firstLaunch != value)
                    firstLaunch = value;
            }
        }


        #endregion


        /// <summary>
        /// Контекст данных главного окна
        /// </summary>
        /// <param name="page">соответствующая страница</param>
        /// <param name="rootObject">головной объект (для подчиненной страницы)</param>
        /// <param name="url">ссылка на объект</param>
        public MainPage_Context(MainPage page, IPilotObject rootObject, string url)
        {
            this.page = page;

            if(rootObject != null && rootObject is PilotTask)
            {
                root = new PilotTreeItem(rootObject.DObject, true);
                Parent = Root;
                Mode = PageMode.Slave;

                if (IsConnected)
                    GetTaskRootObjects();

                searchVisible = false;
                backVisible = true;
            }
            else if (IsConnected)
                GetRootObjects(false, url);

            upCommand = new Command(Up_Execute);
            homeCommand = new Command(Home_Execute);
            updateCommand = new Command(Update_Execute);
            mainMenuCommand = new Command(MainMenu_Execute);
            backCommand = new Command(Back_Execute);
            testCommand = new Command(TestCommand_Execute);

            Up_CanExecute();
        }


        #region Методы


        /// <summary>
        /// Получить список головных объектов
        /// </summary>
        /// <param name="update">обновить данные</param>
        /// <param name="url">ссылка на объект</param>
        public async void GetRootObjects(bool update = false, string url = "")
        {
            try
            {
                Exception exep = Global.Reconnect();
                if (exep != null)
                {
                    var res = await Global.DisplayError(page, exep.Message);

                    if (res)
                        await Global.SendErrorReport(exep);

                    return;
                }

                if (Mode != PageMode.Slave)
                {
                    DObject rootObj = Global.DALContext.Repository.GetObjects(new[] { DObject.RootId }).First();

                    root = new PilotTreeItem(rootObj);
                }

                Guid? guid;
                bool needGetRoot = false; // Признак необходимости получения головных объектов

                // Получение головных объектов
                if (url == null || url == "" || (guid = ParseURL(url)) == null)
                {
                    needGetRoot = true;
                }
                // Получение объекта по ссылке
                else
                {
                    DObject dObject = Global.DALContext.Repository.GetObjects(new Guid[] { (Guid)guid }).FirstOrDefault();

                    if (dObject != null)
                    {
                        PilotTreeItem item = new PilotTreeItem(dObject, true);

                        if (!item.DObject.InRootRecycleBin())
                        {
                            Mode = PageMode.Url;

                            // Получение структуры для объекта
                            if (!item.Type.IsDocument && !item.Type.IsTask && !item.Type.IsSystem && item.VisibleName != "")
                            {
                                try
                                {
                                    Parent = item;
                                    Items = Parent.Children;

                                    AsyncGetChildren(item);
                                }
                                catch (Exception ex)
                                {
                                    var res = await Global.DisplayError(page, ex.Message, "Ошибка доступа к объекту");

                                    if (res)
                                        await Global.SendErrorReport(ex);
                                }
                            }
                            // Открытие документа
                            else if (item.Type.IsDocument && item.Type.Name != TypeConstants.File)
                            {
                                try
                                {
                                    urlItem = item.DObject;

                                    App.Current.ModalPopping += page.HandleModalPopping;

                                    page.Navigation.PushModalAsync(new XpsPage(item));
                                    //page.Navigation.PushModalAsync(new DocumentCarrousel(item));
                                }
                                catch (Exception ex)
                                {
                                    var res = await Global.DisplayError(page, ex.Message, "Ошибка доступа к документу");

                                    if (res)
                                        await Global.SendErrorReport(ex);
                                }
                            }
                            // Открытие задания
                            else if (item.Type.IsTask)
                            {
                                try
                                {
                                    PilotTask task = new PilotTask(item.Guid);

                                    try
                                    {
                                        page.carrouselPage.CurrentPage = page.carrouselPage.Children[1];
                                    }
                                    catch { }

                                    App.Current.ModalPopping += page.HandleModalPopping;

                                    page.Navigation.PushModalAsync(new TaskCarrousel(task));
                                }
                                catch (Exception ex)
                                {
                                    var res = await Global.DisplayError(page, ex.Message, "Ошибка доступа к заданию");

                                    if (res)
                                        await Global.SendErrorReport(ex);
                                }
                            }
                            // Открытие файла TODO
                            else if (item.Type.IsSystem && item.Type.Name == TypeConstants.File)
                            {
                                try
                                {
                                    PilotTreeItem file = new PilotTreeItem(Global.DALContext.Repository.GetObjects(new Guid[] { item.DObject.ParentId }).First(), false);
                                    DObject _document = null;
                                    foreach (DRelation relation in file.DObject.Relations)
                                    {
                                        DObject _dObject = Global.DALContext.Repository.GetObjects(new Guid[] { relation.TargetId }).First();
                                        PType _type = TypeFabrique.GetType(_dObject.TypeId);
                                        if (_type.IsDocument && !_type.IsSystem)
                                        {
                                            _document = _dObject;
                                            break;
                                        }
                                    }

                                    if (_document != null)
                                    {
                                        PilotTreeItem document = new PilotTreeItem(Global.DALContext.Repository.GetObjects(new Guid[] { _document.Id }).First(), true);

                                        urlItem = document.DObject;

                                        App.Current.ModalPopping += page.HandleModalPopping;

                                        page.Navigation.PushModalAsync(new XpsPage(document));
                                        //page.Navigation.PushModalAsync(new DocumentCarrousel(document));
                                    }
                                    else
                                    {
                                        var result = Global.DisplayMessage(page, "Внимание!", "Ошибка доступа к документу", false);

                                        needGetRoot = true;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    var res = await Global.DisplayError(page, ex.Message, "Ошибка доступа к файлу");

                                    if (res)
                                        await Global.SendErrorReport(ex);
                                }
                            }
                            // 
                            //else if(item.VisibleName == "" && !item.Type.IsSystem)
                            //{
                            //    var result = page.DisplayMessage("Внимание!", "У Вас нет доступа к объекту", false);

                            //    needGetRoot = true;
                            //}
                            // Заглушка
                            else
                            {
                                var result = Global.DisplayMessage(page, "Внимание!", "Ссылка на объекты данного типа не поддерживается в мобильной версии", false);

                                needGetRoot = true;
                            }
                        }
                        else if (item.DObject.InRootRecycleBin() || item.DObject.IsForbidden())
                        {
                            var result = Global.DisplayMessage(page, "Внимание!", "Объект был удален", false);

                            needGetRoot = true;
                        }
                        else
                        {
                            var result = Global.DisplayMessage(page, "Внимание!", "У Вас нет доступа к указанному объекту", false);

                            needGetRoot = true;
                        }
                    }
                    else
                    {
                        needGetRoot = true;
                    }
                }

                // Получение головных объектов
                if (needGetRoot)
                {
                    Parent = Root;

                    Items = Parent.Children;

                    if (update || Parent.Children.Count == 0)
                    {
                        Parent.Children.Clear();
                        AsyncGetChildren(Parent);
                    }
                }

                Up_CanExecute();
            }
            catch (Exception ex)
            {
                var res = await Global.DisplayError(page, ex.Message);

                if (res)
                    await Global.SendErrorReport(ex);
            }
        }


        /// <summary>
        /// Получение объектов, вложенных в задание
        /// </summary>
        private async void GetTaskRootObjects()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                IsRefreshing = true;
            });

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

                Parent.Children.Clear();
                Items = Parent.Children;

                foreach (DRelation relation in Root.DObject.Relations)
                {
                    if (relation.TargetId == null)
                        continue;

                    DObject childObj = Global.DALContext.Repository.GetObjects(new Guid[] { relation.TargetId }).FirstOrDefault();

                    if (childObj != null)
                    {
                        PilotTreeItem child = new PilotTreeItem(childObj, true);

                        if (child.VisibleName != "")
                        {
                            child.SetParent(Root);
                            Root.Children.Add(child);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var res = await Global.DisplayError(page, ex.Message);

                if (res)
                    await Global.SendErrorReport(ex);
            }

            Device.BeginInvokeOnMainThread(async () =>
            {
                IsRefreshing = false;
                Status = PageStatus.Free;
            });
        }


        /// <summary>
        /// Асинхронный метод получения вложенных объектов
        /// </summary>
        /// <param name="dObject">объект Pilot</param>
        private async void AsyncGetChildren(IPilotObject pilotItem, IPilotObject childItem = null)
        {
            try
            {
                Thread thread = new Thread(new ParameterizedThreadStart(GetChildren));
                thread.Start(new GetChildrenParam(pilotItem, childItem));
            }
            catch (Exception ex)
            {
                var res = await Global.DisplayError(page, ex.Message);

                if (res)
                    await Global.SendErrorReport(ex);
            }
        }


        /// <summary>
        /// Внутренний класс для передачи данных в асинхронный метод
        /// </summary>
        internal class GetChildrenParam
        {
            /// <summary>
            /// Новый головной объект Pilot
            /// </summary>
            public IPilotObject PilotItem { get; private set; }

            /// <summary>
            /// Вложенный объект Pilot
            /// </summary>
            public IPilotObject ChildItem { get; private set; }

            /// <summary>
            /// Класс для передачи данных в асинхронный метод
            /// </summary>
            /// <param name="pilotItem">Новый головной объект Pilot</param>
            /// <param name="childItem">Вложенный объект Pilot</param>
            public GetChildrenParam(IPilotObject pilotItem, IPilotObject childItem)
            {
                PilotItem = pilotItem;
                ChildItem = childItem;
            }
        }


        /// <summary>
        /// Метод получения вложенных объектов для параллельного потока
        /// </summary>
        /// <param name="pilotItem">объект Pilot</param>
        private async void GetChildren(object getChildrenParam)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (!FirstLaunch)
                {
                    IsRefreshing = true;
                    Status = PageStatus.Busy;
                }
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

                GetChildrenParam param = (GetChildrenParam)getChildrenParam;
                PilotTreeItem _item = (PilotTreeItem)param.PilotItem;
                PilotTreeItem _child = (PilotTreeItem)param.ChildItem;

                foreach (DChild dchild in _item.DObject.Children)
                {
                    PilotTreeItem item = new PilotTreeItem(dchild, _item);
                    if (item.VisibleName.Trim() != "" && !item.Type.IsSystem)
                    {
                        if (_child != null && _child.Guid == item.Guid)
                        {
                            // Определение позиции для элемента
                            int index = GetPositionIndex(_item, _child);
                            
                            if (index < _item.Children.Count)
                                _item.Children.Insert(index, _child);
                            else
                                _item.Children.Add(_child);

                            _child.Parent = _item;

                        }
                        else
                        {
                            int index = GetPositionIndex(_item, item);
                            if (index < _item.Children.Count)
                                _item.Children.Insert(index, item);
                            else
                                _item.Children.Add(item);
                        }
                    }
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
        /// Получить сортировочный индекс вставляемого элемента
        /// </summary>
        /// <param name="parent">головной элемент</param>
        /// <param name="child">вставляемый элемент</param>
        private int GetPositionIndex(IPilotObject parent, IPilotObject child)
        {
            if (child.Type.IsTask)
                return parent.Children.Count;

            //int index = 0;
            //while (index < parent.Children.Count && ((!parent.Children[index].Type.IsDocument && child.Type.IsDocument) || (!parent.Children[index].Type.IsDocument && !child.Type.IsDocument && parent.Children[index].VisibleName.CompareTo(child.VisibleName) <= 0) || (parent.Children[index].Type.IsDocument && child.Type.IsDocument && parent.Children[index].VisibleName.CompareTo(child.VisibleName) <= 0)))
            //{
            //    index++;
            //}

            int index = 0;
            while (index < parent.Children.Count && (parent.Children[index].Type.MType.Sort < child.Type.MType.Sort || (parent.Children[index].Type.MType.Sort == child.Type.MType.Sort && parent.Children[index].VisibleName.CompareTo(child.VisibleName) <= 0)))
            {
                index++;
            }

            return index;
        }


        /// <summary>
        /// Получить GUID из ссылки
        /// </summary>
        /// <param name="url">ссылка</param>
        private Guid? ParseURL(string url)
        {
            int index = url.ToLower().IndexOf(@"url?id=");

            if (index == -1)
                return null;

            try
            {
                string _guidStr = url.Substring(index + 7);

                index = _guidStr.IndexOf(@"&v=");

                if(index != -1)
                    _guidStr = _guidStr.Substring(0, index);

                return new Guid(_guidStr);
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// Проверка возможности нажатия кнопкок Вверх, Домой, Обновить
        /// </summary>
        private void Up_CanExecute()
        {
            UpCanExecute = (Parent != null && Root != null && Parent != Root && Parent.DObject != null && ( Parent.Parent != null || Mode == PageMode.Url));

            HomeCanExecute = (UpCanExecute || Mode == PageMode.Search || Mode == PageMode.Url);

            UpdateCanExecute = (Mode != PageMode.Search);
        }


        /// <summary>
        /// Выбор объекта из списка
        /// </summary>
        /// <param name="item">элемент Pilot</param>
        public async void ItemTapped(PilotTreeItem item)
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

                Parent = item;

                Items = Parent.Children;

                if (Parent.Children.Count == 0)
                    AsyncGetChildren(Parent);

                Up_CanExecute();
            }
            catch (Exception ex)
            {
                var res = await Global.DisplayError(page, ex.Message);

                if (res)
                    await Global.SendErrorReport(ex);
            }
        }


        /// <summary>
        /// Нажатие кнопки Вверх
        /// </summary>
        public async void Up_Execute()
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

                if (Mode != PageMode.Url || Parent.Parent != null)
                {
                    Parent = Parent.Parent;

                    Items = Parent.Children;
                }
                else
                {
                    Guid parentGuid = Parent.DObject.ParentId;
                    IPilotObject current = Parent;

                    if (parentGuid != Root.Guid)
                    {
                        PilotTreeItem parent = new PilotTreeItem(Global.DALContext.Repository.GetObjects(new Guid[] { parentGuid }).FirstOrDefault(), true);

                        Parent = parent;
                        Items = Parent.Children;

                        if (Items.Count == 0)
                            AsyncGetChildren(parent, current);
                        else
                        {
                            IPilotObject child = Parent.Children.FirstOrDefault(c => c.Guid == current.Guid);
                            if (child != null)
                            {
                                child = current;
                                current.Parent = Parent;
                            }
                        }
                    }
                    // Получение корневых объектов
                    else
                    {
                        Mode = PageMode.View;

                        Parent = Root;

                        Items = Parent.Children;

                        if (Items.Count == 0)
                        {
                            AsyncGetChildren(Parent, current);
                        }
                        else
                        {
                            IPilotObject child = Parent.Children.FirstOrDefault(c => c.Guid == current.Guid);
                            if (child != null)
                            {
                                child = current;
                                current.Parent = Parent;
                            }
                        }
                    }
                }

                Up_CanExecute();
            }
            catch (Exception ex)
            {
                var res = await Global.DisplayError(page, ex.Message);

                if (res)
                    await Global.SendErrorReport(ex);
            }
        }


        /// <summary>
        /// Нажатие кнопки Домой
        /// </summary>
        public async void Home_Execute()
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

                Mode = PageMode.View;

                Parent = Root;

                Items = Parent.Children;

                if (Items.Count == 0)
                {
                    AsyncGetChildren(Parent);
                }

                Up_CanExecute();
            }
            catch (Exception ex)
            {
                var res = await Global.DisplayError(page, ex.Message);

                if (res)
                    await Global.SendErrorReport(ex);
            }
        }


        /// <summary>
        /// Нажатие кнопки Обновить
        /// </summary>
        public async void Update_Execute()
        {
            if (Status == PageStatus.Busy)
                return;

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

                Parent.Children.Clear();

                Status = PageStatus.Busy;

                Parent.UpdateObjectData();

                if (Mode == PageMode.View || Parent != Root)
                    AsyncGetChildren(Parent);
                else
                    GetTaskRootObjects();

                Up_CanExecute();
            }
            catch (Exception ex)
            {
                var res = await Global.DisplayError(page, ex.Message);

                if (res)
                    await Global.SendErrorReport(ex);
            }
        }


        /// <summary>
        /// Команда Назад
        /// </summary>
        private void Back_Execute()
        {
            page.NavigationToMain();
        }


        /// <summary>
        /// Нажатие кнопки Главное меню
        /// </summary>
        private async void MainMenu_Execute()
        {
            string action = await page.GetAction();

            switch(action)
            {
                case StringConstants.Authentificate:
                    if (Global.Localized != LocalizedVersion.Demo)
                        App.Current.MainPage = new AuthorizePage(true);
                    break;

                case StringConstants.ClearCache:
                    ClearCache();
                    break;

                case StringConstants.Help:
                    page.Navigation.PushModalAsync(new HelpPage());
                    break;

                case StringConstants.Exit:
                    Global.DALContext.Repository.Disconnect();
                    System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
                    break;
            }
        }


        /// <summary>
        /// Очистка кэша приложения
        /// </summary>
        private async void ClearCache()
        {
            string dir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/";          

            string[] files = Directory.GetFiles(dir);

            double size = 0;

            Exception exep = null;

            foreach(string fileName in files)
            {
                FileInfo fileInfo = new FileInfo(fileName);
                
                if(fileInfo.Extension.ToLower() == StringConstants.PDF || fileInfo.Extension.ToLower() == StringConstants.XPS)
                {
                    try
                    {
                        long _tempSize = fileInfo.Length;
                        File.Delete(fileName);
                        size += _tempSize;
                    }
                    catch(Exception ex)
                    {
                        if (exep == null)
                            exep = ex;
                        
                        
                    }
                }
            }

            if (exep != null)
            {
                var res = await Global.DisplayError(page, exep.Message, "Ошибка удаления файла");

                if (res)
                    await Global.SendErrorReport(exep);
            }
            else
            {
                string _size = String.Format("{0:#.#}", size / 1048576);
                if (_size.Trim() == "")
                    _size = "0";
                else if (_size[0] == '.' || _size[0] == ',')
                    _size = "0" + _size;

                CrossToastPopUp.Current.ShowToastSuccess(StringConstants.Free + _size + StringConstants.TotalSize);
            }
        }


        /// <summary>
        /// Выполнить поиск данных
        /// </summary>
        /// <param name="query">строка поиска</param>
        public async void DoSearch(string query)
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

                Mode = PageMode.Search;

                Parent = new PilotTreeItem();

                Items = Parent.Children;

                Thread thread = new Thread(new ParameterizedThreadStart(GetSearchResults));
                thread.Start(query);
            }
            catch (Exception ex)
            {
                var res = await Global.DisplayError(page, ex.Message);

                if (res)
                    await Global.SendErrorReport(ex);
            }
        }


        /// <summary>
        /// Получение результатов поиска
        /// </summary>
        /// <param name="queryString">запрос поиска</param>
        private async void GetSearchResults(object queryString)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                IsRefreshing = true;
                Status = PageStatus.Busy;
            });

            try
            {
                string query = (string)queryString;
                DSearchDefinition searchDefinition = new DSearchDefinition
                {
                    Id = Guid.NewGuid(),
                    Request =
                    {
                    MaxResults = 1000,
                    SearchKind = SearchKind.Custom,
                    SearchString = query,
                    SortDefinitions =
                    {
                        new DSortDefinition {
                            Ascending = false,
                            FieldName = SystemAttributes.CREATION_TIME
                        }
                    }
                    }
                };

                DSearchResult result = await Global.DALContext.Repository.Search(searchDefinition);

                if (result.Found == null)
                {
                    Up_CanExecute();

                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        IsRefreshing = false;
                        Status = PageStatus.Free;
                    });

                    return;
                }

                List<DObject> dObjects = Global.DALContext.Repository.GetObjects(result.Found.ToArray());

                foreach (DObject dObject in dObjects)
                {
                    PilotTreeItem pilotTreeItem = new PilotTreeItem(dObject, true);
                    if (pilotTreeItem.VisibleName != "")
                        Parent.Children.Add(pilotTreeItem);
                }
            }
            catch(Exception ex)
            {
                var res = await Global.DisplayError(page, ex.Message);

                if (res)
                    await Global.SendErrorReport(ex);
            }

            Device.BeginInvokeOnMainThread(async () =>
            {
                IsRefreshing = false;
                Status = PageStatus.Free;
            });

            Up_CanExecute();
        }


        /// <summary>
        /// Открыть структуру головного объекта
        /// <para>для документов, открытых по сслыке</para>
        /// </summary>
        public void OpenParentStructure()
        {
            string url = @"url?id=" + UrlItem.ParentId.ToString();
            urlItem = null;

            GetRootObjects(true, url);
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }


        #endregion


        /// <summary>
        /// Метод обработки тестовой команды
        /// </summary>
        private async void TestCommand_Execute()
        {
            #region Проверка нажатия Поделиться

            /*try
            {
                Plugin.Share.CrossShare.Current.Share(new Plugin.Share.Abstractions.ShareMessage { Url = "url" });
            }
            catch(Exception ex)
            {
                if(ex != null)
                { }
            }*/

            #endregion

            #region Проверка работоспособности отчета об ошибке
            /*try
            {
                Exception exception0 = new Exception("Ошибка уровня 0");

                Exception exception1 = new Exception("Ошибка уровня 1", exception0);

                Exception exception2 = new Exception("Ошибка уровня 2", exception1);
                exception2.Source = "Тестовый метод";

                throw exception2;
            }
            catch (Exception ex)
            {
                var res = await Global.DisplayError(page, ex.Message);

                if (res)
                    await Global.SendErrorReport(ex);
            }*/
            #endregion


            #region Проверка получения уведомлений

            /*var notifies = Global.DALContext.Repository.GetNotifies();
            if(notifies != null && notifies.Changed != null)
            {
                foreach(var notify in notifies.Changed)
                {

                }
            }*/

            //var _notifies = Global.DALContext.Repository.GetNotifies();
            //if(_notifies != null)
            //{
            //    foreach(var notify in _notifies)
            //    {
            //        NotifyResult res = new NotifyResult();
            //        DRule rule = Global.DALContext.Rules.FirstOrDefault(r => r.Id == notify.Item1);
            //        if (rule != null)
            //        {
            //            Global.DALContext.Repository.PrintChangeDetails(notify.Item2, rule, res);
            //            if(res.Result.Count != 0)
            //            {

            //            }
            //        }

            //        /*if (notify.Item2 != null)
            //        {
            //            foreach(var data in notify.Item2)
            //            {
            //                if(data != null)
            //                {

            //                }
            //            }
            //        }*/
            //    }
            //}

            #endregion
        }
    }
}