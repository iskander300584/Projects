using Ascon.Pilot.DataClasses;
using PilotMobile.AppContext;
using PilotMobile.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin_HelloApp.AppContext;

namespace PilotMobile.ViewModels
{
    /// <summary>
    /// Задание Pilot
    /// </summary>
    public class PilotTask : IPilotObject
    {
        #region Поля класса

        private PState state;
        /// <summary>
        /// Состояние задания
        /// </summary>
        public PState State
        {
            get => state;
            private set
            {
                if(state != value)
                {
                    state = value;
                    OnPropertyChanged();

                    //GetStateMachineData();
                }
            }
        }


        private string title = string.Empty;
        /// <summary>
        /// Наименование задания
        /// </summary>
        public string Title
        {
            get => title;
            private set
            {
                if(title != value)
                {
                    title = value;
                    OnPropertyChanged();

                    GetVisibleName();
                }
            }
        }


        private bool isActual = false;
        /// <summary>
        /// Актуально для текущего пользователя
        /// </summary>
        public bool IsActual
        {
            get => isActual;
        }


        private bool isInitiator = false;
        /// <summary>
        /// Пользователь является инициатором задания
        /// </summary>
        public bool IsInitiator
        {
            get => isInitiator;
        }


        private bool isExecutor = false;
        /// <summary>
        /// Пользователь является исполнителем задания
        /// </summary>
        public bool IsExecutor
        {
            get => isExecutor;
        }


        private bool isResponsible = false;
        /// <summary>
        /// Пользователь является ответственным за исполнение задания
        /// </summary>
        public bool IsResponsible
        {
            get => isResponsible;
        }


        private bool isAuditor = false;
        /// <summary>
        /// Пользователь является аудитором
        /// </summary>
        public bool IsAuditor
        {
            get => isAuditor;
        }


        private DateTime deadline;
        /// <summary>
        /// Срок исполнения задания
        /// </summary>
        public DateTime Deadline
        {
            get => deadline;
            set
            {
                if(deadline != value)
                {
                    deadline = value;
                    OnPropertyChanged();

                    GetVisibleName();
                }
            }
        }


        private DateTime dateOfAssignment;
        /// <summary>
        /// Дата выдачи
        /// </summary>
        public DateTime DateOfAssignment
        {
            get => dateOfAssignment;
            set
            {
                if(dateOfAssignment != value)
                {
                    dateOfAssignment = value;
                    OnPropertyChanged();
                }
            }
        }


        private MUserStateMachine stateMachine = null;
        /// <summary>
        /// Соответствующая машина состояний
        /// </summary>
        public MUserStateMachine StateMachine
        {
            get => stateMachine;
            private set
            {
                if(stateMachine != value)
                {
                    stateMachine = value;
                    OnPropertyChanged();
                }
            }
        }


        private List<PState> avaliableStates = new List<PState>();
        /// <summary>
        /// Доступные состояния
        /// </summary>
        public List<PState> AvaliableStates
        {
            get => avaliableStates;
            private set
            {
                if(avaliableStates != value)
                {
                    avaliableStates = value;
                    OnPropertyChanged();
                }
            }
        }


        #endregion


        /// <summary>
        /// Задание Pilot
        /// </summary>
        /// <param name="guid">GUID задания</param>
        public PilotTask(Guid guid)
        {
            this.guid = guid;

            dObject = Global.DALContext.Repository.GetObjects(new[] { guid }).FirstOrDefault();
            
            type = TypeFabrique.GetType(dObject.TypeId);

            GetData();
        }


        #region Методы класса


        /// <summary>
        /// Формирование отображаемого имени
        /// </summary>
        private void GetVisibleName()
        {
            string _name = title;
            if(deadline != null && deadline.Year != 9999)
            {
                _name += $"\nСрок: {deadline.Day}.{deadline.Month}.{deadline.Year}";
            }
            _name = _name.Trim();

            VisibleName = _name;
        }


        /// <summary>
        /// Получение данных задачи
        /// </summary>
        private void GetData()
        {
            // Наименование
            DValue value = TryGetAttribute(TaskConstants.TitleAttribute);
            if (value != null)
                Title = value.StrValue;

            // Дата выдачи
            value = TryGetAttribute(TaskConstants.DateOfAssignmentAttribute);
            if (value != null)
                DateOfAssignment = (DateTime)value.DateValue;
            else
                DateOfAssignment = DateTime.MinValue;

            // Срок выполнения
            value = TryGetAttribute(TaskConstants.DeadlineAttribute);
            if (value != null)
                Deadline = (DateTime)value.DateValue;

            // Актуально для
            value = TryGetAttribute(TaskConstants.ActualAttribute);
            if(value != null && value.IntValue != null)
                isActual = (Global.CurrentPerson.Positions.Contains((int)value.IntValue));
            else if(value != null && value.IsArray && value.ArrayIntValue != null)
                foreach (int actual in value.ArrayIntValue)
                    if (Global.CurrentPerson.Positions.Contains(actual))
                    {
                        isActual = true;
                        break;
                    }

            // Исполнитель
            value = TryGetAttribute(TaskConstants.ExecutorAttribute);
            if (value != null && value.IsArray && value.ArrayIntValue != null)
                foreach (int executor in value.ArrayIntValue)
                    if (Global.CurrentPerson.Positions.Contains(executor))
                    {
                        isExecutor = true;
                        break;
                    }

            // Инициатор
            value = TryGetAttribute(TaskConstants.InitiatorAttribute);
            if (value != null && value.IsArray && value.ArrayIntValue != null)
                foreach (int initiator in value.ArrayIntValue)
                    if (Global.CurrentPerson.Positions.Contains(initiator))
                    {
                        isInitiator = true;
                        break;
                    }

            // Аудитор
            value = TryGetAttribute(TaskConstants.AuditorsAttribute);
            if (value != null && value.IsArray && value.ArrayIntValue != null)
                foreach(int auditor in value.ArrayIntValue)
                    if(Global.CurrentPerson.Positions.Contains(auditor))
                    {
                        isAuditor = true;
                        break;
                    }

            // Ответственный
            value = TryGetAttribute(TaskConstants.ResponsibleAttribute);
            if (value != null && value.IsArray && value.ArrayIntValue != null)
                foreach (int responsible in value.ArrayIntValue)
                    if (Global.CurrentPerson.Positions.Contains(responsible))
                    {
                        isResponsible = true;
                        break;
                    }

            // Состояние
            UpdateState();
        }


        /// <summary>
        /// Получение значения атрибута
        /// </summary>
        /// <param name="attribute">сисстемное имя атрибута</param>
        /// <returns>возвращает значение или NULL, если атрибута нет</returns>
        private DValue TryGetAttribute(string attribute)
        {
            if (!dObject.Attributes.Keys.Contains(attribute))
                return null;

            return dObject.Attributes[attribute];
        }


        /// <summary>
        /// Получение состояния объекта
        /// </summary>
        public void UpdateState()
        {
            var value = TryGetAttribute(TaskConstants.StateAttribute);
            if (value != null && value.GuidValue != null)
            {
                Guid stateGuid = (Guid)value.GuidValue;

                if (stateGuid != null)
                    State = StateFabrique.GetState(stateGuid);
            }
        }


        /// <summary>
        /// Обновление данных объекта
        /// </summary>
        public override void UpdateObjectData()
        {
            base.UpdateObjectData();

            GetData();
        }


        /// <summary>
        /// Получить данные в соответствии с машиной состояний
        /// </summary>
        /// <param name="page">страница приложения</param>
        public void GetStateMachineData(ContentPage page)
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

            Guid _stateId = State.MUserState.Id;

            // Получение машины состояний
            if (StateMachine == null)
            {
                var attribute = Type.MType.Attributes.Where(a => a.Type == MAttrType.UserState).FirstOrDefault();
                if (attribute == null)
                    return;

                Guid _idStateMachine;
                Guid.TryParse(attribute.Configuration, out _idStateMachine);

                if (_idStateMachine == Guid.Empty)
                    return;

                StateMachine = Global.StateMachines.FirstOrDefault(sm => sm.Id == _idStateMachine);
            }

            // Получение следующего состояния
            if (StateMachine != null)
            {
                avaliableStates = new List<PState>();

                if (StateMachine.StateTransitions != null && StateMachine.StateTransitions.Any(st => st.Key == _stateId))
                {
                    foreach (var stateTransition in StateMachine.StateTransitions.Where(t => t.Key == _stateId))
                    {
                        if (stateTransition.Value != null)
                        {
                            foreach (MTransition transition in stateTransition.Value)
                            {
                                if (transition.AvailableForPositionsSource != null && !avaliableStates.Any(s => s.Guid.ToString() == transition.StateTo.ToString()))
                                {
                                    if ( transition.AvailableForPositionsSource.Length == 0 ||
                                        (IsActual && transition.AvailableForPositionsSource.Contains("&" + RolesConstants.Actual)) ||
                                        (IsExecutor && transition.AvailableForPositionsSource.Contains("&" + RolesConstants.Executor)) ||
                                        (IsInitiator && transition.AvailableForPositionsSource.Contains("&" + RolesConstants.Initiator)) ||
                                        (IsResponsible && transition.AvailableForPositionsSource.Contains("&" + RolesConstants.Responsible)) ||
                                        (IsAuditor && transition.AvailableForPositionsSource.Contains("&" + RolesConstants.Auditors)))
                                    {
                                        avaliableStates.Add(StateFabrique.GetState(transition.StateTo));
                                    }
                                }
                            }
                        }
                    }

                    if (avaliableStates.Count > 0)
                        avaliableStates = avaliableStates.OrderBy(s => s.MUserState.Title).ToList();
                }
            }
        }


        public void SetState(PState state)
        {
            State = state;
        }

        #endregion
    }
}