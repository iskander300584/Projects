﻿using System;
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

                    string sql = String.Format(@"INSERT INTO `PRODUCT` (`PRODUCT_CODE`, `NOMINATION`, `COUNT_IN_PACKAGE`, `COST`, `COUPON`, `ID_UNIT`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}')", _productCode, _nomination, _countInPackage, _cost, _coupon, _id_unit);
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        ID = command.ExecuteNonQuery();
                    }

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
                    bool getData = false;

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

                                    string tempS = reader["COUNT_IN_PACKAGE"].ToString();
                                    double tempD = 0;
                                    if (double.TryParse(tempS, out tempD))
                                        CountInPackage = tempD;
                                    else
                                        CountInPackage = 0;

                                    tempS = reader["COST"].ToString();
                                    tempD = 0;
                                    if (double.TryParse(tempS, out tempD))
                                        Cost = tempD;
                                    else
                                        Cost = 0;

                                    tempS = reader["COUPON"].ToString();
                                    tempD = 0;
                                    if (double.TryParse(tempS, out tempD))
                                        Coupon = tempD;
                                    else
                                        Coupon = 0;

                                    tempS = reader["COUNT"].ToString();
                                    tempD = 0;
                                    if (double.TryParse(tempS, out tempD))
                                        Count = tempD;
                                    else
                                        Count = 0;

                                    tempS = reader["LOCKED_COUNT"].ToString();
                                    tempD = 0;
                                    if (double.TryParse(tempS, out tempD))
                                        LockedCount = tempD;
                                    else
                                        LockedCount = 0;

                                    tempS = reader["ID_UNIT"].ToString();
                                    int tempI = 0;
                                    if (int.TryParse(tempS, out tempI))
                                        ID_Unit = tempI;
                                    else
                                        ID_Unit = 0;

                                    tempS = reader["IS_ANNULATED"].ToString();
                                    bool tempB = false;
                                    if (bool.TryParse(tempS, out tempB))
                                        IsAnnulated = tempB;
                                    else
                                        IsAnnulated = false;

                                    getData = true;
                                }
                            }
                        }

                        connection.Close();
                    }

                    result = getData;
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

                        string sql = String.Format(@"UPDATE `PRODUCT` SET `PRODUCT_CODE` = '{0}', `NOMINATION` = '{1}', `COUNT_IN_PACKAGE` = '{2}', `COST` = '{3}', `COUPON` = '{4}', `ID_UNIT` = '{5}' WHERE `PRODUCT`.`ID` = {6}", _productCode, _nomination, Conversion.ToString(_countInPackage), Conversion.ToString(_cost), Conversion.ToString(_coupon), _id_unit, _id);
                        using (MySqlCommand command = new MySqlCommand(sql, connection))
                        {
                            command.ExecuteNonQuery();
                        }

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
        public static Product CreateProduct(string productCode, string nomination, double countInPackage, double cost, double coupon, int idUnit)
        {
            Product product = new Product();
            product.ProductCode = productCode;
            product.Nomination = nomination;
            product.CountInPackage = countInPackage;
            product.Cost = cost;
            product.Coupon = coupon;
            product.ID_Unit = idUnit;

            if (product.CreateProduct())
                return product;
            else
                return null;
        }

        /// <summary>
        /// Возвращает список не аннулированного товара
        /// </summary>
        /// <returns></returns>
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

                                string tempS = reader["ID"].ToString();
                                int tempI = 0;
                                if (int.TryParse(tempS, out tempI))
                                    item.ID = tempI;
                                else
                                    item.ID = 0;

                                item.ProductCode = reader["PRODUCT_CODE"].ToString();
                                item.Nomination = reader["NOMINATION"].ToString();

                                tempS = reader["COUNT_IN_PACKAGE"].ToString();
                                double tempD = 0;
                                if (double.TryParse(tempS, out tempD))
                                    item.CountInPackage = tempD;
                                else
                                    item.CountInPackage = 0;

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

                                tempS = reader["COUNT"].ToString();
                                tempD = 0;
                                if (double.TryParse(tempS, out tempD))
                                    item.Count = tempD;
                                else
                                    item.Count = 0;

                                tempS = reader["LOCKED_COUNT"].ToString();
                                tempD = 0;
                                if (double.TryParse(tempS, out tempD))
                                    item.LockedCount = tempD;
                                else
                                    item.LockedCount = 0;

                                tempS = reader["ID_UNIT"].ToString();
                                tempI = 0;
                                if (int.TryParse(tempS, out tempI))
                                    item.ID_Unit = tempI;
                                else
                                    item.ID_Unit = 0;

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
        /// <returns></returns>
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

                                string tempS = reader["ID"].ToString();
                                int tempI = 0;
                                if (int.TryParse(tempS, out tempI))
                                    item.ID = tempI;
                                else
                                    item.ID = 0;

                                item.ProductCode = reader["PRODUCT_CODE"].ToString();
                                item.Nomination = reader["NOMINATION"].ToString();

                                tempS = reader["COUNT_IN_PACKAGE"].ToString();
                                double tempD = 0;
                                if (double.TryParse(tempS, out tempD))
                                    item.CountInPackage = tempD;
                                else
                                    item.CountInPackage = 0;

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

                                tempS = reader["COUNT"].ToString();
                                tempD = 0;
                                if (double.TryParse(tempS, out tempD))
                                    item.Count = tempD;
                                else
                                    item.Count = 0;

                                tempS = reader["LOCKED_COUNT"].ToString();
                                tempD = 0;
                                if (double.TryParse(tempS, out tempD))
                                    item.LockedCount = tempD;
                                else
                                    item.LockedCount = 0;

                                tempS = reader["ID_UNIT"].ToString();
                                tempI = 0;
                                if (int.TryParse(tempS, out tempI))
                                    item.ID_Unit = tempI;
                                else
                                    item.ID_Unit = 0;

                                tempS = reader["IS_ANNULATED"].ToString();
                                bool tempB = false;
                                if (bool.TryParse(tempS, out tempB))
                                    item.IsAnnulated = tempB;
                                else
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
                Dialog.ErrorMessage(null, "Ошибка получения списка товара", ex.Message);
            }

            return Products.OrderBy(p => p.ProductCode).ToList();
        }

        #endregion
    }
}
