using GreenLeaf.Classes;
using GreenLeaf.Constants;
using GreenLeaf.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Excel = Microsoft.Office.Interop.Excel;

namespace GreenLeaf.Windows.AdminPanel
{
    /// <summary>
    /// Окно журнала событий
    /// </summary>
    public partial class AdminJournalWindow : Window
    {
        #region Поля класса

        /// <summary>
        /// Отображаемые события журнала
        /// </summary>
        private List<Journal> JournalList;

        /// <summary>
        /// Список пользователей
        /// </summary>
        private List<Account> Accounts;

        #endregion

        /// <summary>
        /// Окно журнала событий
        /// </summary>
        public AdminJournalWindow()
        {
            InitializeComponent();

            dpFromDate.SelectedDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dpTillDate.SelectedDate = DateTime.Today;

            GetUsers();

            GetData();
        }

        /// <summary>
        /// Получение списка пользователей
        /// </summary>
        private void GetUsers()
        {
            Accounts = Account.GetAllAccountsPersonalData();

            cbUser.Items.Add("Все");

            foreach (Account account in Accounts.OrderBy(a => a.PersonalData.VisibleName))
                cbUser.Items.Add(account.PersonalData.VisibleName);

            cbUser.SelectedIndex = 0;
        }

        /// <summary>
        /// Получение данных
        /// </summary>
        private void GetData()
        {
            Mouse.OverrideCursor = Cursors.Wait;

            dgJournal.ItemsSource = null;

            Account account = null;

            if (cbUser.SelectedIndex != 0)
            {
                account = Accounts.FirstOrDefault(a => a.PersonalData.VisibleName == cbUser.SelectedItem.ToString());
            }

            if (account == null)
                JournalList = Journal.GetJournal((DateTime)dpFromDate.SelectedDate, (DateTime)dpTillDate.SelectedDate);
            else
                JournalList = Journal.GetJournal(account.ID, (DateTime)dpFromDate.SelectedDate, (DateTime)dpTillDate.SelectedDate);

            dgJournal.ItemsSource = JournalList;

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Кнопка Поиск
        /// </summary>
        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            GetData();
        }

        /// <summary>
        /// Экспорт в Excel
        /// </summary>
        private void Excel_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + FileNames.JournalTemplate))
            {
                Dialog.ErrorMessage(this, "Шаблон отчета не найден. Обратитесь к администратору");
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog();

            saveDialog.Filter = "Файлы Excel|*.xls";
            saveDialog.FilterIndex = 0;
            saveDialog.FileName = "";
            saveDialog.CheckPathExists = true;
            saveDialog.CheckFileExists = false;

            Mouse.OverrideCursor = null;

            if (!(bool)saveDialog.ShowDialog())
                return;

            string fileName = saveDialog.FileName;

            Mouse.OverrideCursor = Cursors.Wait;

            if (!fileName.Contains(".xls"))
                fileName += ".xls";

            Excel.Application excellApp = null;
            Excel.Workbook workbook = null;

            try
            {
                // Копирование шаблона отчета
                FileInfo template = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + FileNames.JournalTemplate);
                FileInfo report = template.CopyTo(fileName, true);

                excellApp = new Excel.Application(); // открываем Excel
                excellApp.Visible = false;
                excellApp.DisplayAlerts = false;

                workbook = excellApp.Workbooks.Open(report.FullName);
                workbook.DisplayInkComments = false;

                Excel.Worksheet worksheet = workbook.Worksheets.get_Item(1);

                // Заполнение периода
                worksheet.Cells[3, "B"] = ((DateTime)dpFromDate.SelectedDate).ToShortDateString();
                worksheet.Cells[3, "C"] = ((DateTime)dpTillDate.SelectedDate).ToShortDateString();

                // Заполнение товара
                int i = 1;
                int currRow = 7;
                foreach (Journal journal in JournalList)
                {
                    // Добавление пустой строки
                    Excel.Range cellRange = (Excel.Range)worksheet.Cells[currRow, 1];
                    Excel.Range rowRange = cellRange.EntireRow;
                    rowRange.Insert(Excel.XlInsertShiftDirection.xlShiftDown, false);

                    // "№ п/п"
                    worksheet.Cells[currRow, "A"] = i++.ToString();

                    // Пользователь
                    if (journal.Account != null)
                        worksheet.Cells[currRow, "B"] = journal.Account.PersonalData.VisibleName;

                    // Дата
                    worksheet.Cells[currRow, "C"] = journal.Date.ToShortDateString();

                    // Событие
                    worksheet.Cells[currRow, "D"] = journal.Act;

                    currRow++;
                }

                // Удаление пустых строк
                Excel.Range cRange = (Excel.Range)worksheet.Cells[currRow, 1];
                Excel.Range rRange = cRange.EntireRow;
                rRange.Delete(Excel.XlDeleteShiftDirection.xlShiftUp);
                cRange = (Excel.Range)worksheet.Cells[6, 1];
                rRange = cRange.EntireRow;
                rRange.Delete(Excel.XlDeleteShiftDirection.xlShiftUp);

                // сохранение отчета
                workbook.Save();

                // Закрытие Excel
                workbook.Close(false);
                excellApp.Quit();

                workbook = null;
                excellApp = null;

                // Запуск отчета
                Process.Start(report.FullName);
            }
            catch (Exception ex)
            {
                Dialog.ErrorMessage(this, "Ошибка выгрузки журнала событий в Excel", ex.Message);

                try
                {
                    // Закрытие Excel
                    workbook.Close(false);
                    excellApp.Quit();

                    workbook = null;
                    excellApp = null;
                }
                catch { }
            }

            Mouse.OverrideCursor = null;
        }
    }
}