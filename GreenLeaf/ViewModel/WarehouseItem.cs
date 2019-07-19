using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MySql.Data.MySqlClient;
using GreenLeaf.Classes;

namespace GreenLeaf.ViewModel
{
    public class WarehouseItem : INotifyPropertyChanged
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

        private int _id_product = 0;
        /// <summary>
        /// ID вида товара
        /// </summary>
        public int ID_Product
        {
            get { return _id_product; }
            set
            {
                if(_id_product != value)
                {
                    _id_product = value;
                    OnPropertyChanged();

                    /*_product = new Product { ID = _id_product };
                    _product.GetDataByID();
                    OnPropertyChanged("Product");*/
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
            set
            {
                if(_product != value)
                {
                    _product = value;
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
                if(_count != value)
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
                if(_id_unit != value)
                {
                    _id_unit = value;
                    OnPropertyChanged();

                    _unitNomination = MeasureUnit.Units[_id_unit];
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
        public bool GetDataByID()
        {
            bool result = false;

            if (ID != 0)
            {
                try
                {
                    bool getData = false;

                    using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                    {
                        connection.Open();

                        string sql = "SELECT * FROM WAREHOUSE WHERE ID=" + ID.ToString();
                        MySqlCommand command = new MySqlCommand(sql, connection);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string tempS = reader["ID_PRODUCT"].ToString();
                                int tempI = 0;
                                int.TryParse(tempS, out tempI);
                                ID_Product = tempI;

                                tempS = reader["COUNT"].ToString();
                                double tempD = 0;
                                double.TryParse(tempS, out tempD);
                                Count = tempD;

                                tempS = reader["ID_UNIT"].ToString();
                                tempI = 0;
                                int.TryParse(tempS, out tempI);
                                ID_Unit = tempI;

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
        public bool GetDataByID(int id)
        {
            ID = id;

            return GetDataByID();
        }

        /// <summary>
        /// Получить данные по ID товара
        /// <para>возвращает TRUE, если данные успешно получены</para>
        /// </summary>
        public bool GetDataByID_Product()
        {
            bool result = false;

            if (ID_Product != 0)
            {
                try
                {
                    bool getData = false;

                    using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                    {
                        connection.Open();

                        string sql = "SELECT * FROM WAREHOUSE WHERE ID_PRODUCT=" + ID_Product.ToString();
                        MySqlCommand command = new MySqlCommand(sql, connection);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string tempS = reader["ID"].ToString();
                                int tempI = 0;
                                int.TryParse(tempS, out tempI);
                                ID = tempI;

                                tempS = reader["COUNT"].ToString();
                                double tempD = 0;
                                double.TryParse(tempS, out tempD);
                                Count = tempD;

                                tempS = reader["ID_UNIT"].ToString();
                                tempI = 0;
                                int.TryParse(tempS, out tempI);
                                ID_Unit = tempI;

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
        /// Получить данные по ID товара
        /// <para>возвращает TRUE, если данные успешно получены</para>
        /// </summary>
        /// <param name="id_product">ID товара</param>
        public bool GetDataByID_Product(int id_product)
        {
            ID_Product = id_product;

            return GetDataByID_Product();
        }
    }
}
