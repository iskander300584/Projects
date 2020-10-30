using PilotMobile.AppContext;
using PilotMobile.Models.SearchQuery;
using PilotMobile.Pages;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin_HelloApp.AppContext;
using Xamarin_HelloApp.Models;


namespace PilotMobile.ViewContexts
{
    /// <summary>
    /// Контекст данных страницы формирования запроса поиска
    /// </summary>
    class SearchQueryPage_Context : INotifyPropertyChanged
    {
        #region Поля класса


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
        private SearchQueryPage page;


        /// <summary>
        /// Список всех доступных типов
        /// </summary>
        private List<PType> _allowedTypes;


        private ObservableCollection<string> types = new ObservableCollection<string>();
        /// <summary>
        /// Список типов, доступных для добавления
        /// </summary>
        public ObservableCollection<string> Types
        {
            get => types;
        }


        private string selectedType;
        /// <summary>
        /// Выбранный тип
        /// </summary>
        public string SelectedType
        {
            get => selectedType;
            set
            {
                if(selectedType != value)
                {
                    selectedType = value;
                    OnPropertyChanged();

                    AddType_CanExecute = (selectedType != null);
                }
            }
        }


        private ObservableCollection<string> attributes = new ObservableCollection<string>();
        /// <summary>
        /// Список атрибутов, доступных для добавления
        /// </summary>
        public ObservableCollection<string> Attributes
        {
            get => attributes;
        }


        private string selectedAttribute;
        /// <summary>
        /// Выбранный атрибут
        /// </summary>
        public string SelectedAttribute
        {
            get => selectedAttribute;
            set
            {
                if(selectedAttribute != value)
                {
                    selectedAttribute = value;
                    OnPropertyChanged();

                    AddAttribute_CanExecute = (selectedAttribute != null);
                }
            }
        }


        private ObservableCollection<ISearchQueryItem> items = new ObservableCollection<ISearchQueryItem>();
        /// <summary>
        /// Список элементов поиска
        /// </summary>
        public ObservableCollection<ISearchQueryItem> Items
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


        private ISearchQueryItem selectedItem;
        /// <summary>
        /// Выбранный элемент поиска
        /// </summary>
        public ISearchQueryItem SelectedItem
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


        private ICommand addType;
        /// <summary>
        /// Команда добавления типа
        /// </summary>
        public ICommand AddType
        {
            get => addType;
        }


        private ICommand addAttribute;
        /// <summary>
        /// Команда добавления атрибута
        /// </summary>
        public ICommand AddAttribute
        {
            get => addAttribute;
        }


        private ICommand deleteItem;
        /// <summary>
        /// Команда удаления элемента поиска
        /// </summary>
        public ICommand DeleteItem
        {
            get => deleteItem;
        }


        private ICommand search;
        /// <summary>
        /// Команда Поиск
        /// </summary>
        public ICommand Search
        {
            get => search;
        }


        private bool addType_CanExecute = false;
        /// <summary>
        /// Признак доступности команды добавления типа
        /// </summary>
        public bool AddType_CanExecute
        {
            get => addType_CanExecute;
            private set
            {
                if(addType_CanExecute != value)
                {
                    addType_CanExecute = value;
                    OnPropertyChanged();
                }
            }
        }


        private bool addAttribute_CanExecute = false;
        /// <summary>
        /// Признак доступности команды добавления атрибута
        /// </summary>
        public bool AddAttribute_CanExecute
        {
            get => addAttribute_CanExecute;
            private set
            {
                if(addAttribute_CanExecute != value)
                {
                    addAttribute_CanExecute = value;
                    OnPropertyChanged();
                }
            }
        }


        private bool search_CanExecute = false;
        /// <summary>
        /// Признак доступности команды Поиск
        /// </summary>
        public bool Search_CanExecute
        {
            get => search_CanExecute;
            private set
            {
                if(search_CanExecute != value)
                {
                    search_CanExecute = value;
                    OnPropertyChanged();
                }
            }
        }


        private string query = string.Empty;
        /// <summary>
        /// Строковое представление запроса поиска
        /// </summary>
        public string Query
        {
            get => query;
        }


        #endregion


        /// <summary>
        /// Контекст данных страницы формирования запроса поиска
        /// </summary>
        public SearchQueryPage_Context(SearchQueryPage page)
        {
            this.page = page;

            GetTypes();
            addType = new Command(AddType_Execute);
            addAttribute = new Command(AddAttribute_Execute);
            deleteItem = new Command(DeleteItem_Execute);
            search = new Command(Search_Execute);
        }


        #region Методы класса


        /// <summary>
        /// Получение списка доступных типов
        /// </summary>
        private void GetTypes()
        {
            if (_allowedTypes == null)
                GetAllowedTypes();

            Types.Clear();

            foreach (PType type in _allowedTypes)
                if (!items.Any(i => i.IsType && i.Value == type.VisibleName) && !Types.Contains(type.VisibleName))
                    Types.Add(type.VisibleName);


            SelectedType = (Types.Count > 0) ? Types[0] : null;
        }


        /// <summary>
        /// Получение списка всех доступных типов
        /// </summary>
        private void GetAllowedTypes()
        {
            List<PType> _types = TypeFabrique.GetAllTypes();

            _allowedTypes = new List<PType>();

            foreach (PType type in _types)
                if (!type.IsTask && !type.IsSystem && type.MType.Kind == Ascon.Pilot.DataClasses.TypeKind.User && !type.MType.IsService)
                    _allowedTypes.Add(type);

            _allowedTypes = _allowedTypes.OrderBy(t => t.VisibleName).ToList();
        }


        /// <summary>
        /// Получение списка доступных атрибутов
        /// </summary>
        private void GetAttributes()
        {
            Attributes.Clear();

            // Перебор всех добавленных типов
            foreach(ISearchQueryItem typeQuery in Items.Where(i => i.IsType))
            {
                // Перебор всех типов с таким наименованием
                foreach(PType type in _allowedTypes.Where(t => t.VisibleName == typeQuery.Value))
                {
                    foreach(PAttribute attr in type.Attributes.Where(a => !a.IsSystem))
                    {
                        if (!Attributes.Contains(attr.VisibleName) && !Items.Any(i => !i.IsType && i.Name == attr.VisibleName))
                            Attributes.Add(attr.VisibleName);
                    }
                }
            }

            SelectedAttribute = (Attributes.Count > 0) ? Attributes[0] : null;
        }


        /// <summary>
        /// Выполнение команды добавления типа
        /// </summary>
        private void AddType_Execute()
        {
            PType type = _allowedTypes.FirstOrDefault(t => t.VisibleName == SelectedType);

            if(type != null)
            {
                items.Add(new TypeQueryItem(type, DeleteItem));
                Items = SortItems();

                Types.Remove(SelectedType);

                GetAttributes();
            }

            SelectedType = (Types.Count > 0) ? Types[0] : null;

            Search_CanExecute = (Items.Count > 0);
        }


        /// <summary>
        /// Выполнение команды добавления атрибута
        /// </summary>
        private void AddAttribute_Execute()
        {
            PAttribute pAttribute = null;

            foreach(PType type in _allowedTypes)
            {
                pAttribute = type.Attributes.FirstOrDefault(a => a.VisibleName == SelectedAttribute);

                if (pAttribute != null)
                    break;
            }

            Items.Add(new AttributeQueryItem(pAttribute, DeleteItem));

            Attributes.Remove(SelectedAttribute);
            
            SelectedAttribute = (Attributes.Count > 0) ? Attributes[0] : null;

            Search_CanExecute = (Items.Count > 0);
        }


        /// <summary>
        /// Выполнение команды удаления элемента поиска
        /// </summary>
        private void DeleteItem_Execute(object parameter)
        {
            if(parameter != null)
            {
                string param = (string)parameter;

                int id = 0;
                if (int.TryParse(param, out id))
                {
                    DeleteType(id);

                    GetTypes();
                }
                else
                {
                    ISearchQueryItem item = Items.FirstOrDefault(i => !i.IsType && i.SystemName == param);

                    if (item != null)
                        Items.Remove(item);
                }

                GetAttributes();
            }

            Search_CanExecute = (Items.Count > 0);
        }


        /// <summary>
        /// Удаление типа из элементов поиска
        /// </summary>
        /// <param name="id">ID типа</param>
        private void DeleteType(int id)
        {
            ISearchQueryItem item = Items.FirstOrDefault(i => i.IsType && i.SystemName == id.ToString());
            if(item != null)
            {
                Items.Remove(item);

                TypeQueryItem typeQuery = item as TypeQueryItem;

                // Удаление атрибутов, связанных только с этим типом
                List<string> attrTitles = new List<string>();

                foreach (PAttribute attr in typeQuery.Type.Attributes)
                    attrTitles.Add(attr.VisibleName);

                List<ISearchQueryItem> _typeQueries = Items.Where(i => i.IsType).ToList();

                List<PType> _addedTypes = new List<PType>(); // список типов, для которых могли быть добавлены атрибуты
                foreach(ISearchQueryItem _typeQuery in _typeQueries)
                {
                    foreach (PType type in _allowedTypes.Where(t => t.VisibleName == _typeQuery.Value))
                        _addedTypes.Add(type);
                }

                // Исключение лишних атрибутов
                foreach (string attrTitle in attrTitles)
                {
                    ISearchQueryItem attrQuery = Items.FirstOrDefault(i => !i.IsType && i.Name == attrTitle);
                    if (attrQuery != null)
                    {
                        bool finded = false;

                        foreach(PType _type in _addedTypes)
                        {
                            if(_type.Attributes.Any(a => a.VisibleName == attrTitle && !a.IsSystem))
                            {
                                finded = true;
                                break;
                            }
                        }

                        if (!finded)
                            Items.Remove(attrQuery);
                    }
                }
            }
        }


        /// <summary>
        /// Выполнение команды Поиск
        /// </summary>
        private void Search_Execute()
        {
            ParseSearchQuery();

            page.NavigationToMain();
        }


        /// <summary>
        /// Формирование строки запроса TODO
        /// </summary>
        private void ParseSearchQuery()
        {
            query = "here will be query";
        }


        /// <summary>
        /// Сортировка элементов наблюдаемой коллекции
        /// </summary>
        private ObservableCollection<ISearchQueryItem> SortItems()
        {
            ObservableCollection<ISearchQueryItem> _collection = new ObservableCollection<ISearchQueryItem>();

            IEnumerable<ISearchQueryItem> temp = Items.Where(i => i.IsType).OrderBy(t => t.Value);
            foreach (ISearchQueryItem _typeItem in temp)
                _collection.Add(_typeItem);

            temp = Items.Where(i => !i.IsType).OrderBy(a => a.Name);
            foreach (ISearchQueryItem _attrItem in temp)
                _collection.Add(_attrItem);

            return _collection;
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