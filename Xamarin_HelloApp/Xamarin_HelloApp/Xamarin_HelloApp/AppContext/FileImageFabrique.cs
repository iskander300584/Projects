
namespace PilotMobile.AppContext
{
    /// <summary>
    /// "Фабрика" пиктограмм файлов
    /// </summary>
    public static class FileImageFabrique
    {
        /// <summary>
        /// Получить путь к источнику пиктограммы
        /// </summary>
        /// <param name="extension">расширение файла</param>
        /// <returns>возвращает Source для Image</returns>
        public static string GetImageSource(string extension)
        {
            string source = @"file.png"; // пиктограмма по умолчанию

            switch(extension.ToLower())
            {
                case "doc":
                case "docx":
                    source = @"doc.png";
                    break;

                case "xls":
                case "xlsx":
                    source = @"xls.png";
                    break;

                case "ppt":
                case "pptx":
                    source = @"ppt.png";
                    break;

                case "zip":
                case "7z":
                    source = @"zip.png";
                    break;

                case "cdw":
                case "spw":
                case "a3d":
                case "m3d":
                    source = @"kompas.png";
                    break;

                case "jpg":
                case "jpeg":
                    source = @"jpg.png";
                    break;

                case "pdf":
                    source = @"pdf.png";
                    break;

                case "txt":
                    source = @"txt.png";
                    break;

                case "dwg":
                case "dwt":
                case "dxf":
                case "rvt":
                    source = @"acd.png";
                    break;

                case "model":
                case "catpart":
                case "catdrawing":
                case "catproduct":
                    source = @"cad.png";
                    break;

                case "odp":
                    source = @"odp.png";
                    break;

                case "ods":
                    source = @"ods.png";
                    break;

                case "odt":
                    source = @"odt.png";
                    break;

                case "rar":
                    source = @"rar.png";
                    break;

                case "sxc":
                    source = @"sxc.png";
                    break;

                case "sxi":
                    source = @"sxi.png";
                    break;

                case "sxw":
                    source = @"sxw.png";
                    break;

                case "bimx":
                case "pln":
                case "pla":
                    source = @"archicad.png";
                    break;

                case "sldprt":
                case "sldasm":
                    source = @"sw.png";
                    break;

                case "prt":
                case "unv":
                case "mesh":
                    source = @"nx.png";
                    break;

                case "rnp":
                    source = @"renga.png";
                    break;
            }

            return source;
        }
    }
}