using Ascon.Pilot.DataClasses;

namespace Xamarin_HelloApp.Models
{
    /// <summary>
    /// Атрибут Pilot
    /// </summary>
    class PAttribute
    {
        private MAttribute mAttribute;
        /// <summary>
        /// Тип атрибута Pilot
        /// </summary>
        public MAttribute MAttribute
        {
            get => mAttribute;
        }


        private PType type;
        /// <summary>
        /// Тип Pilot
        /// </summary>
        public PType Type
        {
            get => type;
        }


        private string name = string.Empty;
        /// <summary>
        /// Системное имя атрибута
        /// </summary>
        public string Name
        {
            get => name;
        }


        private string visibleName = string.Empty;
        /// <summary>
        /// Отображаемое имя атрибута
        /// </summary>
        public string VisibleName
        {
            get => visibleName;
        }


        private bool isVisible = false;
        /// <summary>
        /// Признак отображаемого атрибута
        /// </summary>
        public bool IsVisible
        {
            get => isVisible;
        }


        private bool isSystem = false;
        /// <summary>
        /// Признак служебного атрибута
        /// </summary>
        public bool IsSystem
        {
            get => isSystem;
        }


        private MAttrType attributeType;
        /// <summary>
        /// Тип атрибута в Pilot
        /// </summary>
        public MAttrType AttributeType
        {
            get => attributeType;
        }


        /// <summary>
        /// Атрибут Pilot
        /// </summary>
        /// <param name="mAttribute">атрибут Pilot</param>
        /// <param name="type">тип объекта</param>
        public PAttribute(MAttribute mAttribute, PType type)
        {
            this.mAttribute = mAttribute;

            this.type = type;

            name = mAttribute.Name;

            visibleName = mAttribute.Title;

            isVisible = mAttribute.ShowInTree;

            attributeType = mAttribute.Type;

            isSystem = mAttribute.IsService;
        }
    }
}