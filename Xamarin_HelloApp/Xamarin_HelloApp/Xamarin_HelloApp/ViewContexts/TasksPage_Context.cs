using Ascon.Pilot.DataClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Xamarin_HelloApp.AppContext;

namespace PilotMobile.ViewContexts
{
    /// <summary>
    /// Контекст данных окна заданий
    /// </summary>
    class TasksPage_Context : INotifyPropertyChanged
    {
        private List<string> taskFilter = new List<string>();
        /// <summary>
        /// Фильтр заданий
        /// </summary>
        public List<string> TaskFilter
        {
            get => taskFilter;
        }


        private int selectedFilterIndex = 0;
        /// <summary>
        /// Номер выбранного элемента из фильтра
        /// </summary>
        public int SelectedFilterIndex
        {
            get => selectedFilterIndex;
            set
            {
                if(selectedFilterIndex != value)
                {
                    selectedFilterIndex = value;
                    OnPropertyChanged();

                    GetTaskList();
                }
            }
        }


        private ObservableCollection<DObject> tasks = new ObservableCollection<DObject>();
        /// <summary>
        /// Список заданий
        /// </summary>
        public ObservableCollection<DObject> Tasks
        {
            get => tasks;
        }


        /// <summary>
        /// Список всех заданий
        /// </summary>
        private List<DObject> _allTasks = new List<DObject>();


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }


        /// <summary>
        /// Контекст данных окна заданий
        /// </summary>
        public TasksPage_Context()
        {
            FillTaskFilter();

            GetTaskList();
        }


        /// <summary>
        /// Заполнение списка заданий
        /// </summary>
        private void FillTaskFilter()
        {
            taskFilter.Add("Актуальные");
            taskFilter.Add("Выданные");
            taskFilter.Add("Полученные");
            taskFilter.Add("Просроченные");
            taskFilter.Add("Вы в ответственных");
            taskFilter.Add("Вы в аудиторах");
            taskFilter.Add("Отозванные");
            taskFilter.Add("Все");
        }


        /// <summary>
        /// Получение списка заданий
        /// </summary>
        private void GetTaskList()
        {
            tasks.Clear();
            _allTasks.Clear();

            Thread thread = new Thread(AsyncGetTasks);
            thread.Start();
        }


        /// <summary>
        /// Метод получения списка заданий в отдельном потоке
        /// </summary>
        private void AsyncGetTasks()
        {
            DObject rootObj = Global.DALContext.Repository.GetObjects(new[] { DObject.TaskRootId }).First();
            if (rootObj == null)
                return;

            RecurseGetTasks(rootObj);

            //DSearchDefinition searchDefinition = new DSearchDefinition
            //{
            //    Id = Guid.NewGuid(),
            //    Request =
            //    {
            //        MaxResults = 100,
            //        SearchKind = (SearchKind)5,
            //        SortDefinitions =
            //        {
            //            new DSortDefinition {
            //                Ascending = false,
            //                FieldName = SystemAttributes.TASK_DATE_OF_ASSIGNMENT
            //            }
            //        }
            //    }
            //};

            //DSearchResult result = await Global.DALContext.Repository.Search(searchDefinition);

            //if (result == null || result.Found == null)
            //    return;
        }


        /// <summary>
        /// Рекурсивное получение заданий
        /// </summary>
        /// <param name="parent">объект Pilot</param>
        private void RecurseGetTasks(DObject parent)
        {
            List<Guid> _guids = new List<Guid>();
            foreach (DChild dChild in parent.Children)
                _guids.Add(dChild.ObjectId);

            List<DObject> _dObjects = Global.DALContext.Repository.GetObjects(_guids.ToArray());

            foreach(DObject dObject in _dObjects)
            {
                if(dObject.IsTask())
                {
                    _allTasks.Add(dObject);
                    Tasks.Add(dObject);
                }

                if (dObject.Children.Count > 0)
                    RecurseGetTasks(dObject);
            }
        }
    }
}