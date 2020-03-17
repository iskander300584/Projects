using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using GreenLeaf.ViewModel;
using GreenLeaf.Classes;
using GreenLeaf.Windows.InvoiceView;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;
using GreenLeaf.Constants;

namespace GreenLeaf.Windows.Reports
{
    /// <summary>
    /// Окно отчета прихода / расхода
    /// </summary>
    public partial class ReportIncomeWindow : Window
    {
        #region Объявление команд

        /// <summary>
        /// Команда Открыть
        /// </summary>
        public static RoutedUICommand OpenCommand = new RoutedUICommand("Открыть", "OpenCommand", typeof(ReportIncomeWindow));

        /// <summary>
        /// Команда Выгрузить в Excel
        /// </summary>
        public static RoutedUICommand ExportExcel = new RoutedUICommand("Экспорт", "ExportExcel", typeof(ReportIncomeWindow));

        #endregion

        #region Поля класса

        /// <summary>
        /// Список отображаемых накладных
        /// </summary>
        private List<Invoice> InvoiceList = new List<Invoice>();

        /// <summary>
        /// Сумма по приходным накладным
        /// </summary>
        private double SummPurchase = 0;

        /// <summary>
        /// Купон по приходным накладным
        /// </summary>
        private double CouponPurchase = 0;

        /// <summary>
        /// Сумма по расходным накладным
        /// </summary>
        private double SummSales = 0;

        /// <summary>
        /// Купон по расходным накладным
        /// </summary>
        private double CouponSales = 0;

        /// <summary>
        /// Сумма по балансу
        /// </summary>
        private double SummBalance = 0;

        /// <summary>
        /// Купон по балансу
        /// </summary>
        private double CouponBalance = 0;

        #endregion

        /// <summary>
        /// Окно отчета прихода / расхода
        /// </summary>
        public ReportIncomeWindow()
        {
            InitializeComponent();

            dpFromDate.SelectedDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dpTillDate.SelectedDate = DateTime.Today;

            GetData();
        }

        #region Получение данных

        /// <summary>
        /// Получение данных
        /// </summary>
        private void GetData()
        {
            Mouse.OverrideCursor = Cursors.Wait;

            dgInvoices.ItemsSource = null;

            InvoiceList.Clear();

            // Получение списка приходных накладных
            List<Invoice> temp = Invoice.GetPurchaseInvoices((DateTime)dpFromDate.SelectedDate, (DateTime)dpTillDate.SelectedDate).Where(i => i.IsIssued).ToList();

            SummPurchase = 0;
            CouponPurchase = 0;

            foreach(Invoice inv in temp)
            {
                SummPurchase += inv.Cost;
                CouponPurchase += inv.Coupon;

                InvoiceList.Add(inv);
            }

            tbConsumption.Text = Conversion.ToFinance(SummPurchase);
            tbConsumptionCoupon.Text = Conversion.ToFinance(CouponPurchase);

            // Получение списка расходных накладных
            temp = Invoice.GetSalesInvoices((DateTime)dpFromDate.SelectedDate, (DateTime)dpTillDate.SelectedDate).Where(i => i.IsIssued).ToList();

            SummSales = 0;
            CouponSales = 0;

            foreach (Invoice inv in temp)
            {
                SummSales += inv.Cost;
                CouponSales += inv.Coupon;

                InvoiceList.Add(inv);
            }

            tbIncome.Text = Conversion.ToFinance(SummSales);
            tbIncomeCoupon.Text = Conversion.ToFinance(CouponSales);

            // Вычисление баланса
            SummBalance = SummSales - SummPurchase;
            CouponBalance = CouponSales - CouponPurchase;

            tbBalance.Text = Conversion.ToFinance(SummBalance);
            tbBalanceCoupon.Text = Conversion.ToFinance(CouponBalance);

            // Настройка цвета баланса
            if (SummBalance == 0)
                tbBalance.Foreground = Brushes.Black;
            else if (SummBalance > 0)
                tbBalance.Foreground = Brushes.DarkGreen;
            else
                tbBalance.Foreground = Brushes.Red;

            if (CouponBalance == 0)
                tbBalanceCoupon.Foreground = Brushes.Black;
            else if (CouponBalance > 0)
                tbBalanceCoupon.Foreground = Brushes.DarkGreen;
            else
                tbBalanceCoupon.Foreground = Brushes.Red;

            // Сортировка списка
            InvoiceList = InvoiceList.OrderBy(i => i.Date).ToList();

            dgInvoices.ItemsSource = InvoiceList;

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Кнопка Поиск
        /// </summary>
        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            GetData();
        }

        #endregion

        #region Открытие накладной

        /// <summary>
        /// Проверка возможности нажатия кнопки Открыть
        /// </summary>
        private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (dgInvoices != null && dgInvoices.SelectedItem != null) ? true : false;
        }

        /// <summary>
        /// Нажатие кнопки Открыть
        /// </summary>
        private void Open_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            Open();
        }

        /// <summary>
        /// Двойной щелчок по накладной
        /// </summary>
        private void dgInvoices_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgInvoices.SelectedItem != null)
            {
                Open();
            }
        }

        /// <summary>
        /// Открыть накладную
        /// </summary>
        private void Open()
        {
            Mouse.OverrideCursor = Cursors.Wait;

            Invoice invoice = dgInvoices.SelectedItem as Invoice;

            InvoiceWindow view = new InvoiceWindow(invoice.IsPurchase, invoice.ID);
            view.Owner = this;

            Mouse.OverrideCursor = null;

            view.ShowDialog();

            view.Close();

            GetData();
        }

        #endregion

        #region Экспорт в Excel

        /// <summary>
        /// Проверка возможности экспорта данных в Excel
        /// </summary>
        private void ExportExcel_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (InvoiceList.Count > 0) ? true : false;
        }

        /// <summary>
        /// Экспорт данных в Excel
        /// </summary>
        private void ExportExcel_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + FileNames.ReportBalanceTemplate))
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
                FileInfo template = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + FileNames.ReportBalanceTemplate);
                FileInfo report = template.CopyTo(fileName, true);

                excellApp = new Excel.Application(); // открываем Excel
                excellApp.Visible = false;
                excellApp.DisplayAlerts = false;

                workbook = excellApp.Workbooks.Open(report.FullName);
                workbook.DisplayInkComments = false;

                Excel.Worksheet worksheet = workbook.Worksheets.get_Item(1);

                // Заполнение периода
                worksheet.Cells[3, "D"] = ((DateTime)dpFromDate.SelectedDate).ToShortDateString();
                worksheet.Cells[3, "E"] = ((DateTime)dpTillDate.SelectedDate).ToShortDateString();

                // Заполнение итога
                worksheet.Cells[8, "G"] = SummSales.ToString().Replace(',','.');
                worksheet.Cells[8, "H"] = CouponSales.ToString().Replace(',', '.');
                worksheet.Cells[9, "G"] = SummPurchase.ToString().Replace(',', '.');
                worksheet.Cells[9, "H"] = CouponPurchase.ToString().Replace(',', '.');
                worksheet.Cells[10, "G"] = SummBalance.ToString().Replace(',', '.');
                worksheet.Cells[10, "H"] = CouponBalance.ToString().Replace(',', '.');

                // Заполнение накладных
                int i = 1;
                int currRow = 7;
                foreach(Invoice invoice in InvoiceList)
                {
                    // Добавление пустой строки
                    Excel.Range cellRange = (Excel.Range)worksheet.Cells[currRow, 1];
                    Excel.Range rowRange = cellRange.EntireRow;
                    rowRange.Insert(Excel.XlInsertShiftDirection.xlShiftDown, false);

                    // "№ п/п"
                    worksheet.Cells[currRow, "A"] = i++.ToString();

                    // Номер накладной
                    worksheet.Cells[currRow, "B"] = invoice.Number.ToString();

                    // Пользователь
                    worksheet.Cells[currRow, "D"] = invoice.AccountUser.PersonalData.VisibleName;

                    // Контрагент
                    worksheet.Cells[currRow, "E"] = invoice.CounterpartyUser.VisibleName;

                    // Дата проведения
                    worksheet.Cells[currRow, "F"] = invoice.Date.ToShortDateString();

                    if(invoice.IsPurchase)
                    {
                        // Тип накладной
                        worksheet.Cells[currRow, "C"] = "приходная".ToString();

                        // Сумма
                        worksheet.Cells[currRow, "G"] = (invoice.Cost * -1).ToString().Replace(',', '.');

                        // Купон
                        worksheet.Cells[currRow, "H"] = (invoice.Coupon * -1).ToString().Replace(',', '.');
                    }
                    else
                    {
                        // Тип накладной
                        worksheet.Cells[currRow, "C"] = "расходная".ToString();

                        // Сумма
                        worksheet.Cells[currRow, "G"] = invoice.Cost.ToString().Replace(',', '.');

                        // Купон
                        worksheet.Cells[currRow, "H"] = invoice.Coupon.ToString().Replace(',', '.');
                    }

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
                Dialog.ErrorMessage(this, "Ошибка выгрузки отчета в Excel", ex.Message);

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

        #endregion
    }
}