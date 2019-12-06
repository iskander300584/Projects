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

        private double _productCost = 0;
        /// <summary>
        /// Стоимость единицы товара
        /// </summary>
        public double ProductCost
        {
            get { return _productCost; }
            set
            {
                if(_productCost != value)
                {
                    _productCost = value;
                    OnPropertyChanged();

                    Calc();
                }
            }
        }

        private double _productCoupon = 0;
        /// <summary>
        /// Купон единицы товара
        /// </summary>
        public double ProductCoupon
        {
            get { return _productCoupon; }
            set
            {
                if(_productCoupon != value)
                {
                    _productCoupon = value;
                    OnPropertyChanged();

                    Calc();
                }
            }
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
        /// Создание элемента накладной
        /// </summary>
        /// <param name="isPurchase">приходная накладная</param>
        /// <returns>возвращает TRUE, если элемент накладной создан успешно</returns>
        public bool CreateItem(bool isPurchase)
        {
            bool result = false;

            if (_id_invoice != 0)
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ProgramSettings.ConnectionString)))
                    {
                        connection.Open();

                        string table = (isPurchase) ? "PURCHASE_INVOICE_UNIT" : "SALES_INVOICE_UNIT";

                        string sql = String.Format(@"INSERT INTO `{0}` (`ID_INVOICE`, `ID_PRODUCT`, `COUNT`, `COST`, `COUPON`, `PRODUCT_COST`, `PRODUCT_COUPON`) VALUES ('{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}')", table, _id_invoice, _id_product, Conversion.ToString(_count), Conversion.ToString(_cost), Conversion.ToString(_coupon), Conversion.ToString(_productCost), Conversion.ToString(_productCoupon));

                        using (MySqlCommand command = new MySqlCommand(sql, connection))
                        {
                            command.ExecuteNonQuery();

                            ID = (int)command.LastInsertedId;
                        }

                        connection.Close();

                        result = true;
                    }
                }
                catch (Exception ex)
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
                    using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ProgramSettings.ConnectionString)))
                    {
                        connection.Open();

                        string table = (isPurchase) ? "PURCHASE_INVOICE_UNIT" : "SALES_INVOICE_UNIT";

                        string sql = String.Format(@"SELECT * FROM `{0}` WHERE `{0}`.`ID` = {1}", table, ID);

                        using (MySqlCommand command = new MySqlCommand(sql, connection))
                        {
                            using (MySqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    ID_Invoice = Conversion.ToInt(reader["ID_INVOICE"].ToString());
                                    ID_Product = Conversion.ToInt(reader["ID_PRODUCT"].ToString());
                                    Count = Conversion.ToDouble(reader["COUNT"].ToString());
                                    Cost = Conversion.ToDouble(reader["COST"].ToString());
                                    Coupon = Conversion.ToDouble(reader["COUPON"].ToString());
                                    ProductCost = Conversion.ToDouble(reader["PRODUCT_COST"].ToString());
                                    ProductCoupon = Conversion.ToDouble(reader["PRODUCT_COUPON"].ToString());
                                }
                            }
                        }

                        connection.Close();
                    }

                    result = true;
                }
                catch (Exception ex)
                {
                    Dialog.ErrorMessage(null, "Ошибка получения данных элемента накладной", ex.Message);
                }
            }
            else
            {
                Dialog.ErrorMessage(null, "Не указан ID элемента накладной");
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

        #region Редактирование данных

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
                    using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ProgramSettings.ConnectionString)))
                    {
                        connection.Open();

                        string table = (isPurchase) ? "PURCHASE_INVOICE_UNIT" : "SALES_INVOICE_UNIT";

                        string sql = String.Format(@"DELETE FROM `{0}` WHERE `{0}`.`ID` = {1}", table, _id);

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
                    Dialog.ErrorMessage(null, "Ошибка удаления элемента накладной", ex.Message);
                }
            }
            else
            {
                Dialog.ErrorMessage(null, "Не указан ID элемента накладной");
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

            if (_id != 0)
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ProgramSettings.ConnectionString)))
                    {
                        connection.Open();

                        string table = (isPurchase) ? "PURCHASE_INVOICE_UNIT" : "SALES_INVOICE_UNIT";

                        string sql = String.Format(@"UPDATE `{0}` SET `ID_PRODUCT` = '{1}', `COUNT` = '{2}', `COST` = '{3}', `COUPON` = '{4}', `PRODUCT_COST` = '{5}', `PRODUCT_COUPON` = '{6}' WHERE `{0}`.`ID` = {7}", table, _id_product, Conversion.ToString(_count), Conversion.ToString(_cost), Conversion.ToString(_coupon), Conversion.ToString(_productCost), Conversion.ToString(_productCoupon), _id);

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
                    Dialog.ErrorMessage(null, "Ошибка редактирования элемента накладной", ex.Message);
                }
            }
            else
            {
                Dialog.ErrorMessage(null, "Не указан ID элемента накладной");
            }

            return result;
        }

        /// <summary>
        /// Редактирование элемента накладной
        /// </summary>
        /// <param name="count">количество товара</param>
        /// <param name="isPurchase">приходная накладная</param>
        /// <returns>возвращает TRUE, если элемент накладной отредактирован успешно</returns>
        public bool EditItem(double count, bool isPurchase)
        {
            Count = count;

            return EditItem(isPurchase);
        }

        /// <summary>
        /// Редактирование элемента накладной
        /// </summary>
        /// <param name="id_product">ID товара</param>
        /// <param name="count">количество товара</param>
        /// <param name="productCost">стоимость единицы товара</param>
        /// <param name="productCoupon">купон на единицу товара</param>
        /// <param name="isPurchase">приходная накладная</param>
        /// <returns>возвращает TRUE, если элемент накладной отредактирован успешно</returns>
        public bool EditItem(int id_product, double count, double productCost, double productCoupon, bool isPurchase)
        {
            ID_Product = id_product;
            ProductCost = productCost;
            ProductCoupon = productCoupon;
            Count = count;

            return EditItem(isPurchase);
        }

        /// <summary>
        /// Вычисление стоимости и купона
        /// </summary>
        private void Calc()
        {
            Cost = ProductCost * Count;
            Coupon = ProductCoupon * Count;
        }

        #endregion

        #region Статические методы

        /// <summary>
        /// Создать элемент накладной
        /// </summary>
        /// <param name="id_invoice">ID накладной</param>
        /// <param name="id_product">ID товара</param>
        /// <param name="productCost">стоимость единицы товара</param>
        /// <param name="productCoupon">купон на единицу товара</param>
        /// <param name="count">количество товара</param>
        /// <param name="isPurshase">элемент приходной накладной</param>
        /// <returns>возвращает элемент накладной</returns>
        public static InvoiceItem CreateItem(int id_invoice, int id_product, double productCost, double productCoupon, double count, bool isPurshase)
        {
            InvoiceItem item = null;
            
            try
            {
                item = new InvoiceItem();

                item.ID_Invoice = id_invoice;
                item.ID_Product = id_product;
                item.ProductCost = productCost;
                item.ProductCoupon = productCoupon;
                item.Count = count;

                item.CreateItem(isPurshase);
            }
            catch (Exception ex)
            {
                item = null;
                Dialog.ErrorMessage(null, "Ошибка создания элемента накладной", ex.Message);
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
                using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ProgramSettings.ConnectionString)))
                {
                    connection.Open();

                    string table = (isPurchase) ? "PURCHASE_INVOICE_UNIT" : "SALES_INVOICE_UNIT";

                    string sql = String.Format(@"SELECT * FROM `{0}` WHERE `{0}`.`ID_INVOICE` = {1}", table, id_invoice);

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                InvoiceItem item = new InvoiceItem();
                                item.ID_Invoice = id_invoice;

                                item.ID = Conversion.ToInt(reader["ID"].ToString());
                                item.ID_Product = Conversion.ToInt(reader["ID_PRODUCT"].ToString());
                                item.Count = Conversion.ToDouble(reader["COUNT"].ToString());
                                item.ProductCost = Conversion.ToDouble(reader["PRDUCT_COST"].ToString());
                                item.ProductCoupon = Conversion.ToDouble(reader["PRODUCT_COUPON"].ToString());
                                item.Cost = Conversion.ToDouble(reader["COST"].ToString());
                                item.Coupon = Conversion.ToDouble(reader["COUPON"].ToString());
                                Items.Add(item);
                            }
                        }
                    }

                    connection.Close();
                }
            }
            catch(Exception ex)
            {
                Dialog.ErrorMessage(null, "Ошибка получения списка элементов накладной", ex.Message);
            }

            return Items;
        }

        /// <summary>
        /// Возвращает список элементов накладной
        /// </summary>
        /// <param name="id_invoice">ID накладной</param>
        /// <param name="isPurchase">TRUE если приходная накладная, FALSE если расходная</param>
        /// <param name="connection">соединение с БД</param>
        public static List<InvoiceItem> GetInvoiceItemList(int id_invoice, bool isPurchase, MySqlConnection connection)
        {
            List<InvoiceItem> Items = new List<InvoiceItem>();

            try
            {
                string table = (isPurchase) ? "PURCHASE_INVOICE_UNIT" : "SALES_INVOICE_UNIT";

                string sql = String.Format(@"SELECT * FROM `{0}` WHERE `{0}`.`ID_INVOICE` = {1}", table, id_invoice);

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            InvoiceItem item = new InvoiceItem();
                            item.ID_Invoice = id_invoice;

                            item.ID = Conversion.ToInt(reader["ID"].ToString());
                            item.ID_Product = Conversion.ToInt(reader["ID_PRODUCT"].ToString());
                            item.ProductCost = Conversion.ToDouble(reader["PRODUCT_COST"].ToString());
                            item.ProductCoupon = Conversion.ToDouble(reader["PRODUCT_COUPON"].ToString());
                            item.Count = Conversion.ToDouble(reader["COUNT"].ToString());
                            item.Cost = Conversion.ToDouble(reader["COST"].ToString());
                            item.Coupon = Conversion.ToDouble(reader["COUPON"].ToString());

                            Items.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Dialog.ErrorMessage(null, "Ошибка получения списка элементов накладной", ex.Message);
            }

            return Items;
        }

        /// <summary>
        /// Удаление элемента накладной
        /// </summary>
        /// <param name="id">ID элемента накладной</param>
        /// <param name="isPurchase"></param>
        /// <returns></returns>
        public static bool DeleteItem(int id, bool isPurchase)
        {
            bool result = false;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ProgramSettings.ConnectionString)))
                {
                    connection.Open();

                    string table = (isPurchase) ? "PURCHASE_INVOICE_UNIT" : "SALES_INVOICE_UNIT";

                    string sql = String.Format(@"DELETE FROM `{0}` WHERE`{0}`.`ID` = {1}", table, id);

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
                Dialog.ErrorMessage(null, "Ошибка удаления позиции накладной", ex.Message);
            }

            return result;
        }

        #endregion
    }
}