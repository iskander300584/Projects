using Ascon.Pilot.DataClasses;
using PilotMobile.AppContext;
using PilotMobile.Pages;
using PilotMobile.ViewModels;
using System;
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
    /// Контекст данных окна карточки
    /// </summary>
    class CardPage_Context : INotifyPropertyChanged
    {
        private IPilotObject pilotObject;
        /// <summary>
        /// Элемент Pilot
        /// </summary>
        public IPilotObject PilotObject
        {
            get => pilotObject;
        }


        /// <summary>
        /// Страница карточки
        /// </summary>
        private CardPage page;


        private ObservableCollection<View> views = new ObservableCollection<View>();
        /// <summary>
        /// Список отображаемых элементов
        /// </summary>
        public ObservableCollection<View> Views
        {
            get => views;
        }


        private PageMode mode;
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

                    ParsePageMode();
                }
            }
        }


        private bool isEnabled = false;
        /// <summary>
        /// Признак доступности элементов для редактирования
        /// </summary>
        public bool IsEnabled
        {
            get => isEnabled;
            private set
            {
                if(isEnabled != value)
                {
                    isEnabled = value;
                    OnPropertyChanged();
                }
            }
        }


        private bool isReadOnly = true;
        /// <summary>
        /// Признак доступности элементов для записи
        /// </summary>
        public bool IsReadOnly
        {
            get => isReadOnly;
            private set
            {
                if(isReadOnly != value)
                {
                    isReadOnly = value;
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


        private ICommand updateCommand;
        /// <summary>
        /// Команда Обновить
        /// </summary>
        public ICommand UpdateCommand
        {
            get => updateCommand;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }


        /// <summary>
        /// Контекст данных окна карточки
        /// </summary>
        /// <param name="pilotObject">элемент Pilot</param>
        /// <param name="mode">режим работы окна</param>
        /// <param name="page">страница карточки</param>
        public CardPage_Context(IPilotObject pilotObject, CardPage page, PageMode mode = PageMode.View)
        {
            this.pilotObject = pilotObject;
            this.page = page;

            upCommand = new Command(Up_Execute);
            updateCommand = new Command(GetData);

            GetData();

            Mode = mode;
        }


        /// <summary>
        /// Получение данных
        /// </summary>
        private void GetData()
        {
            views.Clear();

            foreach (PAttribute attribute in pilotObject.Type.Attributes.Where(a => !a.IsSystem))
            {
                Label label = new Label
                {
                    Text = attribute.VisibleName,
                    Margin = new Thickness
                    {
                        Left = 10,
                        Right = 10
                    },
                    FontSize = Device.GetNamedSize(NamedSize.Body, typeof(Label)),
                };

                views.Add(label);

                KeyValuePair<string, DValue> attr = new KeyValuePair<string, DValue>();

                switch (attribute.AttributeType)
                {
                    // Строка
                    case MAttrType.String:
                        attr = pilotObject.DObject.Attributes.FirstOrDefault(a => a.Key == attribute.Name);
                        string valueStr = (attr.Value != null && attr.Value.StrValue != null) ? attr.Value.StrValue : "";

                        Entry entry = new Entry
                        {
                            Text = valueStr,
                            FontSize = Device.GetNamedSize(NamedSize.Body, typeof(Label)),
                            Margin = new Thickness
                            {
                                Left = 10,
                                Bottom = 10,
                                Right = 10
                            }
                        };
                        // привязка к свойству ReadOnly
                        Binding bindingEntry = new Binding
                        {
                            Source = this,
                            Path = "IsReadOnly"
                        };
                        entry.SetBinding(Entry.IsReadOnlyProperty, bindingEntry);

                        views.Add(entry);
                        break;

                    // Дата
                    case MAttrType.DateTime:
                        attr = pilotObject.DObject.Attributes.FirstOrDefault(a => a.Key == attribute.Name);
                        DateTime? valueDT = (attr.Value != null && attr.Value.DateValue != null && attr.Value.DateValue != null) ? attr.Value.DateValue : null;

                        DatePicker datePicker = new DatePicker
                        {
                            FontSize = Device.GetNamedSize(NamedSize.Body, typeof(Label)),
                            Margin = new Thickness
                            {
                                Left = 10,
                                Bottom = 5,
                                Right = 10
                            }
                        };
                        if (valueDT != null)
                            datePicker.Date = valueDT.Value;
                        // привязка к свойству IsEnabled
                        Binding bindingDate = new Binding
                        {
                            Source = this,
                            Path = "IsEnabled"
                        };
                        datePicker.SetBinding(DatePicker.IsEnabledProperty, bindingDate);

                        views.Add(datePicker);
                        break;

                    // Организацинная единица
                    case MAttrType.OrgUnit:
                        attr = pilotObject.DObject.Attributes.FirstOrDefault(a => a.Key == attribute.Name);
                        int[] valueOU = (attr.Value != null && attr.Value.IsArray && attr.Value.ArrayIntValue != null) ? attr.Value.ArrayIntValue : null;

                        Editor editor = new Editor
                        {
                            FontSize = Device.GetNamedSize(NamedSize.Body, typeof(Label)),
                            Margin = new Thickness
                            {
                                Left = 10,
                                Bottom = 5,
                                Right = 10
                            }
                        };
                        // привязка к свойству ReadOnly
                        Binding bindingEditor = new Binding
                        {
                            Source = this,
                            Path = "IsReadOnly"
                        };
                        editor.SetBinding(Editor.IsReadOnlyProperty, bindingEditor);

                        // Получение пользователей
                        if (valueOU != null)
                            foreach (int id in valueOU)
                                editor.Text += Global.DALContext.Repository.GetPersonOnOrganisationUnit(id).DisplayName + "\n";

                        views.Add(editor);
                        break;

                    // Число
                    case MAttrType.Integer:
                    case MAttrType.Numerator:
                        attr = pilotObject.DObject.Attributes.FirstOrDefault(a => a.Key == attribute.Name);
                        long? valueInt = (attr.Value != null && attr.Value.IntValue != null) ? attr.Value.IntValue : null;

                        Entry entryInt = new Entry
                        {
                            FontSize = Device.GetNamedSize(NamedSize.Body, typeof(Label)),
                            Margin = new Thickness
                            {
                                Left = 10,
                                Bottom = 10,
                                Right = 10
                            }
                        };
                        if (valueInt != null)
                            entryInt.Text = ((long)valueInt).ToString();
                        // привязка к свойству ReadOnly
                        Binding bindingInt = new Binding
                        {
                            Source = this,
                            Path = "IsReadOnly"
                        };
                        entryInt.SetBinding(Entry.IsReadOnlyProperty, bindingInt);

                        views.Add(entryInt);
                        break;

                    // Decimal
                    case MAttrType.Decimal:
                        attr = pilotObject.DObject.Attributes.FirstOrDefault(a => a.Key == attribute.Name);
                        decimal? valueDec = (attr.Value != null && attr.Value.DecimalValue != null) ? attr.Value.DecimalValue : null;

                        Entry entryDec = new Entry
                        {
                            FontSize = Device.GetNamedSize(NamedSize.Body, typeof(Label)),
                            Margin = new Thickness
                            {
                                Left = 10,
                                Bottom = 10,
                                Right = 10
                            }
                        };
                        if (valueDec != null)
                            entryDec.Text = ((decimal)valueDec).ToString();
                        // привязка к свойству ReadOnly
                        Binding bindingDec = new Binding
                        {
                            Source = this,
                            Path = "IsReadOnly"
                        };
                        entryDec.SetBinding(Entry.IsReadOnlyProperty, bindingDec);

                        views.Add(entryDec);
                        break;

                    // Вещественное число
                    case MAttrType.Double:
                        attr = pilotObject.DObject.Attributes.FirstOrDefault(a => a.Key == attribute.Name);
                        double? valueDou = (attr.Value != null && attr.Value.DoubleValue != null) ? attr.Value.DoubleValue : null;

                        Entry entryDou = new Entry
                        {
                            FontSize = Device.GetNamedSize(NamedSize.Body, typeof(Label)),
                            Margin = new Thickness
                            {
                                Left = 10,
                                Bottom = 10,
                                Right = 10
                            }
                        };
                        if (valueDou != null)
                            entryDou.Text = ((decimal)valueDou).ToString().Replace(',', '.');
                        // привязка к свойству ReadOnly
                        Binding bindingDou = new Binding
                        {
                            Source = this,
                            Path = "IsReadOnly"
                        };
                        entryDou.SetBinding(Entry.IsReadOnlyProperty, bindingDou);

                        views.Add(entryDou);
                        break;
                }
            }
        }


        /// <summary>
        /// Изменение режима работы окна
        /// </summary>
        private void ParsePageMode()
        {
            IsEnabled = (mode != PageMode.View);
            IsReadOnly = !isEnabled;
        }


        /// <summary>
        /// Кнопка Вверх
        /// </summary>
        private void Up_Execute()
        {
            page.NavigateToMainPage();
        }
    }
}