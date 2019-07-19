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
                }
            }
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

        private bool _isAnnuled = false;
        /// <summary>
        /// Аннулировано
        /// </summary>
        public bool IsAnnuled
        {
            get { return _isAnnuled; }
            set
            {
                if(_isAnnuled != value)
                {
                    _isAnnuled = value;
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

                        string sql = "SELECT * FROM PRODUCT WHERE ID=" + ID.ToString();
                        MySqlCommand command = new MySqlCommand(sql, connection);

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

                                tempS = reader["ID_UNIT"].ToString();
                                int tempI = 0;
                                if (int.TryParse(tempS, out tempI))
                                    ID_Unit = tempI;
                                else
                                    ID_Unit = 0;

                                tempS = reader["IS_ANNULED"].ToString();
                                bool tempB = false;
                                if (bool.TryParse(tempS, out tempB))
                                    IsAnnuled = tempB;
                                else
                                    IsAnnuled = false;

                                getData = true;
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

                    string sql = "SELECT * FROM PRODUCT WHERE IS_ANNULED=\'0\'";
                    MySqlCommand command = new MySqlCommand(sql, connection);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Product item = new Product();

                            string tempS = reader["COUNT_IN_PACKAGE"].ToString();
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

                            tempS = reader["ID_UNIT"].ToString();
                            tempI = 0;
                            if (int.TryParse(tempS, out tempI))
                                item.ID_Unit = tempI;
                            else
                                item.ID_Unit = 0;

                            item.IsAnnuled = false;

                            Products.Add(item);
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
    }
}
