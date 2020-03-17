using GreenLeaf.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;
using GreenLeaf.Constants;
using GreenLeaf.Classes;

namespace GreenLeaf.Windows.Reports
{
    /// <summary>
    /// Окно отчета по остаткам на складе
    /// </summary>
    public partial class ReportWarehouseWindow : Window
    {
        #region Поля класса

        /// <summary>
        /// Список отображаемого товара
        /// </summary>
        private List<Product> ProductList;

        #endregion

        /// <summary>
        /// Окно отчета по остаткам на складе
        /// </summary>
        public ReportWarehouseWindow()
        {
            InitializeComponent();

            GetData();
        }

        /// <summary>
        /// Получение данных
        /// </summary>
        private void GetData()
        {
            Mouse.OverrideCursor = Cursors.Wait;

            dgProducts.ItemsSource = null;

            ProductList = Product.GetProductListByParameters(false, true);

            dgProducts.ItemsSource = ProductList;

            if (ProductList != null)
            {
                tbCount.Text = ProductList.Count.ToString();

                btnExcel.IsEnabled = (ProductList.Count > 0) ? true : false;
            }
            else
            {
                tbCount.Text = "0";
                btnExcel.IsEnabled = false;
            }

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Обновить данные
        /// </summary>
        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            GetData();
        }

        /// <summary>
        /// Выгрузить в Excel
        /// </summary>
        private void Excel_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + FileNames.ReportWarehouseTemplate))
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
                FileInfo template = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + FileNames.ReportWarehouseTemplate);
                FileInfo report = template.CopyTo(fileName, true);

                excellApp = new Excel.Application(); // открываем Excel
                excellApp.Visible = false;
                excellApp.DisplayAlerts = false;

                workbook = excellApp.Workbooks.Open(report.FullName);
                workbook.DisplayInkComments = false;

                Excel.Worksheet worksheet = workbook.Worksheets.get_Item(1);

                // Заполнение даты
                worksheet.Cells[3, "D"] = (DateTime.Today).ToShortDateString();
                
                // Заполнение товара
                int i = 1;
                int currRow = 7;
                foreach (Product product in ProductList)
                {
                    // Добавление пустой строки
                    Excel.Range cellRange = (Excel.Range)worksheet.Cells[currRow, 1];
                    Excel.Range rowRange = cellRange.EntireRow;
                    rowRange.Insert(Excel.XlInsertShiftDirection.xlShiftDown, false);

                    // "№ п/п"
                    worksheet.Cells[currRow, "A"] = i++.ToString();

                    // Код товара
                    worksheet.Cells[currRow, "B"] = product.ProductCode;

                    // Наименование
                    worksheet.Cells[currRow, "C"] = product.Nomination;

                    // Количество
                    worksheet.Cells[currRow, "D"] = product.Count;

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
    }
}