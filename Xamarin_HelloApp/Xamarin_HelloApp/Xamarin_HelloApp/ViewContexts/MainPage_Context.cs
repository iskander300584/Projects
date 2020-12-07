using Ascon.Pilot.DataClasses;
using PilotMobile.AppContext;
using PilotMobile.Pages;
using PilotMobile.ViewContexts;
using PilotMobile.ViewModels;
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

            Up_CanExecute();
        }


        #region Методы


        /// <summary>
        /// Получить список головных объектов
        /// </summary>
        /// <param name="update">обновить данные</param>
        /// <param name="url">ссылка на объект</param>
        public void GetRootObjects(bool update = false, String url = null)
        {
            if (Mode != PageMode.Slave)
            {
                DObject rootObj = Global.DALContext.Repository.GetObjects(new[] { DObject.RootId }).First();

                root = new PilotTreeItem(rootObj);
            }

            Guid? guid;
            bool needGetRoot = false; // Признак необходимости получения головных объектов

            // Получение головных объектов
            if (url == null || (guid = ParseURL(url)) == null)
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

                    if (item.VisibleName != "" && !item.Type.IsSystem && !item.DObject.InRootRecycleBin())
                    {
                        Mode = PageMode.Url;

                        // Получение структуры для объекта
                        if (!item.Type.IsDocument)
                        {
                            Parent = item;
                            Items = Parent.Children;

                            AsyncGetChildren(item);
                        }
                        // Открытие документа
                        else
                        {
                            App.Current.ModalPopping += page.HandleModalPopping;

                            page.Navigation.PushModalAsync(new DocumentCarrousel(item));
                        }
                    }
                    else if(item.DObject.InRootRecycleBin() || item.DObject.IsForbidden())
                    {
                        var result = page.DisplayMessage("Внимание!", "Объект был удален", false);

                        needGetRoot = true;
                    }
                    else
                    {
                        var result = page.DisplayMessage("Внимание!", "У Вас нет доступа к указанному объекту", false);

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
        }


        /// <summary>
        /// Получение объектов, вложенных в задание
        /// </summary>
        private void GetTaskRootObjects()
        {
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


        /// <summary>
        /// Асинхронный метод получения вложенных объектов
        /// </summary>
        /// <param name="dObject">объект Pilot</param>
        private void AsyncGetChildren(IPilotObject pilotItem)
        {
            Thread thread = new Thread(new ParameterizedThreadStart(GetChildren));
            thread.Start(pilotItem);
        }


        /// <summary>
        /// Метод получения вложенных объектов для параллельного потока
        /// </summary>
        /// <param name="pilotItem">объект Pilot</param>
        private void GetChildren(object pilotItem)
        {
            PilotTreeItem _item = (PilotTreeItem)pilotItem;

            foreach (DChild dchild in _item.DObject.Children)
            {
                PilotTreeItem item = new PilotTreeItem(dchild, _item);
                // item.HasAccess
                if (item.VisibleName.Trim() != "" && !item.Type.IsSystem)
                    Items.Add(item);
            }
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
        public void ItemTapped(PilotTreeItem item)
        {
            Parent = item;

            Items = Parent.Children;

            if (Parent.Children.Count == 0)
                AsyncGetChildren(Parent);

            Up_CanExecute();
        }


        /// <summary>
        /// Нажатие кнопки Вверх
        /// </summary>
        public void Up_Execute()
        {
            if (Mode != PageMode.Url)
            {
                Parent = Parent.Parent;

                Items = Parent.Children;
            }
            else
            {
                Guid parentGuid = Parent.DObject.ParentId;

                if(parentGuid != Root.Guid)
                {
                    PilotTreeItem parent = new PilotTreeItem(Global.DALContext.Repository.GetObjects(new Guid[] { parentGuid }).FirstOrDefault(), true);

                    Parent = parent;
                    Items = Parent.Children;

                    AsyncGetChildren(parent);
                }
                // Получение корневых объектов
                else
                {
                    Mode = PageMode.View;

                    Parent = Root;

                    Items = Parent.Children;

                    if (Items.Count == 0)
                    {
                        AsyncGetChildren(Parent);
                    }

                    Up_CanExecute();
                }
            }

            Up_CanExecute();
        }


        /// <summary>
        /// Нажатие кнопки Домой
        /// </summary>
        public void Home_Execute()
        {
            Mode = PageMode.View;

            Parent = Root;

            Items = Parent.Children;

            if(Items.Count == 0)
            {
                AsyncGetChildren(Parent);
            }

            Up_CanExecute();
        }


        /// <summary>
        /// Нажатие кнопки Обновить
        /// </summary>
        public void Update_Execute()
        {
            try
            {
                /*if (!IsConnected)
                    if (Global.DALContext.Connect(Global.Credentials) != null)
                        return;*/

                Exception ex = Global.Reconnect();
                if(ex != null)
                {
                    var res = page.DisplayMessage(StringConstants.Warning, ex.Message, false);
                    return;
                }

                if (IsConnected)
                {
                    Parent.Children.Clear();

                    Parent.UpdateObjectData();

                    if (Mode == PageMode.View || Parent != Root)
                        AsyncGetChildren(Parent);
                    else
                        GetTaskRootObjects();
                }

                Up_CanExecute();
            }
            catch(Exception ex)
            {
                var res = page.DisplayMessage("Ошибка", ex.Message, false);
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
                    App.Current.MainPage = new AuthorizePage(true);
                    break;

                case StringConstants.ClearCache:
                    ClearCache();
                    break;

                case StringConstants.Exit:
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
                        await page.DisplayMessage("Ошибка удаления файла", ex.Message, false);
                    }
                }
            }

            string _size = String.Format("{0:#.#}", size / 1048576);
            if (_size.Trim() == "")
                _size = "0";
            else if (_size[0] == '.' || _size[0] == ',')
                _size = "0" + _size;

            await page.DisplayMessage(StringConstants.CacheCleared, StringConstants.Free + _size + StringConstants.TotalSize, false);
        }


        /// <summary>
        /// Выполнить поиск данных
        /// </summary>
        /// <param name="query">строка поиска</param>
        public void DoSearch(string query)
        {
            Mode = PageMode.Search;

            Parent = new PilotTreeItem();

            Items = Parent.Children;

            Thread thread = new Thread(new ParameterizedThreadStart(GetSearchResults));
            thread.Start(query);
        }


        /// <summary>
        /// Получение результатов поиска
        /// </summary>
        /// <param name="queryString">запрос поиска</param>
        private async void GetSearchResults(object queryString)
        {
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
                if(ex != null)
                {

                }
            }

            Up_CanExecute();
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