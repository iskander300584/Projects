using Ascon.Pilot.Common;
using Ascon.Pilot.DataClasses;
using Ascon.Pilot.Server.Api;
using Ascon.Pilot.Server.Api.Contracts;
using PilotMobile.Models;
using System;
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
    }

    class Context : IContext, INotifyPropertyChanged
    {
        private Repository _repository;
        private HttpPilotClient _client;
        private readonly ServerCallback _serverCallback;
        private EventsCallBack _eventsCallback;
        

        public IRepository Repository => _repository;


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
            _eventsCallback = new EventsCallBack();
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
                _eventsCallback.SetCallbackListener(_repository);
                _repository.SetEventsApi(_client.GetEventsApi(_eventsCallback));
                var _messagesApi = _client.GetMessagesApi(new NullableMessagesCallback());
                _repository.SetMessagesApi(_messagesApi);
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