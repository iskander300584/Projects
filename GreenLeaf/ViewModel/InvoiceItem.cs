using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MySql.Data.MySqlClient;
using GreenLeaf.Classes;

namespace GreenLeaf.ViewModel
{
    /// <summary>
    /// Элемент накладной
    /// </summary>
    public class InvoiceItem : INotifyPropertyChanged
    {
        private int _id = 0;
        /// <summary>
        /// ID
        /// </summary>
        public int ID
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _id_invoice = 0;
        /// <summary>
        /// ID накладной
        /// </summary>
        public int ID_Invoice
        {
            get { return _id_invoice; }
            set
            {
                if (_id_invoice != value)
                {
                    _id_invoice = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _id_product = 0;
        /// <summary>
        /// ID товара
        /// </summary>
        public int ID_Product
        {
            get { return _id_product; }
            set
            {
                if (_id_product != value)
                {
                    _id_product = value;
                    OnPropertyChanged();

                    if (_id_product != 0)
                    {
                        _product = new Product();
                        _product.GetDataByID(_id_product);
                    }
                    else
                        _product = null;

                    OnPropertyChanged("Product");

                    Calc();
                }
            }
        }

        private Product _product = null;
        /// <summary>
        /// Товар
        /// </summary>
        public Product Product
        {
            get { return _product; }
        }

        private double _count = 0;
        /// <summary>
        /// Количество товара
        /// </summary>
        public double Count
        {
            get { return _count; }
            set
            {
                if(_count != value)
                {
                    _count = value;
                    OnPropertyChanged();

                    Calc();
                }
            }
        }

        private double _cost = 0;
        /// <summary>
        /// Стоимость товара
        /// </summary>
        public double Cost
        {
            get { return _cost; }
            set
            {
                if(_cost != value)
                {
                    _cost = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _coupon = 0;
        /// <summary>
        /// Купон
        /// </summary>
        public double Coupon
        {
            get { return _coupon; }
            set
            {
                if (_coupon != value)
                {
                    _coupon = value;
                    OnPropertyChanged();
                }
            }
        }

        // Изменение свойств объекта
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        /// <summary>
        /// Вычисление стоимости и купона
        /// </summary>
        private void Calc()
        {
            if(Product != null)
            {
                _cost = Product.Cost * Count;
                OnPropertyChanged("Cost");

                _coupon = Product.Coupon * Count;
                OnPropertyChanged("Coupon");
            }
        }

        #region Получение данных

        /// <summary>
        /// Получить данные по ID
        /// <para>возвращает TRUE, если данные успешно получены</para>
        /// </summary
        /// <param name="isPurchase">TRUE если элемент приходной накладной, FALSE если расходной</param>
        /// <returns>возвращает TRUE, если данные успешно получены</returns>
        public bool GetDataByID(bool isPurchase)
        {
            bool result = false;

            if (_id != 0)
            {
                try
                {
                    bool getData = false;

                    using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                    {
                        connection.Open();

                        string table = (isPurchase) ? "PURCHASE_INVOICE_UNIT" : "SALES_INVOICE_UNIT";

                        string sql = "SELECT * FROM " + table + " WHERE ID=" + ID.ToString();
                        MySqlCommand command = new MySqlCommand(sql, connection);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string tempS = reader["ID_INVOICE"].ToString();
                                int tempI = 0;
                                if (int.TryParse(tempS, out tempI))
                                    ID_Invoice = tempI;
                                else
                                    ID_Invoice = 0;

                                tempS = reader["ID_PRODUCT"].ToString();
                                tempI = 0;
                                if (int.TryParse(tempS, out tempI))
                                    _id_product = tempI;
                                else
                                    _id_product = 0;

                                tempS = reader["COUNT"].ToString();
                                double tempD = 0;
                                if (double.TryParse(tempS, out tempD))
                                    Count = tempD;
                                else
                                    Count = 0;

                                tempS = reader["COST"].ToString();
                                tempD = 0;
                                if (double.TryParse(tempS, out tempD))
                                    _cost = tempD;
                                else
                                    _cost = 0;

                                tempS = reader["COUPON"].ToString();
                                tempD = 0;
                                if (double.TryParse(tempS, out tempD))
                                    _coupon = tempD;
                                else
                                    _coupon = 0;

                                getData = true;
                            }
                        }

                        connection.Close();
                    }

                    result = getData;
                }
                catch (Exception ex)
                {
                    Dialog.ErrorMessage(null, "Ошибка получения данных", ex.Message);
                }
            }

            return result;
        }

        /// <summary>
        /// Получить данные по ID
        /// <para>возвращает TRUE, если данные успешно получены</para>
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="isPurchase">TRUE если элемент приходной накладной, FALSE если расходной</param>
        /// <returns>возвращает TRUE, если данные успешно получены</returns>
        public bool GetDateByID(int id, bool isPurchase)
        {
            _id = id;

            return GetDataByID(isPurchase);
        }

        #endregion

        /// <summary>
        /// Создание элемента накладной
        /// </summary>
        /// <param name="isPurchase">приходная накладная</param>
        /// <returns>возвращает TRUE, если элемент накладной создан успешно</returns>
        public bool CreateItem(bool isPurchase)
        {
            bool result = false;

            if(_id_invoice != 0)
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                    {
                        connection.Open();

                        string table = (isPurchase) ? "PURCHASE_INVOICE_UNIT" : "SALES_INVOICE_UNIT";

                        string sql = String.Format(@"INSERT INTO {0} (`ID_INVOICE`, `ID_PRODUCT`, `COUNT`, `COST`, `COUPON`) VALUES ('{1}', '{2}', '{3}', '{4}', '{5}')", table, _id_invoice, _id_product, _count, _cost, _coupon);

                        using (MySqlCommand command = new MySqlCommand(sql, connection))
                        {
                            ID = command.ExecuteNonQuery();
                        }

                        connection.Close();

                        result = true;
                    }
                }
                catch(Exception ex)
                {
                    Dialog.ErrorMessage(null, "Ошибка создания позиции накладной", ex.Message);
                }
            }
            else
            {
                Dialog.ErrorMessage(null, "Не указан ID накладной");
            }

            return result;
        }

        /// <summary>
        /// Удаление элемента накладной
        /// </summary>
        /// <param name="isPurchase">приходная накладная</param>
        /// <returns>возвращает TRUE, если элемент накладной удален успешно</returns>
        public bool DeleteItem(bool isPurchase)
        {
            bool result = false;

            if(_id != 0)
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                    {
                        connection.Open();

                        string table = (isPurchase) ? "PURCHASE_INVOICE_UNIT" : "SALES_INVOICE_UNIT";

                        string sql = String.Format(@"DELETE FROM {0} WHERE ID = {1}", table, _id);

                        using (MySqlCommand command = new MySqlCommand(sql, connection))
                        {
                            command.ExecuteNonQuery();
                        }

                        connection.Close();

                        result = true;
                    }
                }
                catch(Exception ex)
                {
                    Dialog.ErrorMessage(null, "Ошибка удаления позиции накладной", ex.Message);
                }
            }
            else
            {
                Dialog.ErrorMessage(null, "Не указан ID позиции");
            }

            return result;
        }

        /// <summary>
        /// Редактирование элемента накладной
        /// </summary>
        /// <param name="isPurchase">приходная накладная</param>
        /// <returns>возвращает TRUE, если элемент накладной отредактирован успешно</returns>
        public bool EditItem(bool isPurchase)
        {
            bool result = false;

            if (_id_invoice != 0)
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                    {
                        connection.Open();

                        string table = (isPurchase) ? "PURCHASE_INVOICE_UNIT" : "SALES_INVOICE_UNIT";

                        string sql = String.Format(@"UPDATE {0} SET `ID_PRODUCT` = '{1}', `COUNT` = '{2}', `COST` = '{3}', `COUPON` = '{4}' WHERE ID = {5}", table, _id_product, _count, _cost, _coupon, _id);

                        using (MySqlCommand command = new MySqlCommand(sql, connection))
                        {
                            command.ExecuteNonQuery();
                        }

                        connection.Close();

                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    Dialog.ErrorMessage(null, "Ошибка редактирования позиции накладной", ex.Message);
                }
            }
            else
            {
                Dialog.ErrorMessage(null, "Не указан ID накладной");
            }

            return result;
        }

        /// <summary>
        /// Редактирование элемента накладной
        /// </summary>
        /// <param name="id_product">ID товара</param>
        /// <param name="count">количество товара</param>
        /// <param name="isPurchase">приходная накладная</param>
        /// <returns>возвращает TRUE, если элемент накладной отредактирован успешно</returns>
        public bool EditItem(int id_product, double count, bool isPurchase)
        {
            ID_Product = id_product;
            Count = count;

            return EditItem(isPurchase);
        }

        #region Статические методы

        /// <summary>
        /// Создать элемент накладной
        /// </summary>
        /// <param name="id_invoice">ID накладной</param>
        /// <param name="id_product">ID товара</param>
        /// <param name="count">количество товара</param>
        /// <param name="isPurshase">элемент приходной накладной</param>
        /// <returns>возвращает элемент накладной</returns>
        public static InvoiceItem CreateItem(int id_invoice, int id_product, double count, bool isPurshase)
        {
            InvoiceItem item = null;
            
            try
            {
                item = new InvoiceItem();

                item.ID_Invoice = id_invoice;
                item.ID_Product = id_product;
                item.Count = count;

                item.CreateItem(isPurshase);
            }
            catch (Exception ex)
            {
                item = null;
                Dialog.ErrorMessage(null, "Ошибка создания позиции накладной", ex.Message);
            }

            return item;
        }

        /// <summary>
        /// Возвращает список элементов накладной
        /// </summary>
        /// <param name="id_invoice">ID накладной</param>
        /// <param name="isPurchase">TRUE если приходная накладная, FALSE если расходная</param>
        public static List<InvoiceItem> GetInvoiceItemList(int id_invoice, bool isPurchase)
        {
            List<InvoiceItem> Items = new List<InvoiceItem>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                {
                    connection.Open();

                    string table = (isPurchase) ? "PURCHASE_INVOICE_UNIT" : "SALES_INVOICE_UNIT";

                    string sql = "SELECT * FROM " + table + " WHERE ID_INVOICE=" + id_invoice.ToString();
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                InvoiceItem item = new InvoiceItem();
                                item.ID_Invoice = id_invoice;

                                string tempS = reader["ID"].ToString();
                                int tempI = 0;
                                if (int.TryParse(tempS, out tempI))
                                    item.ID = tempI;
                                else
                                    item.ID = 0;

                                tempS = reader["ID_PRODUCT"].ToString();
                                tempI = 0;
                                if (int.TryParse(tempS, out tempI))
                                    item.ID_Product = tempI;
                                else
                                    item.ID_Product = 0;

                                tempS = reader["COUNT"].ToString();
                                double tempD = 0;
                                if (double.TryParse(tempS, out tempD))
                                    item.Count = tempD;
                                else
                                    item.Count = 0;

                                tempS = reader["COST"].ToString();
                                tempD = 0;
                                if (double.TryParse(tempS, out tempD))
                                    item.Cost = tempD;
                                else
                                    item.Cost = 0;

                                tempS = reader["COUPON"].ToString();
                                tempD = 0;
                                if (double.TryParse(tempS, out tempD))
                                    item.Coupon = tempD;
                                else
                                    item.Coupon = 0;

                                Items.Add(item);
                            }
                        }
                    }

                    connection.Close();
                }
            }
            catch(Exception ex)
            {
                Dialog.ErrorMessage(null, "Ошибка получения данных", ex.Message);
            }

            return Items;
        }

        #endregion
    }
}
