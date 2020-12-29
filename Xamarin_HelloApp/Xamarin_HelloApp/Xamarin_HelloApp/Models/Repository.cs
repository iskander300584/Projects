using Ascon.Pilot.DataClasses;
using Ascon.Pilot.DataModifier;
using Ascon.Pilot.Server.Api;
using Ascon.Pilot.Server.Api.Contracts;
using BackendImpl;
using PilotMobile.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Xamarin_HelloApp.Models
{
    public interface IRepository
    {
        Task<DSearchResult> Search(DSearchDefinition searchDefinition);
        List<DObject> GetObjects(Guid[] ids);
        DPerson GetPersonOnOrganisationUnit(int id);
        DPerson GetPerson(int id);
        IEnumerable<DPerson> GetPersons(string login);
        DOrganisationUnit GetOrganisationUnit(int id);
        DPerson CurrentPerson();
        MType GetType(int id);
        IEnumerable<MType> GetTypes();
        byte[] GetFileChunk(Guid id, long pos, int count);
        List<MUserState> GetStates();
        //List<Tuple<Guid, DChangesetData[]>> GetNotifies();
        DNotificationChangeset GetNotifies();
        public void AcceptChanges(Guid changesetId, Guid ruleId);
        public void PrintChangeDetails(IEnumerable<DChangesetData> changes, DRule rule, NotifyResult result);

        /// <summary>
        /// Задать класс удаленного подключения
        /// </summary>
        /// <param name="client">класс удаленного подключения</param>
        public void SetHttpClient(HttpPilotClient client);

        /// <summary>
        /// Получить список машин состояний
        /// </summary>
        public List<MUserStateMachine> GetStateMachines();

        /// <summary>
        /// Задать Backend
        /// </summary>
        /// <param name="backend"></param>
        public void SetBackend(Backend backend);

        /// <summary>
        /// Получить интерфейс для изменения данных
        /// </summary>
        public IModifier GetModifier();

        public void SaveChanges(DChangesetData dChangesetData);

    }

    class Repository : IRepository, IRemoteStorageListener, IRemoteServiceListener
    {
        private readonly IServerApi _serverApi;
        private Dictionary<int, DPerson> _persons = new Dictionary<int, DPerson>();
        private Dictionary<int, DOrganisationUnit> _organisationUnits = new Dictionary<int, DOrganisationUnit>();
        private TaskCompletionSource<DSearchResult> _searchCompletionSource;
        private TaskCompletionSource<DNotificationChangeset> _notificationCompletionSource;
        private DPerson _person;
        private List<MType> _types;
        private IEventsApi _eventsApi;
        private IMessagesApi _messagesApi;
        private HttpPilotClient _client;
        private Backend _backend;
        private ServerCallback _serverCallback;

        public Repository(IServerApi serverApi, ServerCallback serverCallback)
        {
            _serverApi = serverApi;
            var info = _serverApi.GetDatabaseInfo();

            _serverCallback = serverCallback;
            serverCallback.SetCallbackListener(this, this);
        }

        public void Initialize(string currentLogin)
        {
            _persons = _serverApi.LoadPeople().ToDictionary(x => x.Id, y => y);
            _organisationUnits = _serverApi.LoadOrganisationUnits().ToDictionary(x => x.Id, y => y);
            _person = _persons.First(p => p.Value.Login.Equals(currentLogin, StringComparison.OrdinalIgnoreCase) && !p.Value.IsDeleted && !p.Value.IsInactive).Value;
            _types = _serverApi.GetMetadata(0).Types;
        }

        public Task<DSearchResult> Search(DSearchDefinition searchDefinition)
        {
            _searchCompletionSource = new TaskCompletionSource<DSearchResult>();
            _serverApi.AddSearch(searchDefinition);
            return _searchCompletionSource.Task;
        }

        public List<DObject> GetObjects(Guid[] ids)
        {
            List<DObject> objects = new List<DObject>();

            try
            {
                objects = _serverApi.GetObjects(ids);
            }
            catch { }

            return objects;
        }

        public DPerson GetPersonOnOrganisationUnit(int id)
        {
            return _persons.Values.Where(x => x.Positions.Contains(id))
                .OrderBy(x => x.Positions.IndexOf(id))
                .FirstOrDefault();
        }

        public DPerson GetPerson(int id)
        {
            return _persons.Values.FirstOrDefault(p => p.Id.Equals(id));
        }

        public IEnumerable<DPerson> GetPersons(string login)
        {
            return _persons.Values.Where(p => p.Login.ToLower() == login.ToLower());
        }

        public DOrganisationUnit GetOrganisationUnit(int id)
        {
            DOrganisationUnit result;
            if (!_organisationUnits.TryGetValue(id, out result))
                return new DOrganisationUnit();
            return result;
        }

        public MType GetType(int id)
        {
            return _types.FirstOrDefault(t => t.Id == id);
        }

        public DPerson CurrentPerson()
        {
            return _person;
        }

        

        public IEnumerable<MType> GetTypes()
        {
            return _types;
        }

        public byte[] GetFileChunk(Guid id, long pos, int count)
        {
            return _serverApi.GetFileChunk(id, pos, count);
        }

        public List<MUserState> GetStates()
        {
            var info = _serverApi.GetDatabaseInfo();
            var metaData = _serverApi.GetMetadata(info.MetadataVersion);

            return metaData.UserStates;
        }

        public void SetEventsApi(IEventsApi eventsApi)
        {
            _eventsApi = eventsApi;
        }


        
        
        public void SetMessagesApi(IMessagesApi messagesApi)
        {
            _messagesApi = messagesApi;
        }

        public void GetMessages()
        {
            
        }

        public void AcceptChanges(Guid changesetId, Guid ruleId)
        {
            _eventsApi.AcceptChange(changesetId, ruleId);
        }

        public void PrintChangeDetails(IEnumerable<DChangesetData> changes, DRule rule, NotifyResult result)
        {
            result.Result = new List<string>();

            foreach (var dChangesetData in changes)
            {
                foreach (var change in dChangesetData.Changes)
                {
                    var name = "";
                    if (!change.New.ActualFileSnapshot.Files.Any())
                    {
                        var type = _types.First(x => x.Id == change.New.TypeId);
                        if(type.IsTaskType())
                        {
                            var attrs = type.Attributes.Where(x => x.ShowInTree).OrderBy(y => y.DisplaySortOrder);
                            foreach (var mAttribute in attrs)
                            {
                                DValue value;
                                change.New.Attributes.TryGetValue(mAttribute.Name, out value);
                                if (value != null)
                                    name += " " + value.StrValue;
                            }
                        }
                        

                        //if (change.New.Attributes.ContainsKey(SystemAttributes.PROJECT_ITEM_NAME))
                        //{
                        //    name = change.New.Attributes[SystemAttributes.PROJECT_ITEM_NAME];
                        //    result.Result.Add(name);
                        //}


                    }
                    /*if (change.New.Attributes.ContainsKey(SystemAttributes.TASK_STATE))
                    {
                        name = change.New.Attributes[SystemAttributes.TASK_STATE];
                        result.Result.Add(GetRuleText(rule) + " файл " + name);
                    }
                    else
                    {
                        if (!change.New.ActualFileSnapshot.Files.Any())
                            continue;

                        var type = _types.First(x => x.Id == change.New.TypeId);
                        var attrs = type.Attributes.Where(x => x.ShowInTree).OrderBy(y => y.DisplaySortOrder);
                        foreach (var mAttribute in attrs)
                        {
                            DValue value;
                            change.New.Attributes.TryGetValue(mAttribute.Name, out value);
                            if (value != null)
                                name += " " + value.StrValue;
                        }

                        if (change.Old == null)
                        {
                            result.Result.Add(GetRuleText(rule) + " документ " + name);
                        }
                        else
                        {
                            result.Result.Add(GetRuleText(rule) + " а версия документа " + name);
                        }
                    }*/
                }
            }
        }

        private string GetRuleText(DRule rule)
        {
            switch (rule.ChangeType)
            {
                case ChangeType.Create:
                    return "Создан";
                case ChangeType.Delete:
                    return "Удален";
                case ChangeType.Update:
                    return "Изменен";
            }

            return "";
        }

        public void SetHttpClient(HttpPilotClient client)
        {
            _client = client;
        }


        /// <summary>
        /// Получить список машин состояний
        /// </summary>
        public List<MUserStateMachine> GetStateMachines()
        {
            DMetadata metadata = _serverApi.GetMetadata(0);
            return metadata.StateMachines;
        }

        public void SetBackend(Backend backend)
        {
            _backend = backend;
        }


        /// <summary>
        /// Получить интерфейс для изменения данных
        /// </summary>
        public IModifier GetModifier()
        {
            return new Modifier(_backend);
        }


        public void SaveChanges(DChangesetData dChangesetData)
        {
            _serverApi.Change(dChangesetData);
        }

        /*public List<Tuple<Guid, DChangesetData[]>> GetNotifies()
        {
            //_serverCallback.NotifyDNotificationChangeset(new DNotificationChangeset());
            return _eventsApi.GetMissedChanges();
        }*/

        public DNotificationChangeset GetNotifies()
        {
            _notificationCompletionSource = new TaskCompletionSource<DNotificationChangeset>();
            _serverCallback.NotifyDNotificationChangeset(new DNotificationChangeset());
            return _notificationCompletionSource.Task.Result;
        }

        public void Notify(DMetadataChangeset changeset)
        {
            
        }

        public void Notify(OrganisationUnitChangeset changeset)
        {
            
        }

        public void Notify(PersonChangeset changeset)
        {
            
        }

        public void Notify(DNotificationChangeset changeset)
        {
            try
            {
                //_serverCallback.NotifyDNotificationChangeset(changeset);
                _notificationCompletionSource.SetResult(changeset);
            }
            catch (Exception ex)
            {
                if(ex != null)
                {

                }
            }
        }

        public void Notify(DSearchResult result)
        {
            try
            {
                _searchCompletionSource.SetResult(result);
            }
            catch (Exception)
            {
                ;
            }
        }
    }
}