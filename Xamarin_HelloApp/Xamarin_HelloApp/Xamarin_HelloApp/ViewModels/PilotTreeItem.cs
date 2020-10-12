using Ascon.Pilot.DataClasses;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Xamarin_HelloApp.AppContext;
using Xamarin_HelloApp.Models;

namespace Xamarin_HelloApp.ViewModels
{
    /// <summary>
    /// Элемент дерева Pilot
    /// </summary>
    public class PilotTreeItem : INotifyPropertyChanged
    {
        private Guid guid = new Guid();
        /// <summary>
        /// GUID
        /// </summary>
        public Guid Guid
        {
            get => guid;
        }


        private DObject dObject = null;
        /// <summary>
        /// Объект Pilot
        /// </summary>
        public DObject DObject
        {
            get => dObject;
        }


        private PilotTreeItem parent;
        /// <summary>
        /// Головной объект
        /// </summary>
        public PilotTreeItem Parent
        {
            get => parent;
        }


        private ObservableCollection<PilotTreeItem> children = new ObservableCollection<PilotTreeItem>();
        /// <summary>
        /// Коллекция потомков
        /// </summary>
        public ObservableCollection<PilotTreeItem> Children
        {
            get => children;
        }


        private PType type;
        /// <summary>
        /// Тип объекта
        /// </summary>
        public PType Type
        {
            get => type;
        }


        private string visibleName = string.Empty;
        /// <summary>
        /// Отображаемое имя
        /// </summary>
        public string VisibleName
        {
            get => visibleName;
            private set
            {
                if(visibleName != value)
                {
                    visibleName = value;
                    OnPropertyChanged();
                }
            }
        }


        private bool hasAccess = false;
        /// <summary>
        /// У пользователя есть доступ к объекту
        /// </summary>
        public bool HasAccess
        {
            get => hasAccess;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }


        /// <summary>
        /// Элемент дерева Pilot
        /// </summary>
        /// <param name="element">дочерний элемент Pilot</param>
        /// <param name="parent">головной элемент</param>
        public PilotTreeItem(DChild element, PilotTreeItem parent)
        {
            guid = element.ObjectId;
            this.parent = parent;

            type = TypeFabrique.GetType(element.TypeId);  

            dObject = Global.DALContext.Repository.GetObjects(new[] { element.ObjectId }).FirstOrDefault();          

            if(dObject != null)
            {
                foreach (AccessRecord accessRecord in dObject.Access)
                {
                    if(Global.CurrentPerson != null && Global.CurrentPerson.AllOrgUnits.Contains(accessRecord.OrgUnitId))
                    {
                        Access _access = accessRecord.Access;
                        hasAccess = (_access.AccessLevel != AccessLevel.None);

                        if (hasAccess)
                            break;
                    }
                }

                foreach(var attr in dObject.Attributes)
                {
                    if (Type != null && Type.Attributes.Any(a => a.Name == attr.Key && a.IsVisible && !a.IsSystem))
                        visibleName += attr.Value.StrValue + " ";
                }

                visibleName.Trim();
            }           
        }


        /// <summary>
        /// Корневой элемент дерева Pilot
        /// </summary>
        /// <param name="rootObject">корневой элемент дерева Pilot</param>
        public PilotTreeItem(DObject rootObject)
        {
            guid = rootObject.Id;

            dObject = rootObject;

            parent = null;

            type = null;

            hasAccess = true;
        }
    }
}