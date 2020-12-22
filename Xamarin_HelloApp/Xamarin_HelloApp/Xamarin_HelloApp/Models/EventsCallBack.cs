using Ascon.Pilot.DataClasses;
using Ascon.Pilot.Server.Api.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin_HelloApp.Models;

namespace PilotMobile.Models
{
    class EventsCallBack : IEventsCallback
    {
        private readonly List<DRule> _rules;
        private readonly Action<Guid, Guid> _acceptAction;
        private readonly Action<IEnumerable<DChangesetData>, DRule, NotifyResult> _printChangeDetails;

        private IRemoteStorageListener _listener;


        public EventsCallBack(List<DRule> rules, Action<Guid, Guid> acceptAction, Action<IEnumerable<DChangesetData>, DRule, NotifyResult > printChangeDetails)
        {
            _rules = rules;
            _acceptAction = acceptAction;
            _printChangeDetails = printChangeDetails;
        }


        public void SetCallbackListener(IRemoteStorageListener listener)
        {
            _listener = listener;
        }

        public void NotifyChange(Guid ruleId, DChangesetData change)
        {
            var rule = _rules.FirstOrDefault(x => x.Id == ruleId);
            if(rule != null)
            {
                _printChangeDetails(new List<DChangesetData>() { change }, rule, new NotifyResult());
            }

            _acceptAction(change.Identity, ruleId);
        }
    }
}