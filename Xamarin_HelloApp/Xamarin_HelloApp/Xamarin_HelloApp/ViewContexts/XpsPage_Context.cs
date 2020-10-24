using Ascon.Pilot.DataClasses;
using PilotMobile.AppContext;
using PilotMobile.Models;
using PilotMobile.ViewModels;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin_HelloApp.AppContext;
using Xamarin_HelloApp.Pages;
using Xamarin_HelloApp.ViewModels;


namespace PilotMobile.ViewContexts
{
    /// <summary>
    /// Контекст данных окна документа
    /// </summary>
    class XpsPage_Context : INotifyPropertyChanged
    {
        private string pdfFileName = string.Empty;
        /// <summary>
        /// Путь к файлу PDF
        /// </summary>
        public string PdfFileName
        {
            get => pdfFileName;
            set
            {
                if(pdfFileName != value)
                {
                    pdfFileName = value;
                    OnPropertyChanged();

                    CalcMessage();

                    page.LoadDocument();
                }
            }
        }


        /// <summary>
        /// Страница документа
        /// </summary>
        private XpsPage page;


        private IPilotObject pilotItem;
        /// <summary>
        /// Элемент Pilot
        /// </summary>
        public IPilotObject PilotItem
        {
            get => pilotItem;
        }


        private ICommand upCommand;
        /// <summary>
        /// Команда Вверх
        /// </summary>
        public ICommand UpCommand
        {
            get => upCommand;
        }


        private ICommand updateCommand;
        /// <summary>
        /// Команда обновления данных
        /// </summary>
        public ICommand UpdateCommand
        {
            get => updateCommand;
        }


        private string message;
        /// <summary>
        /// Текст сообщения
        /// </summary>
        public string Message
        {
            get => message;
            private set
            {
                if(message != value)
                {
                    message = value;
                    OnPropertyChanged();
                }
            }
        }


        private bool isMessageVisible = true;
        /// <summary>
        /// Видимость сообщения
        /// </summary>
        public bool IsMessageVisible
        {
            get => isMessageVisible;
            private set
            {
                if(isMessageVisible != value)
                {
                    isMessageVisible = value;
                    OnPropertyChanged();
                }
            }
        }


        private bool docLoaded = false;
        /// <summary>
        /// Признак выполненной загрузки документа
        /// </summary>
        public bool DocLoaded
        {
            get => docLoaded;
            set
            {
                if(docLoaded != value)
                {
                    docLoaded = true;
                    OnPropertyChanged();
                }
            }
        }


        /// <summary>
        /// Контекст данных окна документа
        /// </summary>
        /// <param name="pilotItem">элемент Pilot</param>
        /// <param name="page">страница окна документа</param>
        public XpsPage_Context(IPilotObject pilotItem, XpsPage page)
        {
            this.pilotItem = pilotItem;
            this.page = page;
            upCommand = new Command(Up_Execute);
            updateCommand = new Command(Update_Execute);

            CalcMessage();
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }


        /// <summary>
        /// Получение данных XPS
        /// </summary>
        /// <param name="update">обновить данные</param>
        public void GetXPS(bool update = false)
        {
            GetPermissions();

            GetXpsData(update);
        }


        /// <summary>
        /// Получение данных XPS документа в отдельном потоке
        /// </summary>
        /// <param name="update">обновить данные</param>
        private void GetXpsData(bool update)
        {
            string _xpsName = string.Empty;

            try
            {
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
                else if (pilotItem is PilotTask)
                {
                    DRelation relation = pilotItem.DObject.Relations.FirstOrDefault();
                    if (relation.TargetId != null)
                    {
                        DObject child = Global.DALContext.Repository.GetObjects(new Guid[] { relation.TargetId }).FirstOrDefault();

                        if (child != null)
                            file = child.ActualFileSnapshot.Files.FirstOrDefault();
                    }
                }

                if (file != null)
                {
                    string md5 = file.Body.Md5.Part1.ToString() + file.Body.Md5.Part2.ToString();
                    string extension = file.Name.Substring(file.Name.LastIndexOf('.'));

                    LoadedFile loadedFile = new LoadedFile(file.Body.Id, md5, extension);

                    string dir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/";
                    string _pdfName = Path.Combine(dir + loadedFile.FileName);

                    // Загрузка и формирование PDF, если не был загружен ранее
                    if (!File.Exists(_pdfName) || update)
                    {
                        try
                        {
                            if (File.Exists(_pdfName))
                                File.Delete(_pdfName);
                        }
                        catch { }

                        Regex regex = new Regex(@"xps$");

                        // Конвертация XPS
                        if (regex.IsMatch(file.Name.ToLower()))
                        {
                            // Загрузка XPS
                            byte[] array = Global.DALContext.Repository.GetFileChunk(file.Body.Id, 0, (int)file.Body.Size);
                            _xpsName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), new Guid().ToString() + StringConstants.XPS);

                            File.WriteAllBytes(_xpsName, array);

                            if (File.Exists(_xpsName))
                            {
                                try
                                {
                                    //Spire.License.LicenseProvider.SetLicenseKey("5TV2WACA75Q3");
                                    //Spire.License.LicenseProvider.LoadLicense();

                                    Spire.Pdf.PdfDocument doc = new Spire.Pdf.PdfDocument();
                                    doc.LoadFromFile(_xpsName, Spire.Pdf.FileFormat.XPS);
                                    doc.SaveToFile(_pdfName, Spire.Pdf.FileFormat.PDF);
                                    doc.Dispose();

                                    try
                                    {
                                        File.Delete(_xpsName);
                                    }
                                    catch { }
                                }
                                catch (Exception ex)
                                {
                                    string msg = ex.Message;
                                    if (msg != "") { }
                                    try
                                    {
                                        if (File.Exists(_pdfName))
                                            File.Delete(_pdfName);
                                        if (File.Exists(_xpsName))
                                            File.Delete(_xpsName);
                                    }
                                    catch { }
                                }
                            }
                        }
                        else
                        {
                            regex = new Regex(@"pdf$");

                            if(regex.IsMatch(file.Name.ToLower()))
                            {
                                try
                                {
                                    byte[] array = Global.DALContext.Repository.GetFileChunk(file.Body.Id, 0, (int)file.Body.Size);
                                    File.WriteAllBytes(_pdfName, array);
                                }
                                catch { }
                            }
                        }
                    }

                    if (File.Exists(_pdfName))
                    {
                        PdfFileName = _pdfName;
                    }
                    else
                        PdfFileName = "Failed";
                }
            }
            catch 
            {
                if(PdfFileName != "" && File.Exists(PdfFileName))
                {
                    try
                    {
                        File.Delete(PdfFileName);
                    }
                    catch { }
                }

                try
                {
                    if (File.Exists(_xpsName))
                        File.Delete(_xpsName);
                }
                catch { }
            }

            if (PdfFileName == "")
                PdfFileName = "Failed";
        }


        /// <summary>
        /// Получение прав на работу с файлами
        /// </summary>
        private async void GetPermissions()
        {
            await (Global.GetFilesPermissions());
        }


        /// <summary>
        /// Команда Вверх
        /// </summary>
        private void Up_Execute()
        {
            page.UnLoadDocument(true);
            page.NavigateToMainPage();
        }


        /// <summary>
        /// Команда Обновить данные
        /// </summary>
        private void Update_Execute()
        {
            DocLoaded = false;
            PdfFileName = string.Empty;
            //page.UnLoadDocument();

            GetXPS(true);
        }


        /// <summary>
        /// Определение видимости и текста сообщения
        /// </summary>
        private void CalcMessage()
        {
            if(PdfFileName != "")
            {
                if(PdfFileName == "Failed")
                {
                    Message = "Ошибка загрузки документа";
                }
                else
                {
                    IsMessageVisible = false;
                }
            }
            else
            {
                IsMessageVisible = true;
                Message = "Загрузка данных...";
            }
        }
    }
}