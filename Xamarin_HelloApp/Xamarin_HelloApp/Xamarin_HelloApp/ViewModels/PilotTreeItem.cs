using Ascon.Pilot.DataClasses;
using FFImageLoading.Svg.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;

using Xamarin.Forms;

using Xamarin_HelloApp.AppContext;
using Xamarin_HelloApp.Models;

namespace Xamarin_HelloApp.ViewModels
{
    /// <summary>
    /// Элемент дерева Pilot
    /// </summary>
    class PilotTreeItem : INotifyPropertyChanged
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


        private SvgImageSource imageSource = null;
        /// <summary>
        /// Источник изображения
        /// </summary>
        public SvgImageSource ImageSource
        {
            get => imageSource;
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

        public PilotTreeItem(DChild element, PilotTreeItem parent)
        {
            guid = element.ObjectId;
            this.parent = parent;

            imageSource = TypeImages.GetImageSource(element.TypeId);

            

            dObject = Global.DALContext.Repository.GetObjects(new[] { element.ObjectId }).FirstOrDefault();          

            if(dObject != null)
            {
                //DPerson _currentPerson = context.Repository.CurrentPerson();

                foreach (AccessRecord accessRecord in dObject.Access)
                {
                    if(Global.CurrentPerson.AllOrgUnits.Contains(accessRecord.OrgUnitId))
                    {
                        Access _access = accessRecord.Access;
                        hasAccess = (_access.AccessLevel != AccessLevel.None);

                        if (hasAccess)
                            break;
                    }
                }

                foreach(var attr in dObject.Attributes)
                {
                    visibleName += attr.Value.StrValue + " ";
                }

                visibleName.Trim();
            }
        }
    }
}
