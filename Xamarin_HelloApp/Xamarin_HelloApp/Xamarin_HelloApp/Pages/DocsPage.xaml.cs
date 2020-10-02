using Ascon.Pilot.DataClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin_HelloApp.AppContext;
using Xamarin_HelloApp.ViewModels;

namespace Xamarin_HelloApp.Pages
{
    /// <summary>
    /// Страница списка документов
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DocsPage : ContentPage
    {
        List<DFile> Files;

        /// <summary>
        /// Страница списка документов
        /// </summary>
        /// <param name="files">список файлов</param>
        public DocsPage(DObject dObject)
        {
            InitializeComponent();

            //Files = files;

            //DChild file = files.First();

            //DObject File = Global.DALContext.Repository.GetObjects(new Guid[] { file.ObjectId }).FirstOrDefault();

            //var snapshot = File.ActualFileSnapshot;
            //Files = snapshot.Files;

            Files = new List<DFile>();

            foreach(DChild dChild in dObject.Children)
            {
                 DObject child = Global.DALContext.Repository.GetObjects(new Guid[] { dChild.ObjectId }).FirstOrDefault();

                if(child != null)
                {
                    DFile file = child.ActualFileSnapshot.Files.FirstOrDefault();
                    if (file != null)
                        Files.Add(file);
                }
            }

            listView.ItemsSource = Files;
        }


        /// <summary>
        /// Нажат файл
        /// </summary>
        private void File_Tapped(object sender, ItemTappedEventArgs e)
        {
            DFile dFile = e.Item as DFile;

            
        }
    }
}