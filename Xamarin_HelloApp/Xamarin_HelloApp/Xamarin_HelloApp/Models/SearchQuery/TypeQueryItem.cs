using System.Windows.Input;
using Xamarin_HelloApp.Models;


namespace PilotMobile.Models.SearchQuery
{
    /// <summary>
    /// Элемент поискового запроса "Тип"
    /// </summary>
    class TypeQueryItem : ISearchQueryItem
    {
        private PType type;
        /// <summary>
        /// Тип Pilot
        /// </summary>
        public PType Type
        {
            get => type;
        }


        /// <summary>
        /// Элемент поискового запроса "Тип"
        /// </summary>
        /// <param name="type">тип Pilot</param>
        public TypeQueryItem(PType type, ICommand delCommand)
        {
            this.type = type;
            this.delete = delCommand;
            systemName = type.ID.ToString();
            isType = true;
            Name = "Тип";
            Value = Type.VisibleName;
        }


        /// <summary>
        /// Получение строки поискового запроса
        /// </summary>
        protected override void GetStringValue()
        {
            stringValue = @$"&#32;{type.ID}";
        }
    }
}