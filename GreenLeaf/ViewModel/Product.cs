using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MySql.Data.MySqlClient;
using GreenLeaf.Classes;
using System.Linq;
using System.Collections.Generic;

namespace GreenLeaf.ViewModel
{
    /// <summary>
    /// Товар
    /// </summary>
    public class Product : INotifyPropertyChanged
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
                if(_id != value)
                {
                    _id = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _productCode = string.Empty;
        /// <summary>
        /// Код товара
        /// </summary>
        public string ProductCode
        {
            get { return _productCode; }
            set
            {
                if(_productCode != value)
                {
                    _productCode = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _nomination = string.Empty;
        /// <summary>
        /// Наименование
        /// </summary>
        public string Nomination
        {
            get { return _nomination; }
            set
            {
                if(_nomination != value)
                {
                    _nomination = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _countInPackage = 0;
        /// <summary>
        /// Количество в упаковке
        /// </summary>
        public double CountInPackage
        {
            get { return _countInPackage; }
            set
            {
                if(_countInPackage != value)
                {
                    _countInPackage = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _cost = 0;
        /// <summary>
        /// Стоимость
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
                if(_coupon != value)
                {
                    _coupon = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _costPurchase = 0;
        /// <summary>
        /// Стоимость закупки
        /// </summary>
        public double CostPurchase
        {
            get { return _costPurchase; }
            set
            {
                if(_costPurchase != value)
                {
                    _costPurchase = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _couponPurchase = 0;
        /// <summary>
        /// Купон по закупке
        /// </summary>
        public double CouponPurchase
        {
            get { return _couponPurchase; }
            set
            {
                if (_couponPurchase != value)
                {
                    _couponPurchase = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _count = 0;
        /// <summary>
        /// Количество товара на складе
        /// </summary>
        public double Count
        {
            get { return _count; }
            set
            {
                if (_count != value)
                {
                    _count = value;
                    OnPropertyChanged();

                    Calc();
                }
            }
        }

        private double _locked_count = 0;
        /// <summary>
        /// Заблокированное количество
        /// </summary>
        public double LockedCount
        {
            get { return _locked_count; }
            set
            {
                if(_locked_count != value)
                {
                    _locked_count = value;
                    OnPropertyChanged();

                    Calc();
                }
            }
        }

        private double _allowedCount = 0;
        /// <summary>
        /// Доступное количество
        /// </summary>
        public double AllowedCount
        {
            get { return _allowedCount; }
        }

        private int _id_unit = 0;
        /// <summary>
        /// ID единицы измерения
        /// </summary>
        public int ID_Unit
        {
            get { return _id_unit; }
            set
            {
                if (_id_unit != value)
                {
                    _id_unit = value;
                    OnPropertyChanged();

                    if (MeasureUnit.Units.Keys.Contains(_id_unit))
                        _unitNomination = MeasureUnit.Units[_id_unit];
                    else
                        _unitNomination = string.Empty;
                    OnPropertyChanged("UnitNomination");
                }
            }
        }

        private string _unitNomination = string.Empty;
        /// <summary>
        /// Наименование единицы измерения
        /// </summary>
        public string UnitNomination
        {
            get { return _unitNomination; }
        }

        private bool _isAnnulated = false;
        /// <summary>
        /// Аннулировано
        /// </summary>
        public bool IsAnnulated
        {
            get { return _isAnnulated; }
            set
            {
                if(_isAnnulated != value)
                {
                    _isAnnulated = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Вычисление доступного количества
        /// </summary>
        private void Calc()
        {
            _allowedCount = _count - _locked_count;

            if (_allowedCount < 0)
                _allowedCount = 0;

            OnPropertyChanged("AllowedCount");
        }

        // Изменение свойств объекта
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        /// <summary>
        /// Создать товар
        /// </summary>
        /// <returns>возвращает TRUE если товар создан успешно</returns>
        public bool CreateProduct()
        {
            bool result = false;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                {
                    connection.Open();

                    string sql = String.Format(@"INSERT INTO `PRODUCT` (`PRODUCT_CODE`, `NOMINATION`, `COUNT_IN_PACKAGE`, `COST`, `COUPON`, `ID_UNIT`, `COST_PURCHASE`, `COUPON_PURCHASE`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}')", _productCode, _nomination, _countInPackage, Conversion.ToString(_cost), Conversion.ToString(_coupon), _id_unit, Conversion.ToString(_costPurchase), Conversion.ToString(_couponPurchase));

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();

                        ID = (int)command.LastInsertedId;
                    }

                    Journal.CreateJournal("создал", "товар " + ProductCode, connection);

                    connection.Close();
                }

                result = true;
            }
            catch (Exception ex)
            {
                Dialog.ErrorMessage(null, "Ошибка создания товара", ex.Message);
            }

            return result;
        }

        #region Получение данных

        /// <summary>
        /// Получить данные по ID
        /// <para>возвращает TRUE, если данные успешно получены</para>
        /// </summary>
        /// <returns></returns>
        public bool GetDataByID()
        {
            bool result = false;

            if(_id != 0)
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                    {
                        connection.Open();

                        string sql = "SELECT * FROM `PRODUCT` WHERE `PRODUCT`.`ID` = " + ID.ToString();

                        using (MySqlCommand command = new MySqlCommand(sql, connection))
                        {
                            using (MySqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    ProductCode = reader["PRODUCT_CODE"].ToString();
                                    Nomination = reader["NOMINATION"].ToString();

                                    CountInPackage = Conversion.ToDouble(reader["COUNT_IN_PACKAGE"].ToString());
                                    Cost = Conversion.ToDouble(reader["COST"].ToString());
                                    Coupon = Conversion.ToDouble(reader["COUPON"].ToString());
                                    CostPurchase = Conversion.ToDouble(reader["COST_PURCHASE"].ToString());
                                    CouponPurchase = Conversion.ToDouble(reader["COUPON_PURCHASE"].ToString());
                                    Count = Conversion.ToDouble(reader["COUNT"].ToString());
                                    LockedCount = Conversion.ToDouble(reader["LOCKED_COUNT"].ToString());
                                    ID_Unit = Conversion.ToInt(reader["ID_UNIT"].ToString());
                                    IsAnnulated = Conversion.ToBool(reader["IS_ANNULATED"].ToString());
                                }
                            }
                        }

                        connection.Close();
                    }

                    result = true;
                }
                catch(Exception ex)
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
        /// <returns></returns>
        public bool GetDataByID(int id)
        {
            _id = id;

            return GetDataByID();
        }

        #endregion

        #region Редактирование данных

        /// <summary>
        /// Редактирование товара
        /// </summary>
        /// <returns>возвращает TRUE если товар отредактирован успешно</returns>
        public bool EditProduct()
        {
            bool result = false;

            if (_id != 0)
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                    {
                        connection.Open();

                        string sql = String.Format(@"UPDATE `PRODUCT` SET `PRODUCT_CODE` = '{0}', `NOMINATION` = '{1}', `COUNT_IN_PACKAGE` = '{2}', `COST` = '{3}', `COUPON` = '{4}', `ID_UNIT` = '{5}', `COST_PURCHASE` = '{6}', `COUPON_PURCHASE` = '{7}' WHERE `PRODUCT`.`ID` = {8}", _productCode, _nomination, Conversion.ToString(_countInPackage), Conversion.ToString(_cost), Conversion.ToString(_coupon), _id_unit, _costPurchase, _couponPurchase, _id);

                        using (MySqlCommand command = new MySqlCommand(sql, connection))
                        {
                            command.ExecuteNonQuery();
                        }

                        Journal.CreateJournal("изменил", "товар " + ProductCode, connection);

                        connection.Close();
                    }

                    result = true;
                }
                catch (Exception ex)
                {
                    Dialog.ErrorMessage(null, "Ошибка редактирования товара", ex.Message);
                }
            }
            else
            {
                Dialog.ErrorMessage(null, "Не указан ID товара");
            }

            return result;
        }

        /// <summary>
        /// Аннулировать товар
        /// </summary>
        /// <returns>возвращает TRUE если товар аннулирован успешно</returns>
        public bool AnnulateProduct()
        {
            bool result = false;

            if (_id != 0)
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                    {
                        connection.Open();

                        IsAnnulated = true;

                        string sql = String.Format(@"UPDATE `PRODUCT` SET `IS_ANNULATED` = '1' WHERE `PRODUCT`.`ID` = {0}", _id);

                        using (MySqlCommand command = new MySqlCommand(sql, connection))
                        {
                            command.ExecuteNonQuery();
                        }

                        Journal.CreateJournal("аннулировал", "товар " + ProductCode, connection);

                        connection.Close();
                    }

                    result = true;
                }
                catch (Exception ex)
                {
                    Dialog.ErrorMessage(null, "Ошибка аннулирования товара", ex.Message);
                }
            }
            else
            {
                Dialog.ErrorMessage(null, "Не указан ID товара");
            }

            return result;
        }

        /// <summary>
        /// Отменить аннулирование товара
        /// </summary>
        /// <returns>возвращает TRUE если для товара аннулирование отменено успешно</returns>
        public bool UnAnnulateProduct()
        {
            bool result = false;

            if (_id != 0)
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                    {
                        connection.Open();

                        IsAnnulated = false;

                        string sql = String.Format(@"UPDATE `PRODUCT` SET `IS_ANNULATED` = '0' WHERE `PRODUCT`.`ID` = {0}", _id);

                        using (MySqlCommand command = new MySqlCommand(sql, connection))
                        {
                            command.ExecuteNonQuery();
                        }

                        Journal.CreateJournal("отменил", "аннулирование товара " + ProductCode, connection);

                        connection.Close();
                    }

                    result = true;
                }
                catch (Exception ex)
                {
                    Dialog.ErrorMessage(null, "Ошибка отмены аннулирования товара", ex.Message);
                }
            }
            else
            {
                Dialog.ErrorMessage(null, "Не указан ID товара");
            }

            return result;
        }

        /// <summary>
        /// Редактировать количество
        /// </summary>
        /// <returns>возвращает TRUE если количество отредактировано успешно</returns>
        public bool EditCount()
        {
            bool result = false;

            if (_id != 0)
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                    {
                        connection.Open();

                        string sql = String.Format(@"UPDATE `PRODUCT` SET `COUNT` = '{0}', `ID_UNIT` = '{1}' WHERE `PRODUCT`.`ID` = {2}", Conversion.ToString(_count), _id_unit, _id);

                        using (MySqlCommand command = new MySqlCommand(sql, connection))
                        {
                            command.ExecuteNonQuery();
                        }

                        Journal.CreateJournal("изменил", "количество товара " + ProductCode, connection);

                        connection.Close();
                    }

                    result = true;
                }
                catch (Exception ex)
                {
                    Dialog.ErrorMessage(null, "Ошибка изменения количества товара", ex.Message);
                }
            }
            else
            {
                Dialog.ErrorMessage(null, "Не указан ID товара");
            }

            return result;
        }

        /// <summary>
        /// Редактировать количество
        /// </summary>
        /// <param name="newCount">новое количество товара</param>
        /// <returns>возвращает TRUE если количество отредактировано успешно</returns>
        public bool EditCount(double newCount)
        {
            Count = newCount;

            return EditCount();
        }

        /// <summary>
        /// Редактировать количество
        /// </summary>
        /// <param name="newCount">новое количество товара</param>
        /// <param name="newIdUnit">новый ID единицы измерения</param>
        /// <returns>возвращает TRUE если количество отредактировано успешно</returns>
        public bool EditCount(double newCount, int newIdUnit)
        {
            Count = newCount;
            ID_Unit = newIdUnit;

            return EditCount();
        }

        #endregion

        #region Статические методы

        /// <summary>
        /// Создать товар
        /// </summary>
        /// <param name="productCode">код товара</param>
        /// <param name="nomination">наименование товара</param>
        /// <param name="countInPackage">количество в упаковке</param>
        /// <param name="cost">стоимость</param>
        /// <param name="coupon">купон</param>
        /// <param name="idUnit">ID единицы измерения</param>
        /// <returns></returns>
        public static Product CreateProduct(string productCode, string nomination, double countInPackage, double cost, double coupon, double costPurchase, double couponPurchase, int idUnit)
        {
            Product product = new Product();
            product.ProductCode = productCode;
            product.Nomination = nomination;
            product.CountInPackage = countInPackage;
            product.Cost = cost;
            product.Coupon = coupon;
            product.CostPurchase = costPurchase;
            product.CouponPurchase = couponPurchase;
            product.ID_Unit = idUnit;

            if (product.CreateProduct())
                return product;
            else
                return null;
        }

        /// <summary>
        /// Возвращает список не аннулированного товара
        /// </summary>
        public static List<Product> GetActualProductList()
        {
            List<Product> Products = new List<Product>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                {
                    connection.Open();

                    string sql = @"SELECT * FROM `PRODUCT` WHERE `PRODUCT`.`IS_ANNULATED` = '0'";
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Product item = new Product();

                                item.ID = Conversion.ToInt(reader["ID"].ToString());
                                item.ProductCode = reader["PRODUCT_CODE"].ToString();
                                item.Nomination = reader["NOMINATION"].ToString();
                                item.CountInPackage = Conversion.ToDouble(reader["COUNT_IN_PACKAGE"].ToString());
                                item.Cost = Conversion.ToDouble(reader["COST"].ToString());
                                item.Coupon = Conversion.ToDouble(reader["COUPON"].ToString());
                                item.CostPurchase = Conversion.ToDouble(reader["COST_PURCHASE"].ToString());
                                item.CouponPurchase = Conversion.ToDouble(reader["COUPON_PURCHASE"].ToString());
                                item.Count = Conversion.ToDouble(reader["COUNT"].ToString());
                                item.LockedCount = Conversion.ToDouble(reader["LOCKED_COUNT"].ToString());
                                item.ID_Unit = Conversion.ToInt(reader["ID_UNIT"].ToString());
                                item.IsAnnulated = false;

                                Products.Add(item);
                            }
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Dialog.ErrorMessage(null, "Ошибка получения данных", ex.Message);
            }

            return Products.OrderBy(p => p.ProductCode).ToList();
        }

        /// <summary>
        /// Возвращает список товаров, включая аннулированные
        /// </summary>
        public static List<Product> GetAllProductList()
        {
            List<Product> Products = new List<Product>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                {
                    connection.Open();

                    string sql = "SELECT * FROM `PRODUCT`";
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Product item = new Product();

                                item.ID = Conversion.ToInt(reader["ID"].ToString());
                                item.ProductCode = reader["PRODUCT_CODE"].ToString();
                                item.Nomination = reader["NOMINATION"].ToString();
                                item.CountInPackage = Conversion.ToDouble(reader["COUNT_IN_PACKAGE"].ToString());
                                item.Cost = Conversion.ToDouble(reader["COST"].ToString());
                                item.Coupon = Conversion.ToDouble(reader["COUPON"].ToString());
                                item.CostPurchase = Conversion.ToDouble(reader["COST_PURCHASE"].ToString());
                                item.CouponPurchase = Conversion.ToDouble(reader["COUPON_PURCHASE"].ToString());
                                item.Count = Conversion.ToDouble(reader["COUNT"].ToString());
                                item.LockedCount = Conversion.ToDouble(reader["LOCKED_COUNT"].ToString());
                                item.ID_Unit = Conversion.ToInt(reader["ID_UNIT"].ToString());
                                item.IsAnnulated = Conversion.ToBool(reader["IS_ANNULATED"].ToString());

                                Products.Add(item);
                            }
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Dialog.ErrorMessage(null, "Ошибка получения списка товара", ex.Message);
            }

            return Products.OrderBy(p => p.ProductCode).ToList();
        }

        /// <summary>
        /// Возвращает список товаров, согласно параметрам поиска
        /// </summary>
        /// <param name="hideAnnulated">скрыть аннулированные</param>
        /// <param name="hideEmpty">скрыть пустой товар</param>
        /// <param name="productCode">код товара</param>
        /// <param name="nomination">наименование товара</param>
        public static List<Product> GetProductListByParameters(bool hideAnnulated = true, bool hideEmpty = true, string productCode = "", string nomination = "")
        {
            List<Product> Products = new List<Product>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                {
                    connection.Open();

                    string sql = "SELECT * FROM `PRODUCT`";

                    bool whereAdded = false;

                    if(hideAnnulated)
                    {
                        sql += @" WHERE `PRODUCT`.`IS_ANNULATED` = '0'";
                        whereAdded = true;
                    }

                    if(hideEmpty)
                    {
                        if(whereAdded)
                        {
                            sql += " AND ";
                        }
                        else
                        {
                            sql += " WHERE ";
                            whereAdded = true;
                        }

                        sql += "`PRODUCT`.`COUNT` > 0";
                    }

                    if(productCode != "")
                    {
                        if (whereAdded)
                        {
                            sql += " AND ";
                        }
                        else
                        {
                            sql += " WHERE ";
                            whereAdded = true;
                        }

                        sql += @"`PRODUCT`.`PRODUCT_CODE` LIKE '%" + productCode + @"%'";
                    }

                    if (nomination != "")
                    {
                        if (whereAdded)
                        {
                            sql += " AND ";
                        }
                        else
                        {
                            sql += " WHERE ";
                            whereAdded = true;
                        }

                        sql += @"`PRODUCT`.`NOMINATION` LIKE '%" + nomination + @"%'";
                    }

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Product item = new Product();

                                item.ID = Conversion.ToInt(reader["ID"].ToString());
                                item.ProductCode = reader["PRODUCT_CODE"].ToString();
                                item.Nomination = reader["NOMINATION"].ToString();
                                item.CountInPackage = Conversion.ToDouble(reader["COUNT_IN_PACKAGE"].ToString());
                                item.Cost = Conversion.ToDouble(reader["COST"].ToString());
                                item.Coupon = Conversion.ToDouble(reader["COUPON"].ToString());
                                item.CostPurchase = Conversion.ToDouble(reader["COST_PURCHASE"].ToString());
                                item.CouponPurchase = Conversion.ToDouble(reader["COUPON_PURCHASE"].ToString());
                                item.Count = Conversion.ToDouble(reader["COUNT"].ToString());
                                item.LockedCount = Conversion.ToDouble(reader["LOCKED_COUNT"].ToString());
                                item.ID_Unit = Conversion.ToInt(reader["ID_UNIT"].ToString());
                                item.IsAnnulated = Conversion.ToBool(reader["IS_ANNULATED"].ToString());

                                Products.Add(item);
                            }
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Dialog.ErrorMessage(null, "Ошибка получения списка товара", ex.Message);
            }

            return Products.OrderBy(p => p.ProductCode).ToList();
        }

        #endregion
    }
}