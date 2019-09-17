using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MySql.Data.MySqlClient;
using GreenLeaf.Classes;

namespace GreenLeaf.ViewModel
{
    /// <summary>
    /// Нумератор
    /// </summary>
    public class Numerator : INotifyPropertyChanged
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

        private int _value = 0;
        /// <summary>
        /// Последнее значение
        /// </summary>
        public int Value
        {
            get { return _value; }
            set
            {
                if(_value != value)
                {
                    _value = value;
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
        /// Установить значение нумератора
        /// </summary>
        /// <param name="value">значение</param>
        /// <returns>возвращает TRUE, если значение изменено успешно</returns>
        public bool SetValue(int value)
        {
            bool result = false;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                {
                    connection.Open();

                    string sql = String.Format(@"UPDATE `NUMERATOR` SET `VALUE` = '{0}' WHERE `NUMERATOR`.`ID` = {1}", value, ID);

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    connection.Close();

                    Value = value;
                }

                result = true;
            }
            catch (Exception ex)
            {
                Dialog.ErrorMessage(null, "Ошибка изменения значения нумератора", ex.Message);
            }

            return result;
        }

        #region Статические методы

        /// <summary>
        /// Получить значение нумератора
        /// </summary>
        /// <param name="isPurchase">приходная накладная</param>
        /// <returns>возвращает текущее значение нумератора, или 0, если данные не получены</returns>
        public static int GetNumeratorValue(bool isPurchase)
        {
            int value = 0;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                {
                    connection.Open();

                    string nomination = (isPurchase) ? "Приходная накладная" : "Расходная накладная";

                    string sql = String.Format(@"SELECT `VALUE` FROM `NUMERATOR` WHERE `NUMERATOR`.`NOMINATION` = '{0}'", nomination);

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                value = Conversion.ToInt(reader["VALUE"].ToString());
                            }
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Dialog.ErrorMessage(null, "Ошибка получения значения нумератора", ex.Message);
            }

            return value;
        }

        /// <summary>
        /// Установить значение нумератора
        /// </summary>
        /// <param name="value">значение</param>
        /// <param name="isPurchase">приходная накладная</param>
        /// <returns>возвращает TRUE, если значение нумератора изменено успешно</returns>
        public static bool SetNumeratorValue(int value, bool isPurchase)
        {
            bool result = false;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                {
                    connection.Open();

                    string nomination = (isPurchase) ? "Приходная накладная" : "Расходная накладная";

                    string sql = String.Format(@"UPDATE `NUMERATOR` SET `VALUE` = '{0}' WHERE `NUMERATOR`.`NOMINATION` = '{1}'", value, nomination);

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
                Dialog.ErrorMessage(null, "Ошибка изменения значения нумератора", ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Получение списка нумераторов
        /// </summary>
        public static List<Numerator> GetNumerators()
        {
            List<Numerator> numerators = new List<Numerator>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                {
                    connection.Open();

                    string sql = String.Format(@"SELECT * FROM `NUMERATOR`");

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Numerator numerator = new Numerator();

                                numerator.ID = Conversion.ToInt(reader["ID"].ToString());
                                numerator.Nomination = reader["NOMINATION"].ToString();
                                numerator.Value = Conversion.ToInt(reader["VALUE"].ToString());

                                numerators.Add(numerator);
                            }
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Dialog.ErrorMessage(null, "Ошибка получения списка нумераторов", ex.Message);
            }

            return numerators;
        }

        #endregion
    }
}
