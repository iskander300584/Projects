using Ascon.Pilot.DataClasses;
using PilotMobile.AppContext;
using PilotMobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin_HelloApp.AppContext;
using Xamarin_HelloApp.Models;

namespace PilotMobile.ViewContexts
{
    /// <summary>
    /// Контекст данных окна заданий
    /// </summary>
    class TasksPage_Context : INotifyPropertyChanged
    {
        #region Поля класса

        /// <summary>
        /// Наименование приложения
        /// </summary>
        public string AppName
        {
            get => StringConstants.ApplicationName;
        }


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

                    SearchTasks();
                }
            }
        }


        private ObservableCollection<PilotTask> tasks = new ObservableCollection<PilotTask>();
        /// <summary>
        /// Список заданий
        /// </summary>
        public ObservableCollection<PilotTask> Tasks
        {
            get => tasks;
            private set
            {
                if(tasks != value)
                {
                    tasks = value;
                    OnPropertyChanged();
                }
            }
        }


        /// <summary>
        /// Список всех заданий
        /// </summary>
        private List<PilotTask> _allTasks = new List<PilotTask>();


        private ICommand updateCommand;
        /// <summary>
        /// Команда Обновить
        /// </summary>
        public ICommand UpdateCommand
        {
            get => updateCommand;
        }


        /// <summary>
        /// Актуальные задания получены
        /// </summary>
        private bool _actualTaken;


        /// <summary>
        /// Выданные задания получены
        /// </summary>
        private bool _initiatorTaken;


        /// <summary>
        /// Полученные задания получены
        /// </summary>
        private bool _executorTaken;


        /// <summary>
        /// Ответственные задания получены
        /// </summary>
        private bool _responsibleTaken;


        /// <summary>
        /// Аудиторские задания получены
        /// </summary>
        private bool _auditorTaken;


        /// <summary>
        /// Все задания получены
        /// </summary>
        private bool _allTaken = false;


        #endregion


        /// <summary>
        /// Контекст данных окна заданий
        /// </summary>
        public TasksPage_Context()
        {
            updateCommand = new Command(GetTaskList);

            FillTaskFilter();

            GetTaskList();
        }


        #region Методы класса


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

            Global.CurrentPerson = Global.DALContext.Repository.GetPerson(Global.CurrentPerson.Id);

            ClearMarkers();

            SearchTasks();
        }


        /// <summary>
        /// Выполнить поиск заданий
        /// </summary>
        private void SearchTasks()
        {
            Thread thread = new Thread(AsyncGetTasks);
            thread.Start();
        }


        /// <summary>
        /// Обнуление маркеров полученных заданий
        /// </summary>
        private void ClearMarkers()
        {
            _actualTaken = false;
            _initiatorTaken = false;
            _executorTaken = false;
            _responsibleTaken = false;
            _auditorTaken = false;
            _allTaken = false;
        }


        /// <summary>
        /// Выбор актуального списка заданий
        /// </summary>
        private void SelectVisibleTasks()
        {
            Tasks = new ObservableCollection<PilotTask>();

            switch(selectedFilterIndex)
            {
                // Актуальные
                case 0:
                    foreach (PilotTask task in _allTasks.Where(t => t.IsActual))
                        Tasks.Add(task);
                    break;

                // Выданные
                case 1:
                    foreach (PilotTask task in _allTasks.Where(t => t.IsInitiator))
                        Tasks.Add(task);
                    break;

                // Полученные
                case 2:
                    foreach (PilotTask task in _allTasks.Where(t => t.IsExecutor))
                        Tasks.Add(task);
                    break;

                // Просроченные
                case 3:
                    foreach (PilotTask task in _allTasks.Where(t => t.Deadline != null && t.Deadline > DateTime.Today && t.State.Name != "done"))
                        Tasks.Add(task);
                    break;

                // Ответственный
                case 4:
                    foreach (PilotTask task in _allTasks.Where(t => t.IsResponsible))
                        Tasks.Add(task);
                    break;

                // Аудитор
                case 5:
                    foreach (PilotTask task in _allTasks.Where(t => t.IsAuditor))
                        Tasks.Add(task);
                    break;

                // Отозванные
                case 6:
                    foreach (PilotTask task in _allTasks.Where(t => t.State.Name == TaskConstants.RevokedState))
                        Tasks.Add(task);
                    break;

                // Все
                case 7:
                    foreach (PilotTask task in _allTasks)
                        Tasks.Add(task);
                    break;
            }
        }


        /// <summary>
        /// Метод получения списка заданий в отдельном потоке
        /// </summary>
        private async void AsyncGetTasks()
        {
            List<Guid> _taskGuids = new List<Guid>();

            // Проверка необходимости выполнения поиска в БД
            if (CheckNeedSearch())
            {
                // формирование условия поиска по типам заданий
                string searchType = @"+DObject\.TypeId:(";
                foreach (PType pType in TypeFabrique.GetAllTypes().Where(t => t.IsTask))
                    searchType += @"&#32;" + pType.ID + " OR ";

                searchType = searchType.Remove(searchType.Length - 4).Trim();
                searchType += ")";

                // Поиск по должностям
                List<int> units = Global.CurrentPerson.Positions;
                foreach (int unit in units)
                {
                    IEnumerable<Guid> _guids = await GetTaskGuidList(searchType, unit);

                    if (_guids != null)
                        foreach (Guid guid in _guids)
                            if (!_taskGuids.Contains(guid))
                                _taskGuids.Add(guid);
                }

                foreach (Guid guid in _taskGuids)
                    if (!_allTasks.Any(t => t.Guid == guid))
                        _allTasks.Add(new PilotTask(guid));

                SetMarkers();
            }

            SelectVisibleTasks();
        }


        /// <summary>
        /// Получение списка Guid заданий
        /// </summary>
        /// <param name="searchType">условие поиска по типам</param>
        /// <param name="unit">ID должности пользователя</param>
        /// <returns>возвращает массив GUID заданий или NULL, если задания не найдены</returns>
        private async Task<IEnumerable<Guid>> GetTaskGuidList(string searchType, int unit)
        {
            string searchAttribute = GetSearchQuery(unit);

            // Формирование общего условия поиска
            string search = searchType + " " + searchAttribute;
            DSearchDefinition searchDefinition = new DSearchDefinition
            {
                Id = Guid.NewGuid(),
                Request =
                    {
                    MaxResults = 1000,
                    SearchKind = SearchKind.Custom,
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

            DSearchResult result = await Global.DALContext.Repository.Search(searchDefinition);

            return result.Found;
        }


        /// <summary>
        /// Проверка необходимости выполнить поиск заданий в БД
        /// </summary>
        /// <returns>возвращает TRUE, если необходимо выполнить поиск заданий</returns>
        private bool CheckNeedSearch()
        {
            return (!_allTaken && (
                (SelectedFilterIndex == 0 && !_actualTaken) ||
                (SelectedFilterIndex == 1 && !_initiatorTaken) ||
                (SelectedFilterIndex == 2 && !_executorTaken) ||
                (SelectedFilterIndex == 4 && !_responsibleTaken) ||
                (SelectedFilterIndex == 5 && !_auditorTaken) ||
                SelectedFilterIndex == 3 ||
                SelectedFilterIndex == 6 ||
                SelectedFilterIndex == 7
                ));
        }


        /// <summary>
        /// Возвращает строку запроса по атрибутам
        /// </summary>
        /// <param name="unit">ID организационной единицы</param>
        private string GetSearchQuery(int unit)
        {
            string query = string.Empty;

            switch(SelectedFilterIndex)
            {
                // Актуальные
                case 0:
                    query = $@"+(i32\.actualFor:(&#32;{unit}))";
                    break;

                // Выданные
                case 1:
                    query = $@"+(i32\.initiator:(&#32;{unit}))";
                    break;

                // Полученные
                case 2:
                    query = $@"+(i32\.executor:(&#32;{unit}))";
                    break;

                // Ответственный
                case 4:
                    query = $@"+(i32\.responsible:(&#32;{unit}))";
                    break;

                // Аудитор
                case 5:
                    query = $@"+(i32\.auditors:(&#32;{unit}))";
                    break;

                // Остальные
                default:
                    query = $@"+(i32\.actualFor:(&#32;{unit}) OR i32\.initiator:(&#32;{unit}) OR i32\.executor:(&#32;{unit}) OR i32\.auditors:(&#32;{unit}) OR i32\.responsible:(&#32;{unit}))";
                    break;
            }

            return query;
        }


        /// <summary>
        /// Установить маркеры полученных заданий
        /// </summary>
        private void SetMarkers()
        {
            switch (SelectedFilterIndex)
            {

                // Актуальные
                case 0:
                    _actualTaken = true;
                    break;

                // Выданные
                case 1:
                    _initiatorTaken = true;
                    break;

                // Полученные
                case 2:
                    _executorTaken = true;
                    break;

                // Ответственный
                case 4:
                    _responsibleTaken = true;
                    break;

                // Аудитор
                case 5:
                    _auditorTaken = true;
                    break;

                // Остальные
                default:
                    _allTaken = true;
                    break;
            }
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