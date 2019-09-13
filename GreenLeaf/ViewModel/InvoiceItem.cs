﻿using System;
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
        }

        private double _coupon = 0;
        /// <summary>
        /// Купон
        /// </summary>
        public double Coupon
        {
            get { return _coupon; }
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
                                    ID_Product = tempI;
                                else
                                    ID_Product = 0;

                                tempS = reader["COUNT"].ToString();
                                double tempD = 0;
                                if (double.TryParse(tempS, out tempD))
                                    Сount = tempD;
                                else
                                    Count = 0;

                                tempS = reader["COST"].ToString();
                                tempD = 0;
                                if (double.TryParse(tempS, out tempD))
                                    _сost = tempD;
                                else
                                    _сost = 0;

                                tempS = reader["COUPON"].ToString();
                                tempD = 0;
                                if (double.TryParse(tempS, out tempD))
                                    _сoupon= tempD;
                                else
                                    _сoupon = 0;

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

                        string table = (isPurchase) ? "PURCHASE_INVOICE" : "SALES_INVOICE";

                        string sql = String.Format(@"INSERT INTO {0} (`ID_INVOICE`, `ID_PRODUCT`, `COUNT`, `COST`, `COUPON`) VALUES ('{1}', '{2}', '{3}', '{4}', '{5}')", table, _id_invoice, _id_product, _count, _cost, _product);

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

                        string table = (isPurchase) ? "PURCHASE_INVOICE" : "SALES_INVOICE";

                        string sql = String.Format(@"DELETE FROM {} WHERE ID = {}", table, _id);

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



        #region Статические методы

        /// <summary>
        /// Возвращает список элементов накладной
        /// </summary>
        /// <param name="id_invoice">ID накладной</param>
        /// <param name="isPurchase">TRUE если приходная накладная, FALSE если расходная</param>
        /// <returns></returns>
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
                    MySqlCommand command = new MySqlCommand(sql, connection);

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
