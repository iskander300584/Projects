using Ascon.Pilot.DataClasses;
using Ascon.Pilot.Server.Api.Contracts;
using System;


namespace Xamarin_HelloApp.Models
{
    public interface IRemoteServiceListener
    {
        void Notify(DMetadataChangeset changeset);
        void Notify(OrganisationUnitChangeset changeset);
        void Notify(PersonChangeset changeset);
        void Notify(DNotificationChangeset changeset);
    }

    public interface IRemoteSearchServiceListener
    {
        void Notify(DSearchResult result);
    }

    interface IRemoteStorageListener
    {
        void Notify(DSearchResult result);
    }

    class ServerCallback : IServerCallback
    {
        private IRemoteStorageListener _listener;
        private IRemoteServiceListener _serviceListener;


        public void SetCallbackListener(IRemoteStorageListener listener, IRemoteServiceListener serviceListener)
        {
            _listener = listener;
            _serviceListener = serviceListener;
        }

        public void NotifyChangeset(DChangeset changeset)
        {

        }

        public void NotifyCommandResult(Guid requestId, byte[] data, ServerCommandResult result)
        {

        }

        public void NotifyDMetadataChangeset(DMetadataChangeset changeset)
        {

        }

        public void NotifyDNotificationChangeset(DNotificationChangeset changeset)
        {
            try
            {
                _serviceListener?.Notify(changeset);
            }
            catch { }
        }

        public void NotifyGeometrySearchResult(DGeometrySearchResult searchResult)
        {

        }

        public void NotifyOrganisationUnitChangeset(OrganisationUnitChangeset changeset)
        {

        }

        public void NotifyPersonChangeset(PersonChangeset changeset)
        {

        }

        public void NotifySearchResult(DSearchResult searchResult)
        {
            _listener?.Notify(searchResult);
        }
    }
}