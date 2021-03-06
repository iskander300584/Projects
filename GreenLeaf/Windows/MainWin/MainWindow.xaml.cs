﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GreenLeaf.Classes;
using GreenLeaf.ViewModel;

//ConnectSetting.Server = "remotemysql.com";
//ServerAccount = aldr046@mail.ru
//ServerPassword = Password_1
//ConnectSetting.DB = "wYOq2RAf6y";
//ConnectSetting.AdminLogin = "wYOq2RAf6y";
//ConnectSetting.AdminPassword = "YA109CCuPp";

namespace GreenLeaf.Windows.MainWin
{
    /// <summary>
    /// Главное окно
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Список товаров на складе
        /// </summary>
        private List<Product> Warehouse = new List<Product>();

        #region Объявление команд

        // Обновить данные
        public static RoutedUICommand RefreshData = new RoutedUICommand("Обновить данные", "RefreshData", typeof(MainWindow));

        #region Накладные

        // Создать приходную накладную
        public static RoutedUICommand CreatePurchaseInvoice = new RoutedUICommand("Создать приходную накладную", "CreatePurchaseInvoice", typeof(MainWindow));

        // Создать расходную накладную
        public static RoutedUICommand CreateSalesInvoice = new RoutedUICommand("Создать расходную накладную", "CreateSalesInvoice", typeof(MainWindow));

        #endregion

        #region Отчеты

        /// <summary>
        /// Отчеты
        /// </summary>
        public static RoutedUICommand ReportPopup = new RoutedUICommand("Отчеты", "ReportPopup", typeof(MainWindow));

        #endregion

        #region Контрагенты

        public static RoutedUICommand CounterpartyPopup = new RoutedUICommand("Контрагенты", "CounterpartyPopup", typeof(MainWindow));

        #endregion

        #region Поиск

        // Поиск
        public static RoutedUICommand Search = new RoutedUICommand("Поиск", "Search", typeof(MainWindow));

        // Сбросить фильтры поиска
        public static RoutedUICommand ResetSearch = new RoutedUICommand("Сбросить фильтры поиска", "ResetSearch", typeof(MainWindow));

        #endregion

        #region Склад

        // Управление складом
        public static RoutedUICommand WarehouseManagement = new RoutedUICommand("Управление складом", "WarehouseManagement", typeof(MainWindow));

        // Добавление единицы товара
        public static RoutedUICommand WarehouseAddProduct = new RoutedUICommand("Добавление единицы товара", "WarehouseAddProduct", typeof(MainWindow));

        // Редактирование единицы товара
        public static RoutedUICommand WarehouseEditProduct = new RoutedUICommand("Редактирование единицы товара", "WarehouseEditProduct", typeof(MainWindow));

        // Аннулирование единицы товара
        public static RoutedUICommand WarehouseAnnulateProduct = new RoutedUICommand("Аннулирование единицы товара", "WarehouseAnnulateProduct", typeof(MainWindow));

        // Отмена аннулирования
        public static RoutedUICommand WarehouseUnAnnulateProduct = new RoutedUICommand("Отмена аннулирования", "WarehouseUnAnnulateProduct", typeof(MainWindow));

        // Редактирование количества товара
        public static RoutedUICommand WarehouseEditCount = new RoutedUICommand("Редактирование количества товара", "WarehouseEditCount", typeof(MainWindow));

        #endregion

        #region Панель администратора

        public static RoutedUICommand AdminPopupCommand = new RoutedUICommand("Панель администратора", "AdminPopupCommand", typeof(MainWindow));

        #endregion

        #endregion

        #region Загрузка программы

        /// <summary>
        /// Главное окно
        /// </summary>
        /// <param name="splash">окно загрузки</param>
        /// <param name="dtStart">время запуска программы</param>
        public MainWindow(Authentificate.SplashWindow splash, DateTime dtStart)
        {
            InitializeComponent();

            cbHideAnnuled.Checked += HideAnnuled_Checked;
            cbHideAnnuled.Unchecked += HideAnnuled_UnChecked;
            cbHideEmpty.Checked += HideEmpty_Change;
            cbHideEmpty.Unchecked += HideEmpty_Change;

            cbSortField.SelectedIndex = 0;
            cbSortField.SelectionChanged += cbSortField_SelectionChanged;

            btnSortDirection.Tag = "ascending";

            // Определение доступности элементов интерфейса
            SetControlsVisible();

            // Получение словаря единиц измерения
            MeasureUnit.GetMeasureUnits();

            // Загрузка данных в таблицу
            LoadData();

            int ms = DateTime.Now.Millisecond - dtStart.Millisecond;

            if (ms < 1500)
                System.Threading.Thread.Sleep(1500 - ms);
            splash.Close();
        }

        /// <summary>
        /// Определение доступности элементов интерфейса
        /// </summary>
        private void SetControlsVisible()
        {
            // приходная накладная
            this.btnPurchaseInvoice.Visibility = (ProgramSettings.CurrentUser.InvoiceData.PurchaseInvoice) ? Visibility.Visible : Visibility.Collapsed;

            // расходная накладная
            this.btnSalesInvoice.Visibility = (ProgramSettings.CurrentUser.InvoiceData.SalesInvoice) ? Visibility.Visible : Visibility.Collapsed;

            // отчеты
            this.btnReports.Visibility = (ProgramSettings.CurrentUser.ReportsData.Reports) ? Visibility.Visible : Visibility.Collapsed;

            // отчеты по приходным накладным
            this.btnReportsPurchaseInvoice.IsEnabled = ProgramSettings.CurrentUser.InvoiceData.PurchaseInvoice;

            // отчет по расходным накладным
            this.btnReportsSalesInvoice.IsEnabled = ProgramSettings.CurrentUser.InvoiceData.SalesInvoice;

            // отчет приход/расход
            this.btnReportsIncome.IsEnabled = ProgramSettings.CurrentUser.ReportsData.ReportIncomeExpense;

            // отчет остаток на складе
            this.btnReportsWarehouse.IsEnabled = ProgramSettings.CurrentUser.ReportsData.ReportIncomeExpense;

            // контрагенты
            this.btnCounterparty.Visibility = (ProgramSettings.CurrentUser.CounterpartyData.Counterparty) ? Visibility.Visible : Visibility.Collapsed;

            // список поставщиков
            this.btnReportsProviders.IsEnabled = ProgramSettings.CurrentUser.CounterpartyData.CounterpartyProvider;

            // список покупателей
            this.btnReportsClients.IsEnabled = ProgramSettings.CurrentUser.CounterpartyData.CounterpartyCustomer;

            #region Управление складом

            // управление складом
            this.btnWarehouseManagement.Visibility = (ProgramSettings.CurrentUser.WarehouseData.Warehouse) ? Visibility.Visible : Visibility.Collapsed;

            // скрыть аннулированные товары
            cbHideAnnuled.IsEnabled = ProgramSettings.CurrentUser.WarehouseData.WarehouseAnnulateProduct;

            #endregion

            // панель управления
            this.btnAdminPanel.Visibility = (ProgramSettings.CurrentUser.AdminPanelData.AdminPanel) ? Visibility.Visible : Visibility.Collapsed;

            // управление списком пользователей
            this.btnAdminUser.IsEnabled = (ProgramSettings.CurrentUser.AdminPanelData.AdminPanelAddAccount || ProgramSettings.CurrentUser.AdminPanelData.AdminPanelEditAccount || ProgramSettings.CurrentUser.AdminPanelData.AdminPanelDeleteAccount);

            // настройки программы
            this.btnAdminSettings.IsEnabled = ProgramSettings.CurrentUser.AdminPanelData.AdminPanelSetNumerator;

            // журнал событий
            this.btnAdminJournal.IsEnabled = ProgramSettings.CurrentUser.AdminPanelData.AdminPanelJournal;
        }

        #endregion

        #region Общие методы

        /// <summary>
        /// Скрыть всплывающие меню кнопок
        /// </summary>
        private void HidePopups()
        {
            ReportsPopup.IsOpen = false;
            CounterpartiesPopup.IsOpen = false;
            AdminPopup.IsOpen = false;
        }

        #endregion

        #region Отображение данных в таблице

        /// <summary>
        /// Загрузить данные о складе
        /// </summary>
        private void LoadData()
        {
            Mouse.OverrideCursor = Cursors.Wait;

            dataGrid.ItemsSource = null;

            try
            {
                Warehouse = Product.GetProductListByParameters((bool)cbHideAnnuled.IsChecked, (bool)cbHideEmpty.IsChecked, tbSearchProductCode.Text.Trim(), tbSearchNomination.Text.Trim());
            }
            catch(Exception ex)
            {
                Dialog.ErrorMessage(this, "Ошибка загрузки данных", ex.Message);
                Warehouse = new List<Product>();
            }

            SortData();

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Сортировка данных
        /// </summary>
        private void SortData()
        {
            dataGrid.ItemsSource = null;

            if (cbSortField.SelectedIndex == 0)
            {
                if(btnSortDirection.Tag.ToString() == "ascending")
                {
                    Warehouse = Warehouse.OrderBy(p => p.ProductCode).ToList();
                }
                else
                {
                    Warehouse = Warehouse.OrderByDescending(p => p.ProductCode).ToList();
                }
            }
            else
            {
                if (btnSortDirection.Tag.ToString() == "ascending")
                {
                    Warehouse = Warehouse.OrderBy(p => p.Nomination).ToList();
                }
                else
                {
                    Warehouse = Warehouse.OrderByDescending(p => p.Nomination).ToList();
                }
            }

            dataGrid.ItemsSource = Warehouse;
        }

        /// <summary>
        /// Обновить данные
        /// </summary>
        private void RefreshData_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            HidePopups();

            LoadData();
        }

        /// <summary>
        /// Поиск
        /// </summary>
        private void Search_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            HidePopups();

            LoadData();
        }

        /// <summary>
        /// Сброс фильтров поиска
        /// </summary>
        private void ResetSearch_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            HidePopups();

            tbSearchProductCode.Text = "";
            tbSearchNomination.Text = "";

            LoadData();
        }

        /// <summary>
        /// Проверка доступности кнопок поиска
        /// </summary>
        private void Search_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (tbSearchNomination.Text.Trim() != "" || tbSearchProductCode.Text.Trim() != "") ? true : false;
        }

        /// <summary>
        /// Смена флага скрытия отсутствующих
        /// </summary>
        private void HideEmpty_Change(object sender, RoutedEventArgs e)
        {
            HidePopups();

            LoadData();
        }

        /// <summary>
        /// Смена выбора поля сортировки
        /// </summary>
        private void cbSortField_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HidePopups();

            SortData();
        }

        /// <summary>
        /// Нажатие кнопки Сортировка
        /// </summary>
        private void btnSortDirection_Click(object sender, RoutedEventArgs e)
        {
            HidePopups();

            btnSortDirection.Tag = (btnSortDirection.Tag.ToString() == "ascending") ? "descending" : "ascending";

            SortData();
        }

        /// <summary>
        /// Нажатие ENTER в поисковом поле
        /// </summary>
        private void Search_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            HidePopups();

            if (e.Key == Key.Enter)
                LoadData();
        }

        #endregion

        #region Управление складом

        /// <summary>
        /// Запуск панели управления складом
        /// </summary>
        private void WarehouseManagement_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            stackWarehouseManagement.Visibility = (stackWarehouseManagement.Visibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;

            HidePopups();
        }

        /// <summary>
        /// Проверка возможности добавления единицы товара
        /// </summary>
        private void WarehouseAddProduct_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (ProgramSettings.CurrentUser.WarehouseData.Warehouse && ProgramSettings.CurrentUser.WarehouseData.WarehouseAddProduct) ? true : false;
        }

        /// <summary>
        /// Добавление единицы товара
        /// </summary>
        private void WarehouseAddProduct_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            HidePopups();

            Warehouse.AddProductWindow view = new Warehouse.AddProductWindow();

            view.Owner = this;

            if ((bool)view.ShowDialog())
            {
                view.Close();

                if (!(bool)cbHideEmpty.IsChecked)
                    LoadData();

                Dialog.TransparentMessage(this, "Операция выполнена");
            }
            else
                view.Close();
        }

        /// <summary>
        /// Проверка возможности редактирования единицы товара
        /// </summary>
        private void WarehouseEditProduct_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (ProgramSettings.CurrentUser.WarehouseData.Warehouse && ProgramSettings.CurrentUser.WarehouseData.WarehouseEditProduct && dataGrid.SelectedItem != null && !((Product)dataGrid.SelectedItem).IsAnnulated) ? true : false;
        }

        /// <summary>
        /// Редактирование единицы товара
        /// </summary>
        private void WarehouseEditProduct_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            HidePopups();

            if (dataGrid.SelectedItem == null)
            {
                Dialog.WarningMessage(this, "Не выбран товар");
                return;
            }

            try
            {
                Product product = (Product)dataGrid.SelectedItem;

                Warehouse.AddProductWindow editView = new Warehouse.AddProductWindow(true, product.ID);
                editView.Owner = this;

                if ((bool)editView.ShowDialog())
                {
                    editView.Close();

                    LoadData();

                    Dialog.TransparentMessage(this, "Операция выполнена");
                }
                else
                    editView.Close();
            }
            catch(Exception ex)
            {
                Dialog.ErrorMessage(this, "Ошибка редактирования товара", ex.Message);
            }
        }

        /// <summary>
        /// Проверка возможности аннулирования товара
        /// </summary>
        private void WarehouseAnnulateProduct_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (ProgramSettings.CurrentUser.WarehouseData.Warehouse && ProgramSettings.CurrentUser.WarehouseData.WarehouseAnnulateProduct && dataGrid.SelectedItem != null && !((Product)dataGrid.SelectedItem).IsAnnulated ) ? true : false;
        }

        /// <summary>
        /// Аннулирование товара
        /// </summary>
        private void WarehouseAnnulateProduct_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            HidePopups();

            if (dataGrid.SelectedItem == null)
            {
                Dialog.WarningMessage(this, "Не выбран товар");
                return;
            }

            try
            {
                Product product = (Product)dataGrid.SelectedItem;

                if (!product.IsAnnulated && Dialog.QuestionMessage(this, product.ProductCode + " будет аннулирован. Продолжить?") == MessageBoxResult.Yes)
                {
                    if(product.AnnulateProduct())
                    {
                        LoadData();

                        Dialog.TransparentMessage(this, "Операция выполнена");
                    }
                }
            }
            catch (Exception ex)
            {
                Dialog.ErrorMessage(this, "Ошибка аннулирования видов товара", ex.Message);
            }
        }

        /// <summary>
        /// Проверка возможности отмены аннулирования товара
        /// </summary>
        private void WarehouseUnAnnulateProduct_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (ProgramSettings.CurrentUser.WarehouseData.Warehouse && ProgramSettings.CurrentUser.WarehouseData.WarehouseAnnulateProduct && dataGrid.SelectedItem != null && ((Product)dataGrid.SelectedItem).IsAnnulated) ? true : false;
        }

        /// <summary>
        /// Отменить аннулирование товара
        /// </summary>
        private void WarehouseUnAnnulateProduct_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            HidePopups();

            if (dataGrid.SelectedItem == null)
            {
                Dialog.WarningMessage(this, "Не выбран товар");
                return;
            }

            try
            {
                Product product = (Product)dataGrid.SelectedItem;

                if (product.IsAnnulated && Dialog.QuestionMessage(this, "Для " + product.ProductCode + " будет отменено аннулирование. Продолжить?") == MessageBoxResult.Yes)
                {
                    if(product.UnAnnulateProduct())
                    {
                        LoadData();

                        Dialog.TransparentMessage(this, "Операция выполнена");
                    } 
                }
            }
            catch (Exception ex)
            {
                Dialog.ErrorMessage(this, "Ошибка отмены аннулирования товара", ex.Message);
            }
        }

        /// <summary>
        /// Скрыть аннулированные
        /// </summary>
        private void HideAnnuled_Checked(object sender, RoutedEventArgs e)
        {
            HidePopups();

            AnnuledColumn.Visibility = Visibility.Collapsed;
            LoadData();
        }

        /// <summary>
        /// Показать аннулированные
        /// </summary>
        private void HideAnnuled_UnChecked(object sender, RoutedEventArgs e)
        {
            HidePopups();

            AnnuledColumn.Visibility = Visibility.Visible;
            LoadData();
        }

        /// <summary>
        /// Проверка возможности редактирования количества
        /// </summary>
        private void WarehouseEditCount_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (ProgramSettings.CurrentUser.WarehouseData.Warehouse && ProgramSettings.CurrentUser.WarehouseData.WarehouseEditCount && dataGrid.SelectedItem != null && !((Product)dataGrid.SelectedItem).IsAnnulated) ? true : false;
        }

        /// <summary>
        /// Редактировать количество
        /// </summary>
        private void WarehouseEditCount_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            HidePopups();

            if (dataGrid.SelectedItem == null)
            {
                Dialog.WarningMessage(this, "Не выбран товар");
                return;
            }

            try
            {
                Product product = (Product)dataGrid.SelectedItem;

                Warehouse.EditCountWindow view = new Warehouse.EditCountWindow(product.Count, product.ID_Unit);
                view.Owner = this;

                if((bool)view.ShowDialog())
                {
                    product.Count = view.Count;
                    product.ID_Unit = view.ID_Unit;
                    view.Close();

                    if(product.EditCount())
                    {
                        Dialog.TransparentMessage(this, "Операция выполнена");
                    }
                }
                else
                    view.Close();
            }
            catch(Exception ex)
            {
                Dialog.ErrorMessage(this, "Ошибка изменения количества", ex.Message);
            }
        }

        #endregion

        #region Накладные

        /// <summary>
        /// Создать приходную накладную
        /// </summary>
        private void CreatePurchaseInvoice_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            HidePopups();

            InvoiceView.InvoiceWindow view = new InvoiceView.InvoiceWindow(true);
            view.Owner = this;

            Mouse.OverrideCursor = null;

            view.ShowDialog();
            view.Close();
            LoadData();
        }

        /// <summary>
        /// Создать расходную накладную
        /// </summary>
        private void CreateSalesInvoice_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            HidePopups();

            InvoiceView.InvoiceWindow view = new InvoiceView.InvoiceWindow(false);
            view.Owner = this;

            Mouse.OverrideCursor = null;

            view.ShowDialog();
            view.Close();
            LoadData();
        }

        #endregion

        #region Отчеты

        /// <summary>
        /// Отчеты
        /// </summary>
        private void ReportPopup_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            CounterpartiesPopup.IsOpen = false;
            AdminPopup.IsOpen = false;

            ReportsPopup.IsOpen = !ReportsPopup.IsOpen;
        }

        /// <summary>
        /// Отчет по приходным накладным
        /// </summary>
        private void btnReportsPurchaseInvoice_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            HidePopups();

            InvoiceView.InvoiceListWindow view = new InvoiceView.InvoiceListWindow(true);
            view.Owner = this;

            Mouse.OverrideCursor = null;

            view.ShowDialog();
            view.Close();
            LoadData();
        }

        /// <summary>
        /// Отчет по расходным накладным
        /// </summary>
        private void btnReportsSalesInvoice_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            HidePopups();

            InvoiceView.InvoiceListWindow view = new InvoiceView.InvoiceListWindow(false);
            view.Owner = this;

            Mouse.OverrideCursor = null;

            view.ShowDialog();
            view.Close();
            LoadData();
        }

        /// <summary>
        /// Отчет по балансу
        /// </summary>
        private void btnReportsIncome_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            HidePopups();

            Reports.ReportIncomeWindow view = new Reports.ReportIncomeWindow();
            view.Owner = this;

            Mouse.OverrideCursor = null;

            view.ShowDialog();
            view.Close();
        }

        /// <summary>
        /// Отчет по остаткам на складе
        /// </summary>
        private void btnReportsWarehouse_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            HidePopups();

            Reports.ReportWarehouseWindow view = new Reports.ReportWarehouseWindow();
            view.Owner = this;

            Mouse.OverrideCursor = null;

            view.ShowDialog();
            view.Close();
        }

        #endregion

        #region Контрагенты

        /// <summary>
        /// Контрагенты
        /// </summary>
        private void CounterpartyPopup_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            ReportsPopup.IsOpen = false;
            AdminPopup.IsOpen = false;

            CounterpartiesPopup.IsOpen = !CounterpartiesPopup.IsOpen;
        }

        /// <summary>
        /// Список поставщиков
        /// </summary>
        private void btnReportsProviders_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            HidePopups();

            CounterpartyView.CounterpartyListWindow view = new CounterpartyView.CounterpartyListWindow(true);
            view.Owner = this;

            Mouse.OverrideCursor = null;

            view.ShowDialog();
            view.Close();
        }

        /// <summary>
        /// Список клиентов
        /// </summary>
        private void btnReportsClients_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            HidePopups();

            CounterpartyView.CounterpartyListWindow view = new CounterpartyView.CounterpartyListWindow(false);
            view.Owner = this;

            Mouse.OverrideCursor = null;

            view.ShowDialog();
            view.Close();
        }

        #endregion

        #region Панель администратора

        /// <summary>
        /// Панель администратора
        /// </summary>
        private void AdminPopupCommand_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            CounterpartiesPopup.IsOpen = false;
            ReportsPopup.IsOpen = false;

            AdminPopup.IsOpen = !AdminPopup.IsOpen;
        }

        /// <summary>
        /// Пользователи
        /// </summary>
        private void AdminUsers_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            HidePopups();

            AdminPanel.AdminAccountListWindow view = new AdminPanel.AdminAccountListWindow();
            view.Owner = this;

            Mouse.OverrideCursor = null;

            view.ShowDialog();
            view.Close();
        }

        /// <summary>
        /// Настройки
        /// </summary>
        private void AdminSettings_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            HidePopups();

            AdminPanel.AdminSettingsWindow view = new AdminPanel.AdminSettingsWindow();
            view.Owner = this;

            Mouse.OverrideCursor = null;

            view.ShowDialog();
            view.Close();
        }

        /// <summary>
        /// Журнал событий
        /// </summary>
        private void AdminJournal_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            HidePopups();

            AdminPanel.AdminJournalWindow view = new AdminPanel.AdminJournalWindow();
            view.Owner = this;

            Mouse.OverrideCursor = null;

            view.ShowDialog();
            view.Close();
        }

        #endregion
    }
}