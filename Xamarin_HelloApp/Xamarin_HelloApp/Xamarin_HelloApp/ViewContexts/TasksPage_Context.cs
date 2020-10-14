using Ascon.Pilot.DataClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Xamarin_HelloApp.AppContext;
using Xamarin_HelloApp.Models;

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
        private async void AsyncGetTasks()
        {
            List<Guid> _taskGuids = new List<Guid>();

            // формирование условия поиска по типам заданий
            string searchType = @"+DObject\.TypeId:(";
            foreach (PType pType in TypeFabrique.GetAllTypes().Where(t => t.IsTask))
                searchType += @"&#32;" + pType.ID + " OR ";

            searchType = searchType.Remove(searchType.Length - 4).Trim();
            searchType +=  ")";

            List<int> units = Global.CurrentPerson.Positions;
            foreach (int unit in units)
            {
                // Формирование условия поиска по атрибуту
                string searchAttribute = @"+(t\.actualFor:&#32;" + unit.ToString() + ")";

                // Формирование общего условия поиска
                string search = searchType + " " + searchAttribute;
                DSearchDefinition searchDefinition = new DSearchDefinition
                {
                    Id = Guid.NewGuid(),
                    Request =
                    {
                    MaxResults = 100,
                    SearchKind = (SearchKind)0,
                    SearchString = search,
                    SortDefinitions =
                    {
                        new DSortDefinition {
                            Ascending = false,
                            FieldName = SystemAttributes.TASK_DATE_OF_ASSIGNMENT
                        }
                    }
                    }
                };

                IEnumerable<Guid> _guids = await GetTaskGuidList(searchDefinition);

                if (_guids != null)
                    foreach (Guid guid in _guids)
                        if (!_taskGuids.Contains(guid))
                            _taskGuids.Add(guid);
            }


            // Всегда возвращает NULL
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
        /// Получение списка Guid заданий
        /// </summary>
        /// <param name="searchDefinition">условие поиска</param>
        /// <returns></returns>
        private async Task<IEnumerable<Guid>> GetTaskGuidList(DSearchDefinition searchDefinition)
        {
            DSearchResult result = await Global.DALContext.Repository.Search(searchDefinition);

            return result.Found;
        }
    }
}