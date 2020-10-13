using Ascon.Pilot.DataClasses;
using PilotMobile.AppContext;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace PilotMobile.ViewModels
{
    /// <summary>
    /// Файл Pilot
    /// </summary>
    public class PilotFile : INotifyPropertyChanged
    {
        private DFile dFile;
        /// <summary>
        /// Файл Pilot
        /// </summary>
        public DFile DFile
        {
            get => dFile;
        }


        private string fileName;
        /// <summary>
        /// Имя файла
        /// </summary>
        public string FileName
        {
            get => fileName;
        }


        private string imageSource;
        /// <summary>
        /// Источник изображения для пиктограммы
        /// </summary>
        public string ImageSource
        {
            get => imageSource;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }


        /// <summary>
        /// Файл Pilot
        /// </summary>
        /// <param name="dFile">файл Pilot</param>
        public PilotFile(DFile dFile)
        {
            this.dFile = dFile;
            fileName = dFile.Name;

            string extension = GetExtension(fileName);
            imageSource = FileImageFabrique.GetImageSource(extension);
        }


        /// <summary>
        /// Получение расширения файла
        /// </summary>
        /// <param name="fileName">имя файла</param>
        /// <returns>возвращает расширение файла</returns>
        private string GetExtension(string fileName)
        {
            int index = fileName.LastIndexOf('.');

            if (index == -1 || index == fileName.Length - 1)
                return string.Empty;

            return fileName.Substring(index + 1);
        }
    }
}