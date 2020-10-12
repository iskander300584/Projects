using Ascon.Pilot.DataClasses;
using System.IO;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin_HelloApp.AppContext;
using Xamarin_HelloApp.ViewModels;

namespace Xamarin_HelloApp.Pages
{
    /// <summary>
    /// Окно отображения документа
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class XpsPage : ContentPage
    {
        /// <summary>
        /// Окно отображения документа
        /// </summary>
        /// <param name="pilotTreeItem">объект Pilot</param>
        public XpsPage(PilotTreeItem pilotTreeItem)
        {
            InitializeComponent();

            // Проверка прав доступа
            GetPermissions();

            DFile file = null;
            var snapshot = pilotTreeItem.DObject.ActualFileSnapshot;

            if (snapshot != null)
            {
                file = snapshot.Files.FirstOrDefault();
            }

            if (file != null)
            {

                byte[] array = Global.DALContext.Repository.GetFileChunk(file.Body.Id, 0, (int)file.Body.Size);

                string fileName = Path.Combine(@"/storage/emulated/0/Download", file.Name);

                File.WriteAllBytes(fileName, array);

                if (File.Exists(fileName))
                {
                    //using (PdfSharp.Xps.XpsModel.XpsDocument xpsDoc = PdfSharp.Xps.XpsModel.XpsDocument.Open(fileName))
                    //{
                    //    FileInfo fileInfo = new FileInfo(fileName);
                    //    string newFileName = fileName.Substring(0, fileName.Length - 3);
                    //    newFileName = Path.Combine(newFileName, ".pdf");

                    //    PdfSharp.Xps.XpsConverter.Convert(xpsDoc, newFileName, 0);

                    //    if(File.Exists(newFileName))
                    //    {
                    //        MemoryStream ms = new MemoryStream(File.ReadAllBytes(newFileName));

                    //        pdfViewer.LoadDocument(ms);
                    //    }
                    //}
                }
            }
        }

        private async void GetPermissions()
        {
            await (Global.GetFilesPermissions());
        }
    }
}