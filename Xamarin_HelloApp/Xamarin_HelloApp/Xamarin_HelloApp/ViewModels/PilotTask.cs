using Ascon.Pilot.DataClasses;
using PilotMobile.AppContext;
using PilotMobile.Models;
using System;
using System.Linq;
using Xamarin_HelloApp.AppContext;

namespace PilotMobile.ViewModels
{
    /// <summary>
    /// Задание Pilot
    /// </summary>
    public class PilotTask : IPilotObject
    {
        private PState state;
        /// <summary>
        /// Состояние задания
        /// </summary>
        public PState State
        {
            get => state;
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
            value = TryGetAttribute(TaskConstants.StateAttribute);
                if(value != null && value.GuidValue != null)
            {
                Guid stateGuid = (Guid)value.GuidValue;

                if (stateGuid != null)
                    state = StateFabrique.GetState(stateGuid);
            }

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
    }
}