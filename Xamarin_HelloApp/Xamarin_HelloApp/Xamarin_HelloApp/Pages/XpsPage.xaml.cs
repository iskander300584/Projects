using Ascon.Pilot.DataClasses;
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
        /// Окно отображения документа
        /// </summary>
        /// <param name="dObject">объект Pilot</param>
        public XpsPage(DObject dObject)
        {
            InitializeComponent();

            //DFile file = null;
            //var snapshot = dObject.ActualFileSnapshot;

            //if(snapshot != null)
            //{
            //    file = snapshot.Files.FirstOrDefault();
            //}

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