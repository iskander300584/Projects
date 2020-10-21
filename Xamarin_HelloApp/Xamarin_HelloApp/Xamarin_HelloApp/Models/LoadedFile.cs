using System;


namespace PilotMobile.Models
{
    /// <summary>
    /// Загруженный файл
    /// </summary>
    class LoadedFile
    {
        private Guid guid;
        /// <summary>
        /// GUID объекта
        /// </summary>
        public Guid Guid
        {
            get => Guid;
        }


        private string md5;
        /// <summary>
        /// Контрольная сумма
        /// </summary>
        public string MD5
        {
            get => md5;
        }


        private string extension;
        /// <summary>
        /// Расширение файла
        /// </summary>
        public string Extension
        {
            get => extension;
        }


        private string fileName;
        /// <summary>
        /// Имя файла
        /// </summary>
        public string FileName
        {
            get => fileName;
        }


        /// <summary>
        /// Загруженный файл
        /// </summary>
        /// <param name="guid">GUID</param>
        /// <param name="md5">контрольная сумма</param>
        /// <param name="extension">расширение</param>
        public LoadedFile(Guid guid, string md5, string extension)
        {
            this.guid = guid;
            this.md5 = md5;

            if (extension.Trim().ToLower() == ".xps")
                this.extension = ".pdf";
            else
                this.extension = extension.Trim().ToLower();

            GetFileName();
        }


        /// <summary>
        /// Получение имени файла
        /// </summary>
        private void GetFileName()
        {
            fileName = guid.ToString().ToLower().Trim() + "_" + md5.ToLower().Trim() + extension;
        }


        /// <summary>
        /// Получение имени файла
        /// </summary>
        /// <returns>возвращает имя файла</returns>
        public override string ToString()
        {
            return FileName;
        }
    }
}
