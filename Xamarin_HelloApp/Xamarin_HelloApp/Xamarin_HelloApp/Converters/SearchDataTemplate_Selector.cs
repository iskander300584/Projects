using PilotMobile.Models.SearchQuery;
using Xamarin.Forms;


namespace PilotMobile.Converters
{
    /// <summary>
    /// Выбор шаблона данных для страницы поиска
    /// </summary>
    class SearchDataTemplate_Selector : DataTemplateSelector
    {
        /// <summary>
        /// Шаблон данных для поиска по типу
        /// </summary>
        public DataTemplate TypeTemplate { get; set; }


        /// <summary>
        /// Шаблон данных для поиска по атрибуту
        /// </summary>
        public DataTemplate AttributeTemplate { get; set; }


        /// <summary>
        /// Выбор шаблона данных для страницы поиска
        /// </summary>
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            ISearchQueryItem searchItem = (ISearchQueryItem)item;

            return (searchItem.IsType) ? TypeTemplate : AttributeTemplate;
        }
    }
}