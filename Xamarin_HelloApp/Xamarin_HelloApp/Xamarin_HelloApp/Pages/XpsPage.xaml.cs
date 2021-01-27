using PilotMobile.AppContext;
using PilotMobile.Pages;
using PilotMobile.ViewContexts;
using PilotMobile.ViewModels;
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin_HelloApp.AppContext;

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
            /*Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzM5MjY2QDMxMzgyZTMzMmUzMEIwKzBENG9sRkl0WVlYcXg3QjdYbjlkU1B1cXUxNVpQb0FEVkJYTVIweEE9");*/

            InitializeComponent();

            context = new XpsPage_Context(pilotItem, this);

            this.BindingContext = context;
        }


        /// <summary>
        /// Загрузка документа в просмотрщик
        /// </summary>
        public async void LoadDocument()
        {
            try
            {
                if (context == null || context.DocLoaded)
                    return;

                if (context != null && context.PdfFileName != "Failed")
                {
                    if (context.PdfFileName != "" && File.Exists(context.PdfFileName))
                    {
                        try
                        {
                            pdfView.Uri = context.PdfFileName;
                            /*using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(context.PdfFileName)))
                            {
                                pdfViewer.LoadDocument(ms);
                            }*/
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
            catch (Exception ex)
            {
                var res = await DisplayError(ex.Message);

                if (res)
                    await Global.SendErrorReport(ex);
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
                //pdfViewer.Unload();

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
                if (!context.DocLoaded && !context.Loading )
                    context.GetXPS();
            }
        }


        /// <summary>
        /// Копирование ссылки на объект
        /// </summary>
        private async void CopyLink(object sender, EventArgs e)
        {
            bool result = await Global.CopyLink(context.PilotItem.DObject);
        }


        /// <summary>
        /// Поделиться ссылкой на объект
        /// </summary>
        private async void ShareLink(object sender, EventArgs e)
        {
            bool result = await Global.ShareLink(context.PilotItem.DObject);
        }


        /// <summary>
        /// Вывести сообщение об ошибке
        /// </summary>
        /// <param name="message">текст сообщения об ошибке</param>
        /// <param name="caption">заголовок ошибки</param>
        /// <returns>возвращает TRUE, если необходимо отправить отчет об ошибке</returns>
        public async Task<bool> DisplayError(string message, string caption = "Ошибка")
        {
            return await DisplayAlert(caption, message + StringConstants.SendErrorMessage, StringConstants.Send, StringConstants.DontSend);
        }


        /// <summary>
        /// Нажатие на кнопку Карточка
        /// </summary>
        private void Card_Click(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new DocumentCarrousel(context.PilotItem));
        }
    }
}