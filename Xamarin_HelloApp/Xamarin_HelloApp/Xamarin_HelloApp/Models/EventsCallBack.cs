using Ascon.Pilot.DataClasses;
using Ascon.Pilot.Server.Api.Contracts;
using System;
using Xamarin_HelloApp.Models;

namespace PilotMobile.Models
{
    class EventsCallBack : IEventsCallback
    {
        private IRemoteStorageListener _listener;

        public void SetCallbackListener(IRemoteStorageListener listener)
        {
            _listener = listener;
        }

        public void NotifyChange(Guid ruleId, DChangesetData change)
        {
            
        }
    }
}