using Ascon.Pilot.DataClasses;
using PilotMobile.ViewModels;
using System.Linq;
using Xamarin_HelloApp.AppContext;

namespace Xamarin_HelloApp.ViewModels
{
    /// <summary>
    /// Элемент дерева Pilot
    /// </summary>
    public class PilotTreeItem : IPilotObject
    {
        private bool hasAccess = false;
        /// <summary>
        /// У пользователя есть доступ к объекту
        /// </summary>
        public bool HasAccess
        {
            get => hasAccess;
        }


        #region Конструкторы


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

            GetObjectData();
        }


        /// <summary>
        /// Корневой элемент дерева Pilot
        /// </summary>
        /// <param name="rootObject">корневой элемент дерева Pilot</param>
        /// <param name="getAll">получить все данные</param>
        public PilotTreeItem(DObject rootObject, bool getAll = false)
        {
            guid = rootObject.Id;
            dObject = rootObject;
            parent = null;

            if (!getAll)
            {
                type = null;
                hasAccess = true;
            }
            else
            {
                type = TypeFabrique.GetType(dObject.TypeId);
                GetObjectData();
            }
        }


        /// <summary>
        /// Пустой корневой элемент дерева Pilot 
        /// </summary>
        public PilotTreeItem()
        {
            guid = new System.Guid();

            dObject = null;

            parent = null;

            type = null;

            hasAccess = true;
        }


        #endregion


        /// <summary>
        /// Обновление данных объекта
        /// </summary>
        public override void UpdateObjectData()
        {
            base.UpdateObjectData();

            GetObjectData();
        }


        /// <summary>
        /// Получение данных объекта
        /// </summary>
        private void GetObjectData()
        {
            if (dObject != null)
            {
                foreach (AccessRecord accessRecord in dObject.Access)
                {
                    if (Global.CurrentPerson != null && Global.CurrentPerson.AllOrgUnits.Contains(accessRecord.OrgUnitId))
                    {
                        Access _access = accessRecord.Access;
                        hasAccess = (_access.AccessLevel != AccessLevel.None);

                        if (hasAccess)
                            break;
                    }
                }

                foreach (var attr in dObject.Attributes)
                {
                    if (Type != null && Type.Attributes.Any(a => a.Name == attr.Key && a.IsVisible && !a.IsSystem))
                        visibleName += attr.Value.StrValue + " ";
                }

                visibleName.Trim();
            }
        }
    }
}