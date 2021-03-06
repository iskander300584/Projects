﻿using Ascon.Pilot.DataClasses;
using Ascon.Pilot.DataModifier;
using PilotMobile.AppContext;
using PilotMobile.Models;
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
using Xamarin_HelloApp.Pages;


namespace PilotMobile.ViewContexts
{
    /// <summary>
    /// Контекст данных окна карточки
    /// </summary>
    class CardPage_Context : INotifyPropertyChanged
    {
        #region Поля класса

        /// <summary>
        /// Наименование приложения
        /// </summary>
        public string AppName
        {
            get => StringConstants.ApplicationName;
        }


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


        /// <summary>
        /// Страница XPS
        /// </summary>
        private XpsPage xpsPage;


        private ICommand changeState;
        /// <summary>
        /// Команда Сменить состояние
        /// </summary>
        public ICommand ChangeState
        {
            get => changeState;
        }


        private bool changeStateVisible = false;
        /// <summary>
        /// Видимость кнопки смены состояния
        /// </summary>
        public bool ChangeStateVisible
        {
            get => changeStateVisible;
            private set
            {
                if(changeStateVisible != value)
                {
                    changeStateVisible = value;
                    OnPropertyChanged();
                }
            }
        }


        private bool canChangeState = false;
        /// <summary>
        /// Доступность смены состояния
        /// </summary>
        public bool CanChangeState
        {
            get => canChangeState;
            private set
            {
                if(canChangeState != value)
                {
                    canChangeState = value;
                    OnPropertyChanged();
                }
            }
        }


        private ICommand editCommand = null;
        /// <summary>
        /// Команда Редактировать
        /// </summary>
        public ICommand EditCommand
        {
            get => editCommand;
        }


        #endregion      


        /// <summary>
        /// Контекст данных окна карточки
        /// </summary>
        /// <param name="pilotObject">элемент Pilot</param>
        /// <param name="mode">режим работы окна</param>
        /// <param name="page">страница карточки</param>
        /// <param name="xpsPage">страница XPS</param>
        public CardPage_Context(IPilotObject pilotObject, CardPage page, XpsPage xpsPage, PageMode mode = PageMode.View)
        {
            this.pilotObject = pilotObject;
            this.page = page;
            this.xpsPage = xpsPage;

            upCommand = new Command(Up_Execute);
            updateCommand = new Command(GetData);
            changeState = new Command(ChangeState_Execute);
            editCommand = new Command(EditExecute);

            GetData();

            Mode = mode;
        }


        #region Методы класса

        /// <summary>
        /// Получение данных
        /// </summary>
        private async void GetData()
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

                if(pilotObject is PilotTask)
                {
                    ChangeStateVisible = true;

                    PilotTask task = pilotObject as PilotTask;
                    CanChangeState = (task.AvaliableTransitions.Count > 0);
                    //CanChangeState = (task.AvaliableStates.Count > 0);
                }

                views.Clear();

                foreach (PAttribute attribute in pilotObject.Type.Attributes.Where(a => !a.IsSystem).OrderBy(at => at.MAttribute.DisplaySortOrder))
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
            catch (Exception ex)
            {
                var res = await Global.DisplayError(page, ex.Message);

                if (res)
                    await Global.SendErrorReport(ex);
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


        /// <summary>
        /// Смена состояния
        /// </summary>
        private async void ChangeState_Execute()
        {
            try
            {
                List<string> _states = new List<string>();
                PilotTask _task = PilotObject as PilotTask;
                foreach (MTransition transition in _task.AvaliableTransitions)
                {
                    if (transition.DisplayName.Trim() != "")
                        _states.Add(transition.DisplayName.Trim());
                    else
                    {
                        PState _state = StateFabrique.GetState(transition.StateTo);
                        if (_state != null)
                            _states.Add(_state.MUserState.Title);
                    }
                }

                string newState = await page.GetStateAction(_states.ToArray());

                if (newState == null || !_states.Contains(newState))
                    return;

                PState _nextState = null;

                MTransition _transition = _task.AvaliableTransitions.FirstOrDefault(t => t.DisplayName.Trim() == newState);
                if(_transition == null)
                {
                    foreach(MTransition mTransition in _task.AvaliableTransitions.Where(t => t.DisplayName.Trim() == ""))
                    {
                        PState _tempState = StateFabrique.GetState(mTransition.StateTo);
                        if(_tempState != null && _tempState.MUserState.Title == newState)
                        {
                            _nextState = _tempState;
                            break;
                        }
                    }
                }
                else
                    _nextState = StateFabrique.GetState(_transition.StateTo);

                if (_nextState == null)
                    throw new Exception("Ошибка выбора следующего состояния");

                newState = _nextState.MUserState.Title;

                bool confirm = await page.DisplayMessage("Внимание!", "Перевести задание в состояние: '" + newState + "'?", true);
                if (!confirm)
                    return;

                IModifier _modifier = Global.DALContext.Repository.GetModifier();
                var _editBuilder = _modifier.EditObject(PilotObject.Guid);
                var _cb = _editBuilder.SetAttribute(TaskConstants.StateAttribute, _nextState.Guid);
                if (_modifier.AnyChanges())
                    _modifier.Apply();               

                _task.SetState(_nextState);
                _task.GetStateMachineData(page);
                CanChangeState = (_task.AvaliableTransitions.Count > 0);

                Global.ShowToast("Состояние изменено");
            }
            catch(Exception ex)
            {
                var res = await Global.DisplayError(page, ex.Message);

                if (res)
                    await Global.SendErrorReport(ex);
            }
        }


        /// <summary>
        /// Редактировать
        /// </summary>
        private void EditExecute()
        {
            if (Mode == PageMode.View)
                Mode = PageMode.Edit;
            else if (Mode == PageMode.Edit)
                Mode = PageMode.View;
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