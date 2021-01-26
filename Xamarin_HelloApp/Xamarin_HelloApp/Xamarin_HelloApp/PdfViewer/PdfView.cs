/// Классы PdfViewer взяты из проекта
/// https://github.com/rohitvipin/PdfViewerSample

using Xamarin.Forms;

namespace PilotMobile.PdfViewer
{
    /// <summary>
    /// Элемент отображения PDF
    /// </summary>
    public class PdfView : WebView
    {
        /// <summary>
        /// Свойство ссылки на файл
        /// </summary>
        public static readonly BindableProperty UriProperty = BindableProperty.Create(nameof(Uri), typeof(string), typeof(PdfView));

        /// <summary>
        /// Ссылка на файл
        /// </summary>
        public string Uri
        {
            get => (string)GetValue(UriProperty);
            set => SetValue(UriProperty, value);
        }
    }
}