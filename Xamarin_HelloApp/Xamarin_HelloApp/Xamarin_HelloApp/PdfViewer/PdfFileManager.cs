using System.IO;
using System.Threading.Tasks;
//using PCLStorage

namespace PilotMobile.PdfViewer
{
    /// <summary>
    /// Класс загрузки файла PDF
    /// </summary>
    public static class PdfFileManager
    {
        /// <summary>
        /// Получить поток данных файла
        /// </summary>
        /// <param name="fileName">имя файла</param>
        public static Stream GetFileStreamAsync(string fileName)
        {
            if (!File.Exists(fileName))
                return null;

            return new MemoryStream(File.ReadAllBytes(fileName));
        }
    }
}