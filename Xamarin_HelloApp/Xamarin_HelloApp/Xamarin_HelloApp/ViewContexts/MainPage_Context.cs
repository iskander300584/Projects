using Ascon.Pilot.DataClasses;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Xamarin_HelloApp.AppContext;
using Xamarin_HelloApp.ViewModels;

namespace Xamarin_HelloApp.ViewContexts
{
    /// <summary>
    /// Контекст данных главного окна
    /// </summary>
    class MainPage_Context : INotifyPropertyChanged
    {
        private ObservableCollection<PilotTreeItem> items = new ObservableCollection<PilotTreeItem>();
        /// <summary>
        /// Список отображаемых элементов
        /// </summary>
        public ObservableCollection<PilotTreeItem> Items
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


        private PilotTreeItem selectedItem = null;
        /// <summary>
        /// Выбранный объект списка
        /// </summary>
        public PilotTreeItem SelectedItem
        {
            get => selectedItem;
            set
            {
                if(selectedItem != value)
                {
                    selectedItem = value;
                    OnPropertyChanged();
                }
            }
        }


        private PilotTreeItem parent;
        /// <summary>
        /// Головной элемент для элементов отображаемого списка
        /// <para>NULL, если отображаются корневые элементы</para>
        /// </summary>
        public PilotTreeItem Parent
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


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }


        /// <summary>
        /// Контекст данных главного окна
        /// </summary>
        public MainPage_Context()
        {
            if (IsConnected)
                GetRootObjects();
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
        private void AsyncGetChildren(PilotTreeItem pilotItem)
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
        }


        /// <summary>
        /// Нажатие кнопки Вверх
        /// </summary>
        public void Up_Execute()
        {
            Parent = Parent.Parent;

            Items = Parent.Children;
        }


        /// <summary>
        /// Нажатие кнопки Домой
        /// </summary>
        public void Home_Execute()
        {
            Parent = Root;

            Items = Parent.Children;
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

                AsyncGetChildren(Parent);
            }
        }
    }
}