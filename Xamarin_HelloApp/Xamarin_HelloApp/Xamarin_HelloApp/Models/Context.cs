using Ascon.Pilot.Common;
using Ascon.Pilot.DataClasses;
using Ascon.Pilot.Server.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Xamarin_HelloApp.Models
{
    public interface IContext : IDisposable
    {
        //void Build(HttpContext http);
        IRepository Repository { get; }
        DDatabaseInfo Connect(Credentials credentials);
        bool IsInitialized { get; }
    }

    class Context : IContext, INotifyPropertyChanged
    {
        private Repository _repository;
        private HttpPilotClient _client;
        private readonly ServerCallback _serverCallback;

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
        }

        public DDatabaseInfo Connect(Credentials credentials)
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

        public void Build()
        {
            if (IsInitialized)
                return;
            
            var creds = Credentials.GetConnectionCredentials("pilot_ascon", "ryapolov_an", "sSR4mzCQ");
            Connect(creds);
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