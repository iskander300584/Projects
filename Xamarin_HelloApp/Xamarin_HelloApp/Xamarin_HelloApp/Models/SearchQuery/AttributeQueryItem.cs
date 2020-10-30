using System.Windows.Input;
using Xamarin_HelloApp.Models;


namespace PilotMobile.Models.SearchQuery
{
    class AttributeQueryItem : ISearchQueryItem
    {
        private PAttribute attribute;
        /// <summary>
        /// Атрибут Pilot
        /// </summary>
        public PAttribute Attribute
        {
            get => attribute;
        }


        public AttributeQueryItem(PAttribute attribute, ICommand delComand)
        {
            this.attribute = attribute;
            this.delete = delComand;
            Name = attribute.VisibleName;
            systemName = attribute.Name;
        }


        /// <summary>
        /// Получение строки поискового запроса TODO
        /// </summary>
        protected override void GetStringValue()
        {
            string prefix = @"&#32";
            switch(attribute.AttributeType)
            {
                case Ascon.Pilot.DataClasses.MAttrType.Integer:
                    prefix = @"&#32";
                    break;

                case Ascon.Pilot.DataClasses.MAttrType.String:
                    prefix = @"s";
                    break;
            }

            stringValue = prefix + "\\." + attribute.Name + ":(" + prefix + ";" + Value + ")";
        }
    }
}