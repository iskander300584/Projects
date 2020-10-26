﻿using Ascon.Pilot.DataClasses;
using PilotMobile.AppContext;
using PilotMobile.ViewModels;
using System;
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
        /// <summary>
        /// Наименование приложения
        /// </summary>
        public string AppName
        {
            get => StringConstants.ApplicationName;
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


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }


        /// <summary>
        /// Контекст данных главного окна
        /// </summary>
        public MainPage_Context(MainPage page)
        {
            this.page = page;

            if (IsConnected)
                GetRootObjects();

            upCommand = new Command(Up_Execute);
            homeCommand = new Command(Home_Execute);
            updateCommand = new Command(Update_Execute);
            mainMenuCommand = new Command(MainMenu_Execute);

            Up_CanExecute();
        }


        /// <summary>
        /// Получить список головных объектов
        /// </summary>
        private void GetRootObjects(bool update = false)
        {
            DObject rootObj = Global.DALContext.Repository.GetObjects(new[] { DObject.RootId }).First();

            root = new PilotTreeItem(rootObj);
            Parent = Root;

            Items = Parent.Children;

            if (update || Parent.Children.Count == 0)
            {
                Parent.Children.Clear();
                AsyncGetChildren(Parent);
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
        /// Проверка возможности нажатия кнопки Вверх
        /// </summary>
        private void Up_CanExecute()
        {
            UpCanExecute = (Parent != null && Root != null && Parent != Root);
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
            Parent = Parent.Parent;

            Items = Parent.Children;

            Up_CanExecute();
        }


        /// <summary>
        /// Нажатие кнопки Домой
        /// </summary>
        public void Home_Execute()
        {
            Parent = Root;

            Items = Parent.Children;

            Up_CanExecute();
        }


        /// <summary>
        /// Нажатие кнопки Обновить
        /// </summary>
        public void Update_Execute()
        {
            if (!IsConnected)
                if (Global.DALContext.Connect(Global.Credentials) != null)
                    return;

            if (IsConnected)
            {
                Parent.Children.Clear();

                Parent.UpdateObjectData();

                AsyncGetChildren(Parent);
            }

            Up_CanExecute();
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
    }
}