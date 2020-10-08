using Ascon.Pilot.DataClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
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
        private async void File_Tapped(object sender, ItemTappedEventArgs e)
        {
            // Проверка прав доступа
            if (await (Global.GetFilesPermissions()) != true)
                return;

            // Получение файла из БД Pilot
            DFile dFile = e.Item as DFile;

            byte[] array = Global.DALContext.Repository.GetFileChunk(dFile.Body.Id, 0, (int)dFile.Body.Size);

            string fileName = Path.Combine(@"/storage/emulated/0/Download", dFile.Name);

            File.WriteAllBytes(fileName, array);

            // Открытие файла средствами ОС
            if (File.Exists(fileName))
            {
                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(fileName)
                });
            }
        }
    }
}