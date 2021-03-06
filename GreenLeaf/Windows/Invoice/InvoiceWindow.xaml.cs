﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GreenLeaf.Classes;
using GreenLeaf.ViewModel;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;
using GreenLeaf.Constants;

namespace GreenLeaf.Windows.InvoiceView
{
    /// <summary>
    /// Окно создания / редактирования накладной
    /// </summary>
    public partial class InvoiceWindow : Window
    {
        #region Объявление команд

        /// <summary>
        /// Команда блокировки накладной
        /// </summary>
        public static RoutedUICommand DoLockInvoice = new RoutedUICommand("Блокировка накладной", "DoLockInvoice", typeof(InvoiceWindow));

        /// <summary>
        /// Команда проведения накладной
        /// </summary>
        public static RoutedUICommand DoIssueInvoice = new RoutedUICommand("Проведение накладной", "DoIssueInvoice", typeof(InvoiceWindow));

        /// <summary>
        /// Команда удаления накладной
        /// </summary>
        public static RoutedUICommand DoDeleteInvoice = new RoutedUICommand("Удаление накладной", "DoDeleteInvoice", typeof(InvoiceWindow));

        /// <summary>
        /// Команда добавления элемента накладной
        /// </summary>
        public static RoutedUICommand DoAddItem = new RoutedUICommand("Добавление элемента накладной", "DoAddItem", typeof(InvoiceWindow));

        /// <summary>
        /// Команда редактирования элемента накладной
        /// </summary>
        public static RoutedUICommand DoEditItem = new RoutedUICommand("Редактирование элемента накладной", "DoEditItem", typeof(InvoiceWindow));

        /// <summary>
        /// Команда удаления элемента накладной
        /// </summary>
        public static RoutedUICommand DoDeleteItem = new RoutedUICommand("Удаление элемента накладной", "DoDeleteItem", typeof(InvoiceWindow));

        /// <summary>
        /// Команда вывода данных в Excel
        /// </summary>
        public static RoutedUICommand DoExcelOutput = new RoutedUICommand("Вывести в Excel", "DoExcelOutput", typeof(InvoiceWindow));

        // Поиск
        public static RoutedUICommand Search = new RoutedUICommand("Поиск", "Search", typeof(InvoiceWindow));

        // Сбросить фильтры поиска
        public static RoutedUICommand ResetSearch = new RoutedUICommand("Сбросить фильтры поиска", "ResetSearch", typeof(InvoiceWindow));

        #endregion

        /// <summary>
        /// Накладная
        /// </summary>
        private ViewModel.Invoice CurrentInvoice = new ViewModel.Invoice();

        /// <summary>
        /// Список контрагентов
        /// </summary>
        private List<Counterparty> Counterparties = new List<Counterparty>();

        /// <summary>
        /// Список не аннулированного товара
        /// </summary>
        private List<Product> ProductList = new List<Product>();

        /// <summary>
        /// Доступный товар
        /// </summary>
        private List<Product> ActualProduct = new List<Product>();

        /// <summary>
        /// Окно создания / редактирования накладной
        /// </summary>
        /// <param name="isPurchase">TRUE - приходная накладная, FALSE - расходная накладная</param>
        /// <param name="id">ID редактируемой накладной, если 0, создается новая накладная</param>
        public InvoiceWindow(bool isPurchase, int id=0)
        {
            InitializeComponent();

            this.Title = (isPurchase) ? "Приходная накладная" : "Расходная накладная";

            if(id == 0)
            {
                // Создание накладной
                CurrentInvoice = Invoice.CreateInvoice(isPurchase);
            }
            else
            {
                // Загрузка накладной
                CurrentInvoice = Invoice.GetInvoiceByID(isPurchase, id);
            }

            // Загрузка контрагентов
            if (isPurchase)
                Counterparties = Counterparty.GetProviderList().OrderBy(c => c.VisibleName).ToList();
            else
                Counterparties = Counterparty.GetCustomerList().OrderBy(c => c.VisibleName).ToList();

            foreach (Counterparty customer in Counterparties)
                cbCounterparty.Items.Add(customer.VisibleName);

            if (CurrentInvoice.ID_Counterparty != 0)
            {
                Counterparty currCounterparty = Counterparties.FirstOrDefault(c => c.ID == CurrentInvoice.ID_Counterparty);

                if (currCounterparty != null)
                    cbCounterparty.SelectedItem = currCounterparty.VisibleName;
                else
                {
                    CurrentInvoice.GetUsers();

                    if (CurrentInvoice.CounterpartyUser != null)
                    {
                        cbCounterparty.Items.Add(CurrentInvoice.CounterpartyUser.VisibleName);
                        cbCounterparty.SelectedItem = currCounterparty.VisibleName;
                    }
                    else
                        cbCounterparty.SelectedIndex = 0;
                }
            }

            // Загрузка списка товара
            ProductList = (isPurchase)? Product.GetActualProductList() : Product.GetProductListByParameters(true, true);

            GetFreeProductList();

            tbExecutor.Text = Account.GetAccountByID(CurrentInvoice.ID_Account).PersonalData.VisibleName;

            // Активация элементов в зависимости от типа накладной
            if(CurrentInvoice.IsPurchase)
            {
                btnLockInvoice.Visibility = Visibility.Collapsed;
                grColumnCostSales.Visibility = Visibility.Collapsed;
                grColumnCouponSales.Visibility = Visibility.Collapsed;
                grColumnCountSales.Visibility = Visibility.Collapsed;
            }
            else
            {
                grColumnCostPurchase.Visibility = Visibility.Collapsed;
                grColumnCountPurchase.Visibility = Visibility.Collapsed;
                grColumnCouponPurchase.Visibility = Visibility.Collapsed;
            }

            this.DataContext = CurrentInvoice;
        }

        /// <summary>
        /// Смена выбранного контрагента
        /// </summary>
        private void cbCounterparty_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            Counterparty currCounterparty = Counterparties.FirstOrDefault(c => c.VisibleName == cbCounterparty.SelectedItem.ToString());

            if (currCounterparty != null)
            {
                CurrentInvoice.ID_Counterparty = currCounterparty.ID;
                CurrentInvoice.EditInvoice();
            }

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Заполнить список доступного товара
        /// </summary>
        private void GetFreeProductList()
        {
            dataGridAllowed.ItemsSource = null;

            ActualProduct.Clear();

            // Заполнение списка не использованного товара
            foreach (Product product in ProductList)
            {
                InvoiceItem item = CurrentInvoice.Items.FirstOrDefault(i => i.ID_Product == product.ID);

                string code = tbSearchProductCode.Text.Trim().ToLower();
                string nomination = tbSearchNomination.Text.Trim().ToLower();

                if (item == null)
                {
                    if (code == "" && nomination == "")
                    {
                        ActualProduct.Add(product);
                    }
                    else if(code != "" && nomination != "")
                    {
                        if(product.ProductCode.ToLower().Contains(code) && product.Nomination.ToLower().Contains(nomination))
                            ActualProduct.Add(product);
                    }
                    else if(code != "")
                    {
                        if (product.ProductCode.ToLower().Contains(code))
                            ActualProduct.Add(product);
                    }
                    else
                    {
                        if (product.Nomination.ToLower().Contains(nomination))
                            ActualProduct.Add(product);
                    }
                }
            }

            dataGridAllowed.ItemsSource = ActualProduct;
        }

        /// <summary>
        /// Попытка ввода текста в количество
        /// </summary>
        private void TextBlock_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = NumericTextBoxMethods.DoubleTextBox_PreviewKeyDown(tbCount.Text, e);
        }

        /// <summary>
        /// Ввод текста в количество
        /// </summary>
        private void TextBlock_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                DoAddItem_Execute(null, null);
            else
                e.Handled = NumericTextBoxMethods.DoubleTextBox_PreviewKeyDown(tbCount.Text, e);
        }

        /// <summary>
        /// Изменение количества
        /// </summary>
        private void TextBlock_TextChanged(object sender, TextChangedEventArgs e)
        {
            NumericTextBoxMethods.DoubleTextBox_TextChanged(tbCount);
        }

        /// <summary>
        /// Проверка возможности блокировки накладной
        /// </summary>
        private void DoLockInvoice_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (CurrentInvoice != null && !CurrentInvoice.IsPurchase && !CurrentInvoice.IsIssued && CurrentInvoice.Items != null && CurrentInvoice.Items.Count > 0) ? true : false;
        }

        /// <summary>
        /// Блокировка (разблокировка) накладной
        /// </summary>
        private void DoLockInvoice_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            bool res = false;

            if (CurrentInvoice.IsLocked)
                res = CurrentInvoice.UnLockInvoice();
            else
                res = CurrentInvoice.LockInvoice();

            if(res)
                Dialog.TransparentMessage(this, "Операция выполнена");

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Проверка возможности проведения накладной
        /// </summary>
        private void DoIssueInvoice_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (CurrentInvoice != null && !CurrentInvoice.IsLocked && CurrentInvoice.ID_Counterparty != 0 && CurrentInvoice.Items != null && CurrentInvoice.Items.Count > 0 && (!CurrentInvoice.IsIssued || (CurrentInvoice.IsPurchase && ProgramSettings.CurrentUser.ReportsData.ReportUnIssuePurchaseInvoice) || (!CurrentInvoice.IsPurchase && ProgramSettings.CurrentUser.ReportsData.ReportUnIssueSalesInvoice))) ? true : false;
        }

        /// <summary>
        /// Проведение (отмена проведения) накладной
        /// </summary>
        private void DoIssueInvoice_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            bool res = false;

            if (CurrentInvoice.IsIssued)
            {
                if (Dialog.QuestionMessage(this, "Отменить проведение накладной?", "Внимание!") == MessageBoxResult.Yes)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    res = CurrentInvoice.UnIssueInvoice();
                }
            }
            else
            {
                if (Dialog.QuestionMessage(this, "Провести накладную?", "Внимание!") == MessageBoxResult.Yes)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    res = CurrentInvoice.IssueInvoice();
                }
            }

            if(res)
                Dialog.TransparentMessage(this, "Операция выполнена");

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Проверка возможности удаления накладной
        /// </summary>
        private void DoDeleteInvoice_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (CurrentInvoice != null && !CurrentInvoice.IsLocked && !CurrentInvoice.IsIssued) ? true : false;
        }

        /// <summary>
        /// Удаление накладной
        /// </summary>
        private void DoDeleteInvoice_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if(Dialog.QuestionMessage(this, "Удалить накладную?", "Внимание!") == MessageBoxResult.Yes)
            {
                Mouse.OverrideCursor = Cursors.Wait;

                if (CurrentInvoice.DeleteInvoice())
                {
                    Dialog.TransparentMessage(this, "Операция выполнена");

                    Mouse.OverrideCursor = null;
                    this.DialogResult = false;
                }

                Mouse.OverrideCursor = null;
            }
        }

        /// <summary>
        /// Проверка возможности добавления элемента накладной
        /// </summary>
        private void DoAddItem_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (CurrentInvoice != null && CurrentInvoice.EditEnabled && dataGridAllowed != null && dataGridAllowed.SelectedItem != null && tbCount != null && tbCount.Text.Trim() != "") ? true : false;
        }

        /// <summary>
        /// Добавление элемента накладной
        /// </summary>
        private void DoAddItem_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if(dataGridAllowed.SelectedItem == null)
            {
                //Dialog.WarningMessage(this, "Не выбран товар");
                return;
            }

            Product product = dataGridAllowed.SelectedItem as Product;
            if(product == null)
            {
                Dialog.WarningMessage(this, "Не удалось получить выбранный товар");
                return;
            }

            double count = 0;
            if(!double.TryParse(tbCount.Text.Trim().Replace('.',','), out count))
            {
                Dialog.WarningMessage(this, "Ошибка получения количества товара");
                return;
            }

            if(count <= 0)
            {
                Dialog.WarningMessage(this, "Не корректное количество количество товара");
                return;
            }

            if(!CurrentInvoice.IsPurchase && count > product.AllowedCount)
            {
                Dialog.WarningMessage(this, "Указанное количество превышает допустимое количество товара");
                return;
            }

            Mouse.OverrideCursor = Cursors.Wait;

            // определение стоимости и купона
            double _cost = (CurrentInvoice.IsPurchase) ? product.CostPurchase : product.Cost;
            double _coupon = (CurrentInvoice.IsPurchase) ? product.CouponPurchase : product.Coupon;

            CurrentInvoice.Items.Add(InvoiceItem.CreateItem(CurrentInvoice.ID, product.ID, _cost, _coupon, count, CurrentInvoice.IsPurchase));

            GetFreeProductList();

            CurrentInvoice.Calc();

            CurrentInvoice.EditInvoice();

            dataGrid.Items.Refresh();

            tbCount.Text = "";

            Dialog.TransparentMessage(this, "Операция выполнена");

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Проверка возможности редактирования элемента накладной
        /// </summary>
        private void DoEditItem_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (CurrentInvoice != null && CurrentInvoice.EditEnabled && dataGrid != null && dataGrid.SelectedItem != null) ? true : false;
        }

        /// <summary>
        /// Редактирование элемента накладной
        /// </summary>
        private void DoEditItem_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            InvoiceItem item = dataGrid.SelectedItem as InvoiceItem;

            EditItemCountWindow view = (CurrentInvoice.IsPurchase) ? new EditItemCountWindow(item.Count, item.Product.ProductCode, item.Product.Nomination, true, item.ProductCost, item.ProductCoupon) : new EditItemCountWindow(item.Count, item.Product.ProductCode, item.Product.Nomination, item.Product.AllowedCount, false);

            view.Owner = this;

            if ((bool)view.ShowDialog())
            {
                Mouse.OverrideCursor = Cursors.Wait;

                item.Count = view.NewCount;

                if(CurrentInvoice.IsPurchase)
                {
                    item.ProductCost = view.NewCost;
                    item.ProductCoupon = view.NewCoupon;
                }

                view.Close();

                if(item.EditItem(CurrentInvoice.IsPurchase))
                {
                    CurrentInvoice.Calc();

                    CurrentInvoice.EditInvoice();

                    dataGrid.Items.Refresh();

                    Dialog.TransparentMessage(this, "Операция выполнена");
                }

                Mouse.OverrideCursor = null;
            }
            else
                view.Close();
        }

        /// <summary>
        /// Проверка возможности удаления элемента накладной
        /// </summary>
        private void DoDeleteItem_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (CurrentInvoice != null && CurrentInvoice.EditEnabled && dataGrid != null && dataGrid.SelectedItem != null) ? true : false;
        }

        /// <summary>
        /// Удаление элемента накладной
        /// </summary>
        private void DoDeleteItem_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            InvoiceItem item = dataGrid.SelectedItem as InvoiceItem;

            if (item != null)
            {
                if (Dialog.QuestionMessage(this, "Удалить позицию " + item.Product.ProductCode + "?", "Внимание!") != MessageBoxResult.Yes)
                    return;

                Mouse.OverrideCursor = Cursors.Wait;

                if (InvoiceItem.DeleteItem(item.ID, CurrentInvoice.IsPurchase))
                {
                    CurrentInvoice.Items.Remove(item);

                    GetFreeProductList();

                    CurrentInvoice.Calc();

                    CurrentInvoice.EditInvoice();

                    dataGrid.Items.Refresh();

                    Dialog.TransparentMessage(this, "Операция выполнена");
                }

                Mouse.OverrideCursor = null;
            }
        }

        /// <summary>
        /// Проверка возможности вывода в Excel
        /// </summary>
        private void DoExcelOutput_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (CurrentInvoice != null && CurrentInvoice.Items != null && CurrentInvoice.Items.Count > 0) ? true : false;
        }

        /// <summary>
        /// Вывод в Excel
        /// </summary>
        private void DoExcelOutput_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if(!File.Exists(AppDomain.CurrentDomain.BaseDirectory + FileNames.InvoiceTemplate))
            {
                Dialog.ErrorMessage(this, "Шаблон накладной не найден. Обратитесь к администратору");
                return;
            }

            Mouse.OverrideCursor = Cursors.Wait;

            try
            {
                bool doLoadInvoice = true; // признак необходимости выгрузки накладной

                string fileName = string.Empty;

                if (CurrentInvoice.IsPurchase && ProgramSettings.Settings[SettingsNames.PurchaseInvoicesPath] != "")
                    fileName = ProgramSettings.Settings[SettingsNames.PurchaseInvoicesPath];
                else if (!CurrentInvoice.IsPurchase && ProgramSettings.Settings[SettingsNames.SalesInvoicesPath] != "")
                    fileName = ProgramSettings.Settings[SettingsNames.SalesInvoicesPath];

                CurrentInvoice.GetUsers();

                if (fileName != string.Empty && Directory.Exists(fileName))
                {
                    DateTime dt = DateTime.Today;
                    if (fileName[fileName.Length - 1] != '\\')
                        fileName += @"\";
                    fileName += Conversion.ToStringDateReverse(DateTime.Today);

                    if (CurrentInvoice.CounterpartyUser != null)
                        fileName += " " + CurrentInvoice.CounterpartyUser.VisibleName + ".xls";
                    else
                        fileName += ".xls";
                }
                else
                {
                    SaveFileDialog saveDialog = new SaveFileDialog();

                    saveDialog.Filter = "Файлы Excel|*.xls";
                    saveDialog.FilterIndex = 0;
                    saveDialog.FileName = "";
                    saveDialog.CheckPathExists = true;
                    saveDialog.CheckFileExists = false;

                    Mouse.OverrideCursor = null;

                    doLoadInvoice = (bool)saveDialog.ShowDialog();
                    if(doLoadInvoice)
                        fileName = saveDialog.FileName;
                }

                if (doLoadInvoice)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    
                    if (!fileName.Contains(".xls"))
                        fileName += ".xls";

                    // Копирование шаблона накладной
                    FileInfo template = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + FileNames.InvoiceTemplate);
                    FileInfo report = template.CopyTo(fileName, true);

                    Excel.Application excellApp = new Excel.Application(); // открываем Excel
                    excellApp.Visible = false;
                    excellApp.DisplayAlerts = false;

                    Excel.Workbook workbook = excellApp.Workbooks.Open(report.FullName);
                    workbook.DisplayInkComments = false;

                    Excel.Worksheet worksheet = workbook.Worksheets.get_Item(1);

                    // Заполнение ячейки "Номер документа"
                    if (CurrentInvoice.Number != 0)
                        worksheet.Cells[5, 6] = CurrentInvoice.Number.ToString();

                    // Заполнение ячейки "Дата составления"
                    worksheet.Cells[5, 8] = (CurrentInvoice.Date != DateTime.MinValue) ? CurrentInvoice.Date.ToShortDateString() : CurrentInvoice.CreateDate.ToShortDateString();

                    // Заполнение ячейки "Итого количество"
                    worksheet.Cells[11, 10] = CurrentInvoice.Items.Count.ToString();

                    // Заполнение ячейки "Итого стоимость"
                    worksheet.Cells[11, 11] = CurrentInvoice.Cost.ToString() + " ₽";

                    // Заполнение ячейки "Итого купон"
                    worksheet.Cells[11, 12] = CurrentInvoice.Coupon.ToString() + " ₽";

                    if (CurrentInvoice.ID_Counterparty != 0)
                    {
                        // Получение контрагента
                        string counterpartyName = "";                        
                        if (CurrentInvoice.CounterpartyUser != null && CurrentInvoice.CounterpartyUser.Surname != "")
                        {
                            counterpartyName = CurrentInvoice.CounterpartyUser.Surname;

                            if (CurrentInvoice.CounterpartyUser.Name != "")
                            {
                                counterpartyName += " " + CurrentInvoice.CounterpartyUser.Name[0] + ".";

                                if (CurrentInvoice.CounterpartyUser.Patronymic != "")
                                    counterpartyName += CurrentInvoice.CounterpartyUser.Patronymic[0] + ".";
                            }
                        }

                        // Заполнение ячеек "Отправитель" и "Получатель"
                        if (CurrentInvoice.IsPurchase)
                        {
                            if (CurrentInvoice.CounterpartyUser != null)
                            {
                                worksheet.Cells[1, 3] = CurrentInvoice.CounterpartyUser.VisibleName;
                                worksheet.Cells[1, 11] = CurrentInvoice.CounterpartyUser.Code;
                                worksheet.Cells[14, 4] = counterpartyName;
                            }

                            worksheet.Cells[2, 3] = ProgramSettings.Settings[SettingsNames.CompanyNameForInvoice] + "   " + ProgramSettings.Settings[SettingsNames.ServiceCodeForInvoice];
                            worksheet.Cells[2, 11] = ProgramSettings.Settings[SettingsNames.RUCodeForInvoice];
                            worksheet.Cells[16, 4] = CurrentInvoice.AccountUser.PersonalData.VisibleName;
                        }
                        else
                        {
                            worksheet.Cells[1, 3] = ProgramSettings.Settings[SettingsNames.CompanyNameForInvoice] + "   " + ProgramSettings.Settings[SettingsNames.ServiceCodeForInvoice];
                            worksheet.Cells[1, 11] = ProgramSettings.Settings[SettingsNames.RUCodeForInvoice];
                            worksheet.Cells[14, 4] = CurrentInvoice.AccountUser.PersonalData.VisibleName;

                            if (CurrentInvoice.CounterpartyUser != null)
                            {
                                worksheet.Cells[2, 3] = CurrentInvoice.CounterpartyUser.VisibleName;
                                worksheet.Cells[2, 11] = CurrentInvoice.CounterpartyUser.Code;
                                worksheet.Cells[16, 4] = counterpartyName;
                            }
                        }
                    }

                    int i = CurrentInvoice.Items.Count; // счетчик элементов накладной

                    // Заполнение сервисного сбора
                    if (!CurrentInvoice.IsPurchase)
                    {
                        // Добавление пустой строки
                        Excel.Range cellRange = (Excel.Range)worksheet.Cells[11, 1];
                        Excel.Range rowRange = cellRange.EntireRow;
                        rowRange.Insert(Excel.XlInsertShiftDirection.xlShiftDown, false);

                        // Заполнение ячейки "5%"
                        worksheet.Cells[11, 9] = "5 %";

                        // Заполнение ячейки "5% количество"
                        worksheet.Cells[11, 10] = "-";

                        // Заполнение ячейки "5% стоимость"
                        worksheet.Cells[11, 11] = CurrentInvoice.ServiceCharge.ToString() + " ₽";

                        // Заполнение ячейки "5% купон"
                        worksheet.Cells[11, 12] = "0 ₽";

                        // Добавление пустой строки
                        cellRange = (Excel.Range)worksheet.Cells[11, 1];
                        rowRange = cellRange.EntireRow;
                        rowRange.Insert(Excel.XlInsertShiftDirection.xlShiftDown, false);

                        // Заполнение ячейки "Сумма"
                        worksheet.Cells[11, 9] = "Сумма";

                        // Заполнение ячейки "Сумма количество"
                        worksheet.Cells[11, 10] = "-";

                        // Заполнение ячейки "Сумма стоимость"
                        worksheet.Cells[11, 11] = (CurrentInvoice.Cost - CurrentInvoice.ServiceCharge).ToString() + " ₽";

                        // Заполнение ячейки "Сумма купон"
                        worksheet.Cells[11, 12] = CurrentInvoice.Coupon.ToString() + " ₽"; 
                    }

                    // Заполнение элементов накладной
                    foreach (InvoiceItem item in CurrentInvoice.Items.OrderByDescending(it => it.Product.ProductCode))
                    {
                        // Добавление пустой строки
                        Excel.Range cellRange = (Excel.Range)worksheet.Cells[10, 1];
                        Excel.Range rowRange = cellRange.EntireRow;
                        rowRange.Insert(Excel.XlInsertShiftDirection.xlShiftDown, false);

                        // Объединение ячеек Наименование
                        Excel.Range range = worksheet.get_Range("B10", "C10");
                        range.Merge(Type.Missing);

                        // Объединение ячеек Код товара
                        range = worksheet.get_Range("D10", "E10");
                        range.Merge(Type.Missing);

                        // Заполнение ячейки "№ п/п"
                        worksheet.Cells[11, 1] = i--.ToString();

                        // Заполнение ячейки "Наименование"
                        worksheet.Cells[11, 2] = item.Product.Nomination;

                        // Заполнение ячейки "Код товара"
                        worksheet.Cells[11, 4] = item.Product.ProductCode;

                        // Заполнение ячейки "Количество в упаковке"
                        worksheet.Cells[11, 6] = item.Product.CountInPackage.ToString();

                        // Заполнение ячейки "Единица измерения"
                        worksheet.Cells[11, 7] = item.Product.UnitNomination;

                        // Заполнение ячейки "Стоимость"
                        worksheet.Cells[11, 8] = item.ProductCost.ToString() + " ₽";

                        // Заполнение ячейки "Купон"
                        worksheet.Cells[11, 9] = item.ProductCoupon.ToString() + " ₽";

                        // Заполнение ячейки "Количество"
                        worksheet.Cells[11, 10] = item.Count.ToString();

                        // Заполнение ячейки "Итого стоимость"
                        worksheet.Cells[11, 11] = item.Cost.ToString() + " ₽";

                        // Заполнение ячейки "Итого купон"
                        worksheet.Cells[11, 12] = item.Coupon.ToString() + " ₽";
                    }

                    // Удаление пустых строк
                    Excel.Range cRange = (Excel.Range)worksheet.Cells[10, 1];
                    Excel.Range rRange = cRange.EntireRow;
                    rRange.Delete(Excel.XlDeleteShiftDirection.xlShiftUp);
                    cRange = (Excel.Range)worksheet.Cells[9, 1];
                    rRange = cRange.EntireRow;
                    rRange.Delete(Excel.XlDeleteShiftDirection.xlShiftUp);

                    // сохранение накладной
                    workbook.Save();

                    // Закрытие Excel
                    workbook.Close(false);
                    excellApp.Quit();

                    workbook = null;
                    excellApp = null;

                    // Запуск отчета
                    Process.Start(report.FullName);
                }
            }
            catch(Exception ex)
            {
                Dialog.ErrorMessage(this, "Ошибка выгрузки накладной в Excel", ex.Message);
            }

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Нажатие ENTER в поиске
        /// </summary>
        private void SearchText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Mouse.OverrideCursor = Cursors.Wait;

                GetFreeProductList();

                e.Handled = true;

                Mouse.OverrideCursor = null;
            }
        }

        /// <summary>
        /// Проверка возможности поска доступной позиции
        /// </summary>
        private void Search_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((tbSearchNomination != null && tbSearchNomination.Text.Trim() != "") || (tbSearchProductCode != null && tbSearchProductCode.Text.Trim() != "")) ? true : false;
        }

        /// <summary>
        /// Поиск доступной позиции
        /// </summary>
        private void Search_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            GetFreeProductList();

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Проверка возможности сброса фильтров поиска
        /// </summary>
        private void ResetSearch_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((tbSearchNomination != null && tbSearchNomination.Text.Trim() != "") || (tbSearchProductCode != null && tbSearchProductCode.Text.Trim() != "")) ? true : false;
        }

        /// <summary>
        /// Сброс фильтров поиска
        /// </summary>
        private void ResetSearch_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            tbSearchNomination.Text = "";
            tbSearchProductCode.Text = "";

            GetFreeProductList();

            Mouse.OverrideCursor = null;
        } 
    }
}