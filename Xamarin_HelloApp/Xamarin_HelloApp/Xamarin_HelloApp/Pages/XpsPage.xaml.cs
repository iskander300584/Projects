using PilotMobile.ViewContexts;
using PilotMobile.ViewModels;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace Xamarin_HelloApp.Pages
{
    /// <summary>
    /// Окно отображения документа
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class XpsPage : ContentPage
    {
        /// <summary>
        /// Контекст данных страницы
        /// </summary>
        private XpsPage_Context context;


        /// <summary>
        /// Окно отображения документа
        /// </summary>
        /// <param name="pilotItem">объект Pilot</param>
        public XpsPage(IPilotObject pilotItem)
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzM5MjY2QDMxMzgyZTMzMmUzMEIwKzBENG9sRkl0WVlYcXg3QjdYbjlkU1B1cXUxNVpQb0FEVkJYTVIweEE9");

            InitializeComponent();

            context = new XpsPage_Context(pilotItem, this);

            this.BindingContext = context;
        }


        /// <summary>
        /// Загрузка документа в просмотрщик
        /// </summary>
        public void LoadDocument()
        {
            if (context == null || context.DocLoaded)
                return;

            if(context != null && context.PdfFileName != "Failed")
            {
                if (context.PdfFileName != "" && File.Exists(context.PdfFileName))
                {
                    try
                    {
                        using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(context.PdfFileName)))
                        {
                            pdfViewer.LoadDocument(ms);
                        }
                    }
                    catch 
                    {
                        context.PdfFileName = "Failed";
                    }

                    context.DocLoaded = true;
                }
            }
            else
            {
                context.DocLoaded = true;
            }
        }


        /// <summary>
        /// Выгрузить документ
        /// </summary>
        /// <param name="unloadViever">выгрузить просмотрщик документов</param>
        public void UnLoadDocument(bool unloadViever = false)
        {
            try
            {
                pdfViewer.Unload();

                //if (unloadViever)
                //    pdfViewer.TryDispose();
            }
            catch { }
        }


        /// <summary>
        /// Переход на основную страницу приложения
        /// </summary>
        public void NavigateToMainPage()
        {
            Navigation.PopModalAsync();
        }


        /// <summary>
        /// Открытие страницы документа
        /// </summary>
        private void OnAppearing(object sender, System.EventArgs e)
        {
            if (context != null)
            {
                if (!context.DocLoaded)
                    context.GetXPS();
            }
        }
    }
}