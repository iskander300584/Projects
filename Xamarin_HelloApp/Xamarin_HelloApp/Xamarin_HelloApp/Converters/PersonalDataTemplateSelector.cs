using Xamarin.Forms;

namespace PilotMobile.Converters
{
    /// <summary>
    /// Выбор шаблона данных
    /// </summary>
    public class PersonDataTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Шаблон данных для объекта Label
        /// </summary>
        public DataTemplate LabelTemplate { get; set; }


        /// <summary>
        /// Шаблон данных для объекта Entry
        /// </summary>
        public DataTemplate EntryTemplate { get; set; }


        /// <summary>
        /// Шаблон данных для объекта DatePicker
        /// </summary>
        public DataTemplate DateTemplate { get; set; }


        /// <summary>
        /// Шаблон данных для объекта Editor
        /// </summary>
        public DataTemplate EditorTemplate { get; set; }


        /// <summary>
        /// Выбор шаблона данных
        /// </summary>
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            View view = (View)item;

            if (view is Label)
                return LabelTemplate;
            else if (view is Entry)
                return EntryTemplate;
            else if (view is DatePicker)
                return DateTemplate;
            else if (view is Editor)
                return EditorTemplate;

            return null;
        }
    }
}