using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GreenLeaf.ViewModel;
using GreenLeaf.Classes;
using MySql.Data.MySqlClient;

namespace GreenLeaf.Windows.Warehouse
{
    /// <summary>
    /// Логика взаимодействия для SelectProductWindow.xaml
    /// </summary>
    public partial class SelectProductWindow : Window
    {
        #region Объявление команд

        public static RoutedUICommand Search = new RoutedUICommand("Поиск", "Search", typeof(SelectProductWindow));

        public static RoutedUICommand ResetSearch = new RoutedUICommand("Сбросить фильтры", "ResetSearch", typeof(SelectProductWindow));

        #endregion

        /// <summary>
        /// Список отображаемых товаров
        /// </summary>
        private List<Product> products = new List<Product>();

        /// <summary>
        /// Возможность множественного выбора
        /// </summary>
        private bool Multiselect = false;

        /// <summary>
        /// Отображать аннулированные
        /// </summary>
        private bool ShowAnnuled = false;

        /// <summary>
        /// Выбранные товары
        /// </summary>
        public List<Product> SelectedProducts = new List<Product>();

        /// <summary>
        /// Окно выбора товара
        /// </summary>
        /// <param name="multiselect">возможность множественного выбора</param>
        /// <param name="showAnnuled">отображать аннулированные</param>
        public SelectProductWindow(bool multiselect = false, bool showAnnuled = false)
        {
            InitializeComponent();

            Multiselect = multiselect;
            ShowAnnuled = showAnnuled;

            AnnuledColumn.Visibility = (ShowAnnuled) ? Visibility.Visible : Visibility.Collapsed;

            if (Multiselect)
                prodGrid.SelectionMode = DataGridSelectionMode.Extended;

            if(ResetFilter())
            {
                prodGrid.ItemsSource = products;
                prodGrid.Items.Refresh();
            }
            else
                this.DialogResult = false;
        }

        /// <summary>
        /// Сброс фильтров и поиск всех товаров
        /// </summary>
        /// <returns></returns>
        private bool ResetFilter()
        {
            tbCodeSearch.Text = "";
            tbNominationSearch.Text = "";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                {
                    connection.Open();

                    string sql = @"SELECT `ID`, `PRODUCT_CODE`, `NOMINATION`, `IS_ANNULED` FROM PRODUCT";
                    if (!ShowAnnuled)
                        sql += " WHERE `PRODUCT`.`IS_ANNULED`='0'";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        MySqlDataReader reader = command.ExecuteReader();

                        products = new List<Product>();

                        while (reader.Read())
                        {
                            Product product = new Product();
                            product.ID = int.Parse(reader["ID"].ToString());
                            product.ProductCode = reader["PRODUCT_CODE"].ToString();
                            product.Nomination = reader["NOMINATION"].ToString();
                            product.IsAnnuled = bool.Parse(reader["IS_ANNULED"].ToString());

                            products.Add(product);
                        }
                    }

                    connection.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                Dialog.ErrorMessage(this.Owner, "Ошибка получения списка товаров", ex.Message);

                return false;
            }
        }

        /// <summary>
        /// Поиск по фильтру
        /// </summary>
        /// <returns></returns>
        private bool SearchByFilter()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                {
                    connection.Open();

                    string sql = @"SELECT `ID`, `PRODUCT_CODE`, `NOMINATION`, `IS_ANNULED` FROM PRODUCT WHERE ";

                    string code = tbCodeSearch.Text.Trim();
                    string nomination = tbNominationSearch.Text.Trim();

                    if (code != "" && nomination != "")
                        sql += "`PRODUCT`.`PRODUCT_CODE` LIKE \'%" + code + "%\' AND `PRODUCT`.`NOMINATION` LIKE \'%" + nomination + "%\'";
                    else if(code != "")
                        sql += "`PRODUCT`.`PRODUCT_CODE` LIKE \'%" + code + "%\'";
                    else
                        sql += "`PRODUCT`.`NOMINATION` LIKE \'%" + nomination + "%\'";

                    if (!ShowAnnuled)
                        sql += " AND `PRODUCT`.`IS_ANNULED`='0'";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        MySqlDataReader reader = command.ExecuteReader();

                        products = new List<Product>();

                        while (reader.Read())
                        {
                            Product product = new Product();
                            product.ID = int.Parse(reader["ID"].ToString());
                            product.ProductCode = reader["PRODUCT_CODE"].ToString();
                            product.Nomination = reader["NOMINATION"].ToString();
                            product.IsAnnuled = bool.Parse(reader["IS_ANNULED"].ToString());

                            products.Add(product);
                        }
                    }

                    connection.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                Dialog.ErrorMessage(this.Owner, "Ошибка получения списка товаров", ex.Message);

                return false;
            }
        }

        /// <summary>
        /// Нажатие кнопки Выбрать
        /// </summary>
        private void BtnSelect_Click(object sender, RoutedEventArgs e)
        {
            if(prodGrid.SelectedItems == null || (prodGrid.SelectedItems != null && prodGrid.SelectedItems.Count == 0))
            {
                Dialog.WarningMessage(this, "Не выбран товар");
                return;
            }

            foreach (Product pr in prodGrid.SelectedItems)
                SelectedProducts.Add(pr);

            this.DialogResult = true;
        }

        /// <summary>
        /// Проверка возможности выполнения поиска
        /// </summary>
        private void Search_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (products != null && (tbCodeSearch.Text.Trim() != "" || tbNominationSearch.Text.Trim() != ""))
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }

        /// <summary>
        /// Поиск по фильтру
        /// </summary>
        private void Search_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            prodGrid.ItemsSource = null;
            SearchByFilter();
            prodGrid.ItemsSource = products;
        }

        /// <summary>
        /// Проверка возможности сброса фильтров поиска
        /// </summary>
        private void ResetSearch_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (products != null && (tbCodeSearch.Text.Trim() != "" || tbNominationSearch.Text.Trim() != ""))
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }

        /// <summary>
        /// Сброс условий поиска
        /// </summary>
        private void ResetSearch_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            prodGrid.ItemsSource = null;
            ResetFilter();
            prodGrid.ItemsSource = products;
        }
    }
}
