using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GreenLeaf.Classes;
using GreenLeaf.ViewModel;
using MySql.Data.MySqlClient;

//ConnectSetting.Server = "remotemysql.com";
//ServerAccount = aldr046@mail.ru
//ServerPassword = Password_1
//ConnectSetting.DB = "wLhC3u9El4";
//ConnectSetting.AdminLogin = "wLhC3u9El4";
//ConnectSetting.AdminPassword = "briyOnlYc8";

namespace GreenLeaf.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Product> Warehouse = new List<Product>();

        #region Объявление команд

        // Обновить данные
        public static RoutedUICommand RefreshData = new RoutedUICommand("Обновить данные", "RefreshData", typeof(MainWindow));

        // Создать приходную накладную
        public static RoutedUICommand CreatePurchaseInvoice = new RoutedUICommand("Создать приходную накладную", "CreatePurchaseInvoice", typeof(MainWindow));

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

        #endregion

        public MainWindow(Windows.Authentificate.SplashWindow splash, DateTime dtStart)
        {
            InitializeComponent();

            cbHideAnnuled.Checked += HideAnnuled_Checked;
            cbHideAnnuled.Unchecked += HideAnnuled_UnChecked;
            cbHideEmpty.Checked += HideEmpty_Change;
            cbHideEmpty.Unchecked += HideEmpty_Change;

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
            this.btnPurchaseInvoice.Visibility = (ConnectSetting.CurrentUser.InvoiceData.PurchaseInvoice) ? Visibility.Visible : Visibility.Collapsed;

            // расходная накладная
            this.btnSalesInvoice.Visibility = (ConnectSetting.CurrentUser.InvoiceData.SalesInvoice) ? Visibility.Visible : Visibility.Collapsed;

            // отчеты
            this.btnReports.Visibility = (ConnectSetting.CurrentUser.ReportsData.Reports) ? Visibility.Visible : Visibility.Collapsed;

            // контрагенты
            this.btnCounterparty.Visibility = (ConnectSetting.CurrentUser.CounterpartyData.Counterparty) ? Visibility.Visible : Visibility.Collapsed;

            #region Управление складом

            // управление складом
            this.btnWarehouseManagement.Visibility = (ConnectSetting.CurrentUser.WarehouseData.Warehouse) ? Visibility.Visible : Visibility.Collapsed;

            // добавление товара
            btnAddProduct.IsEnabled = ConnectSetting.CurrentUser.WarehouseData.WarehouseAddProduct;

            // редактирование товара
            btnEditProduct.IsEnabled = ConnectSetting.CurrentUser.WarehouseData.WarehouseEditProduct;

            // аннулирование товара
            btnAnnulateProduct.IsEnabled = ConnectSetting.CurrentUser.WarehouseData.WarehouseAnnulateProduct;
            btnUnAnnulateProduct.IsEnabled = ConnectSetting.CurrentUser.WarehouseData.WarehouseAnnulateProduct;
            cbHideAnnuled.IsEnabled = ConnectSetting.CurrentUser.WarehouseData.WarehouseAnnulateProduct;

            // редактирование количества
            btnEditProductCount.IsEnabled = ConnectSetting.CurrentUser.WarehouseData.WarehouseEditCount;

            #endregion

            // панель управления
            this.btnAdminPanel.Visibility = (ConnectSetting.CurrentUser.AdminPanelData.AdminPanel) ? Visibility.Visible : Visibility.Collapsed;
        }

        #region Отображение данных в таблице

        /// <summary>
        /// Загрузить данные о складе
        /// </summary>
        private void LoadData()
        {
            this.Cursor = Cursors.Wait;

            try
            {
                string sql = "SELECT * FROM PRODUCT";

                // Добавление условий поиска
                string condition = string.Empty;
                string code = tbSearchProductCode.Text.Trim();
                string nomination = tbSearchNomination.Text.Trim();

                if ((bool)cbHideAnnuled.IsChecked || (bool)cbHideEmpty.IsChecked ||
                    code != "" || nomination != "")
                {
                    condition = " WHERE ";

                    bool isAdded = false;

                    if ((bool)cbHideAnnuled.IsChecked)
                    {
                        condition += @"`PRODUCT`.`IS_ANNULED`='0'";
                        isAdded = true;
                    }

                    if((bool)cbHideEmpty.IsChecked)
                    {
                        if (isAdded)
                            condition += " AND ";

                        condition += "`PRODUCT`.`COUNT`>0";
                        isAdded = true;
                    }

                    if (code != "")
                    {
                        if (isAdded)
                            condition += " AND ";

                        condition += "`PRODUCT`.`PRODUCT_CODE` LIKE \'%" + code + "%\'";
                        isAdded = true;
                    }

                    if (nomination != "")
                    {
                        if (isAdded)
                            condition += " AND ";

                        condition += "`PRODUCT`.`NOMINATION` LIKE \'%" + nomination + "%\'";
                        isAdded = true;
                    }

                    sql += condition;
                }

                using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                {
                    connection.Open();

                    dataGrid.ItemsSource = null;
                    Warehouse = new List<Product>();

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        MySqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            Product item = new Product();

                            string tempS = reader["ID"].ToString();
                            int tempI = 0;
                            if (int.TryParse(tempS, out tempI))
                                item.ID = tempI;
                            else
                                item.ID = 0;

                            tempS = reader["COUNT"].ToString();
                            double tempD = 0;
                            if (double.TryParse(tempS, out tempD))
                                item.Count = tempD;
                            else
                                item.Count = tempD;

                            tempS = reader["ID_UNIT"].ToString();
                            if (int.TryParse(tempS, out tempI))
                                item.ID_Unit = tempI;
                            else
                                item.ID_Unit = 0;

                            item.ProductCode = reader["PRODUCT_CODE"].ToString();
                            item.Nomination = reader["NOMINATION"].ToString();

                            tempS = reader["COUNT_IN_PACKAGE"].ToString();
                            if (double.TryParse(tempS, out tempD))
                                item.CountInPackage = tempD;
                            else
                                item.CountInPackage = 0;

                            tempS = reader["COST"].ToString();
                            if (double.TryParse(tempS, out tempD))
                                item.Cost = tempD;
                            else
                                item.Cost = 0;

                            tempS = reader["COUPON"].ToString();
                            if (double.TryParse(tempS, out tempD))
                                item.Coupon = tempD;
                            else
                                item.Coupon = 0;

                            tempS = reader["IS_ANNULED"].ToString();
                            bool tempB = false;
                            if (bool.TryParse(tempS, out tempB))
                                item.IsAnnulated = tempB;
                            else
                                item.IsAnnulated = false;

                            Warehouse.Add(item);
                        }
                    }

                    connection.Close();
                }

                dataGrid.ItemsSource = Warehouse.OrderBy(p => p.ProductCode);
            }
            catch (Exception ex)
            {
                Dialog.ErrorMessage(this, "Ошибка загрузки данных", ex.Message);
            }

            this.Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Обновить данные
        /// </summary>
        private void RefreshData_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// Поиск
        /// </summary>
        private void Search_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// Сброс фильтров поиска
        /// </summary>
        private void ResetSearch_Execute(object sender, ExecutedRoutedEventArgs e)
        {
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
        }

        /// <summary>
        /// Добавление единицы товара
        /// </summary>
        private void WarehouseAddProduct_Execute(object sender, ExecutedRoutedEventArgs e)
        {
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
        /// Редактирование единицы товара
        /// </summary>
        private void WarehouseEditProduct_Execute(object sender, ExecutedRoutedEventArgs e)
        {
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
        /// Аннулирование товара
        /// </summary>
        private void WarehouseAnnulateProduct_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if(dataGrid.SelectedItem == null)
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

                    /*using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                    {
                        connection.Open();

                        using (MySqlCommand command = new MySqlCommand())
                        {
                            command.Connection = connection;

                            // Аннулирование товара
                            string sql = "UPDATE `PRODUCT` SET `IS_ANNULED`=\'1\' WHERE `PRODUCT`.`ID`=" + product.ID.ToString();

                            command.CommandText = sql;
                            command.ExecuteNonQuery();

                            // Запись в журнал событий
                            string act = JournalMethods.JournalItemHeader("аннулировал") + "вид товара " + product.ProductCode;
                            sql = @"INSERT INTO `JOURNAL` (`DATE`, `ID_ACCOUNT`, `ACT`) VALUES ('" + JournalMethods.CurrentDate() + @"', '" +
                                ConnectSetting.User.Person.ID.ToString() + @"', '" + act + @"');";

                            command.CommandText = sql;
                            command.ExecuteNonQuery();
                        }

                        connection.Close();
                    }

                    LoadData();

                    Dialog.TransparentMessage(this, "Операция выполнена");*/
                }
            }
            catch (Exception ex)
            {
                Dialog.ErrorMessage(this, "Ошибка аннулирования видов товара", ex.Message);
            }
        }

        /// <summary>
        /// Отменить аннулирование товара
        /// </summary>
        private void WarehouseUnAnnulateProduct_Execute(object sender, ExecutedRoutedEventArgs e)
        {
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

                    /*using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                    {
                        connection.Open();

                        using (MySqlCommand command = new MySqlCommand())
                        {
                            command.Connection = connection;

                            // Аннулирование товара
                            string sql = "UPDATE `PRODUCT` SET `IS_ANNULED`=\'0\' WHERE `PRODUCT`.`ID`=" + product.ID.ToString();

                            command.CommandText = sql;
                            command.ExecuteNonQuery();

                            // Запись в журнал событий
                            string act = JournalMethods.JournalItemHeader("отменил") + "аннулирование товара " + product.ProductCode;
                            sql = @"INSERT INTO `JOURNAL` (`DATE`, `ID_ACCOUNT`, `ACT`) VALUES ('" + JournalMethods.CurrentDate() + @"', '" +
                                ConnectSetting.User.Person.ID.ToString() + @"', '" + act + @"');";

                            command.CommandText = sql;
                            command.ExecuteNonQuery();
                        }

                        connection.Close();
                    }

                    LoadData();

                    Dialog.TransparentMessage(this, "Операция выполнена");*/
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
            AnnuledColumn.Visibility = Visibility.Collapsed;
            LoadData();
        }

        /// <summary>
        /// Показать аннулированные
        /// </summary>
        private void HideAnnuled_UnChecked(object sender, RoutedEventArgs e)
        {
            AnnuledColumn.Visibility = Visibility.Visible;
            LoadData();
        }

        /// <summary>
        /// Редактировать количество
        /// </summary>
        private void WarehouseEditCount_Execute(object sender, ExecutedRoutedEventArgs e)
        {
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

                    /*using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                    {
                        connection.Open();

                        string sql = String.Format("UPDATE `PRODUCT` SET `COUNT`=\'{0}\', `ID_UNIT`= \'{1}\' WHERE `PRODUCT`.`ID`={2}", product.Count.ToString().Replace(',','.'), product.ID_Unit, product.ID);

                        using (MySqlCommand command = new MySqlCommand(sql, connection))
                        {
                            command.ExecuteNonQuery();

                            string act = JournalMethods.JournalItemHeader("изменил") + "количество товара " + product.ProductCode;
                            sql = @"INSERT INTO `JOURNAL` (`DATE`, `ID_ACCOUNT`, `ACT`) VALUES ('" + JournalMethods.CurrentDate() + @"', '" +
                                ConnectSetting.User.Person.ID.ToString() + @"', '" + act + @"');";

                            command.CommandText = sql;
                            command.ExecuteNonQuery();
                        }

                        connection.Close();
                    }

                    Dialog.TransparentMessage(this, "Операция выполнена");*/
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

        /// <summary>
        /// Создать приходную накладную
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreatePurchaseInvoice_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            Invoice.CreateInvoiceWindow view = new Invoice.CreateInvoiceWindow(true);
            view.Owner = this;

            view.ShowDialog();
            LoadData();
        }
    }
}
