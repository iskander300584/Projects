using Ascon.Pilot.DataClasses;
using Syncfusion.SfPdfViewer.XForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        /// Окно отображения документа
        /// </summary>
        /// <param name="file">опубликованный файл</param>
        public XpsPage(DFile file)
        {
            InitializeComponent();

            //if (file != null)
            //{
                
            //    byte[] array = Global.DALContext.Repository.GetFileChunk(file.Body.Id, 0, (int)file.Body.Size);

            //    if(array != null)
            //    {
            //        MemoryStream ms = new MemoryStream();
            //        ms.Write(array, 0, array.Length);

                    
            //    }
            //}
        }
    }
}