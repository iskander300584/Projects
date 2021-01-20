using Ascon.Pilot.DataClasses;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Xamarin_HelloApp.AppContext;
using Xamarin_HelloApp.Models;

namespace PilotMobile.ViewModels
{
    public abstract class IPilotObject : INotifyPropertyChanged
    {
        protected Guid guid = new Guid();
        /// <summary>
        /// GUID
        /// </summary>
        public Guid Guid
        {
            get => guid;
        }


        protected DObject dObject = null;
        /// <summary>
        /// Объект Pilot
        /// </summary>
        public DObject DObject
        {
            get => dObject;
        }


        protected IPilotObject parent;
        /// <summary>
        /// Головной объект
        /// </summary>
        public IPilotObject Parent
        {
            get => parent;
            set
            {
                if(parent != value)
                {
                    parent = value;
                    OnPropertyChanged();
                }
            }
        }


        protected ObservableCollection<IPilotObject> children = new ObservableCollection<IPilotObject>();
        /// <summary>
        /// Коллекция потомков
        /// </summary>
        public ObservableCollection<IPilotObject> Children
        {
            get => children;
            set
            {
                if(children != value)
                {
                    children = value;
                    OnPropertyChanged();
                }
            }
        }


        protected PType type;
        /// <summary>
        /// Тип объекта
        /// </summary>
        public PType Type
        {
            get => type;
        }


        protected string visibleName = string.Empty;
        /// <summary>
        /// Отображаемое имя
        /// </summary>
        public string VisibleName
        {
            get => visibleName;
            protected set
            {
                if (visibleName != value)
                {
                    visibleName = value;
                    OnPropertyChanged();
                }
            }
        }


        protected ObservableCollection<PilotFile> files = new ObservableCollection<PilotFile>();
        /// <summary>
        /// Список файлов документа
        /// </summary>
        public ObservableCollection<PilotFile> Files
        {
            get => files;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }


        /// <summary>
        /// Обновление данных
        /// </summary>
        public virtual void UpdateObjectData()
        {
            if (guid != null)
                dObject = Global.DALContext.Repository.GetObjects(new[] { guid }).FirstOrDefault();
        }
    }
}