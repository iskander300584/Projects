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


        public AttributeQueryItem(PAttribute attribute)
        {
            this.attribute = attribute;
            Name = attribute.VisibleName;
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