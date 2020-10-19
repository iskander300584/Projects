using Ascon.Pilot.DataClasses;
using PilotMobile.ViewModels;
using System;
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
        /// <param name="pilotItem">объект Pilot</param>
        public XpsPage(IPilotObject pilotItem)
        {
            InitializeComponent();

            // Проверка прав доступа
            GetPermissions();

            DFile file = null;

            // Получение файла для объекта
            if (pilotItem is PilotTreeItem)
            {
                var snapshot = pilotItem.DObject.ActualFileSnapshot;

                if (snapshot != null)
                {
                    file = snapshot.Files.FirstOrDefault();
                }
            }
            // Получение файла для задачи
            else if(pilotItem is PilotTask)
            {
                DRelation relation = pilotItem.DObject.Relations.FirstOrDefault();
                if(relation.TargetId != null)
                {                   
                    DObject child = Global.DALContext.Repository.GetObjects(new Guid[] { relation.TargetId }).FirstOrDefault();

                    if (child != null)
                        file = child.ActualFileSnapshot.Files.FirstOrDefault();
                }
            }

            if (file != null)
            {
                byte[] array = Global.DALContext.Repository.GetFileChunk(file.Body.Id, 0, (int)file.Body.Size);

                string fileName = Path.Combine(@"/storage/emulated/0/Download", file.Name);

                File.WriteAllBytes(fileName, array);

                if (File.Exists(fileName))
                {
                    Spire.Pdf.PdfDocument doc = new Spire.Pdf.PdfDocument();
                    doc.LoadFromFile(fileName, Spire.Pdf.FileFormat.XPS);
                    doc.SaveToFile(@"/storage/emulated/0/Download/temp.pdf", Spire.Pdf.FileFormat.PDF);

                    //Aspose.Pdf.XpsLoadOptions xpsLoadOptions = new Aspose.Pdf.XpsLoadOptions();
                    //xpsLoadOptions.BatchSize = (int)file.Body.Size;
                    //Aspose.Pdf.Document document = new Aspose.Pdf.Document(fileName, xpsLoadOptions);
                    //document.Convert(@"/storage/emulated/0/Download/temp.pdf", Aspose.Pdf.PdfFormat.PDF_A_1A, Aspose.Pdf.ConvertErrorAction.None);

                    //if (File.Exists(@"/storage/emulated/0/Download/temp.pdf"))
                    //{

                    //}

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