using Ascon.Pilot.Common;
using Ascon.Pilot.DataClasses;
using Ascon.Pilot.DataModifier;
using Ascon.Pilot.Server.Api;
using BackendImpl;
using ModifierSample;
using PilotMobile.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace Xamarin_HelloApp.Models
{
    public interface IContext : IDisposable
    {
        //void Build(HttpContext http);
        IRepository Repository { get; }
        DDatabaseInfo ConnectOld(Credentials credentials);
        Exception Connect(Credentials credentials);
        bool IsInitialized { get; }
        List<DRule> Rules { get; }
    }

    class Context : IContext, INotifyPropertyChanged
    {
        private Repository _repository;
        private HttpPilotClient _client;
        private readonly ServerCallback _serverCallback;
        private EventsCallBack _eventsCallback;
        

        public IRepository Repository => _repository;
        private List<DRule> _rules;
        public List<DRule> Rules
        {
            get => _rules;
        }


        private bool isInitialized = false;
        /// <summary>
        /// Подключение выполнено
        /// </summary>
        public bool IsInitialized
        {
            get => isInitialized;
            private set
            {
                if(isInitialized != value)
                {
                    isInitialized = value;
                    OnPropertyChanged();
                }
            }
        }

        public Context()
        {
            _serverCallback = new ServerCallback(); 
        }

        public DDatabaseInfo ConnectOld(Credentials credentials)
        {
            _client = new HttpPilotClient(@"http://ecm.ascon.ru:5545");
            // Do not check versions of the Server and Client
            _client.Connect(false);
            var serverApi = _client.GetServerApi(_serverCallback);
            var authApi = _client.GetAuthenticationApi();
            authApi.Login("pilot_ascon", "ryapolov_an", "sSR4mzCQ".EncryptAes(), false, 103);
            var dbInfo = serverApi.OpenDatabase();
            _repository = new Repository(serverApi, _serverCallback);
            _repository.Initialize(credentials.Username);
            IsInitialized = true;
            return dbInfo;
        }


        /// <summary>
        /// Подключение к БД
        /// </summary>
        /// <param name="credentials">настройки подключения</param>
        /// <returns>возвращает ошибку подключения или NULL, если ошибки не произошло</returns>
        public Exception Connect(Credentials credentials)
        {
            Exception ex = null;

            try
            {
                _client = new HttpPilotClient(credentials.ServerUrl);
                _client.Connect(false);
                
                var serverApi = _client.GetServerApi(_serverCallback);
                var authApi = _client.GetAuthenticationApi();

                int license = (credentials.License != null && credentials.License != 0) ? credentials.License : 103;

                authApi.Login(credentials.DatabaseName, credentials.Username, credentials.ProtectedPassword, false, credentials.License);
                var dbInfo = serverApi.OpenDatabase();
                _repository = new Repository(serverApi, _serverCallback);
                _repository.Initialize(credentials.Username);
                IsInitialized = true;
                _repository.SetHttpClient(_client);

                _rules = new List<DRule>();
                _rules.Add(new DRule
                {
                    Id = new Guid("{853A5C30-5B36-4076-89F5-CA4764DEEB7F}"),
                    FileExtension = ".pdf",
                    ChangeType = ChangeType.Create
                });

                _rules.Add(new DRule
                {
                    Id = new Guid("{0B0FE415-6FB3-4C5B-9301-2420477CBBFE}"),
                    FileExtension = ".xps",
                    ChangeType = ChangeType.Create
                });

                _rules.Add(new DRule
                {
                    Id = new Guid("{C3E6454C-4901-44E9-BA67-8F861A06BB30}"),
                    FileExtension = ".txt",
                    ChangeType = ChangeType.Update
                });

                _rules.Add(new DRule
                {
                    Id = new Guid("{395CE896-AC8C-4037-B99E-D759426621CF}"),
                    FileExtension = ".xps",
                    ChangeType = ChangeType.Delete
                });

                _rules.Add(new DRule
                {
                    Id = new Guid(),
                    FileExtension = ".xps",
                    ChangeType = ChangeType.Update
                });

                _eventsCallback = new EventsCallBack(_rules, _repository.AcceptChanges, _repository.PrintChangeDetails);

                _eventsCallback.SetCallbackListener(_repository);
                var _eventsApi = _client.GetEventsApi(_eventsCallback);
                _eventsApi.SubscribeChanges(_rules);
                _repository.SetEventsApi(_eventsApi);
                
                var _messagesApi = _client.GetMessagesApi(new NullableMessagesCallback());
                _repository.SetMessagesApi(_messagesApi);

                var fileApi = _client.GetFileArchiveApi();
                var localArchiveRootFolder = DirectoryHelper.GetTempPath();
                var _fileStorageProvider = new FileStorageProvider(localArchiveRootFolder);
                var changesetUploader = new ChangesetUploader(fileApi, _fileStorageProvider, null);
                Backend backend = new Backend(serverApi, dbInfo, _messagesApi, changesetUploader);
                _repository.SetBackend(backend);
            }
            catch(Exception exception)
            {
                ex = exception;
            }

            return ex;
        }

        public void Build()
        {
            if (IsInitialized)
                return;
            
            var creds = Credentials.GetConnectionCredentials("pilot_ascon", "ryapolov_an", "sSR4mzCQ");
            ConnectOld(creds);
        }

        public void Dispose()
        {
            _client?.Disconnect();
            _client?.Dispose();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}