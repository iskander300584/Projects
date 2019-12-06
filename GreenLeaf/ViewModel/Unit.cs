using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MySql.Data.MySqlClient;
using GreenLeaf.Classes;

namespace GreenLeaf.ViewModel
{
    /// <summary>
    /// Единица измерения
    /// </summary>
    public class Unit : INotifyPropertyChanged
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
                if (_nomination != value)
                {
                    _nomination = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isAnnuleted = false;
        /// <summary>
        /// Аннулирована
        /// </summary>
        public bool IsAnnulated
        {
            get { return _isAnnuleted; }
            set
            {
                if(_isAnnuleted != value)
                {
                    _isAnnuleted = value;
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
        /// Создание единицы измерения
        /// </summary>
        /// <returns>возвращает TRUE, если единица измерения создана успешно</returns>
        public bool CreateUnit()
        {
            bool result = false;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ProgramSettings.ConnectionString)))
                {
                    connection.Open();

                    string sql = String.Format(@"INSERT INTO `UNIT` (`NOMINATION`) VALUES ('{0}')", Nomination);

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();

                        ID = (int)command.LastInsertedId;
                    }

                    Journal.CreateJournal("создал", "единицу измерения " + Nomination, connection);

                    connection.Close();
                }

                result = true;
            }
            catch (Exception ex)
            {
                Dialog.ErrorMessage(null, "Ошибка создания единицы измерения", ex.Message);
            }

            return result;
        }

        #region Редактирование данных

        /// <summary>
        /// Редактирование единицы измерения
        /// </summary>
        /// <returns>возвращает TRUE, если единица измерения отредактирована успешно</returns>
        public bool EditUnit()
        {
            bool result = false;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ProgramSettings.ConnectionString)))
                {
                    connection.Open();

                    string sql = String.Format(@"UPDATE `UNIT` SET `NOMINATION` = '{0}' WHERE `UNIT`.`ID` = {1}", Nomination, ID);

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    Journal.CreateJournal("изменил", "единицу измерения " + Nomination, connection);

                    connection.Close();
                }

                result = true;
            }
            catch (Exception ex)
            {
                Dialog.ErrorMessage(null, "Ошибка редактирования единицы измерения", ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Редактирование единицы измерения
        /// </summary>
        /// <param name="nomination">наименование</param>
        /// <returns>возвращает TRUE, если единица измерения отредактирована успешно</returns>
        public bool EditUnit(string nomination)
        {
            _nomination = nomination;

            return EditUnit();
        }

        /// <summary>
        /// Аннулирование единицы измерения
        /// </summary>
        /// <returns>возвращает TRUE, если единица измерения аннулирована успешно</returns>
        public bool AnnulateUnit()
        {
            bool result = false;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ProgramSettings.ConnectionString)))
                {
                    connection.Open();

                    string sql = String.Format(@"UPDATE `UNIT` SET `IS_ANNULATED` = '1' WHERE `UNIT`.`ID` = {0}", ID);

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    Journal.CreateJournal("аннулировал", "единицу измерения " + Nomination, connection);

                    connection.Close();

                    IsAnnulated = true;
                }

                result = true;
            }
            catch (Exception ex)
            {
                Dialog.ErrorMessage(null, "Ошибка аннулирования единицы измерения", ex.Message);
            }

            return result;
        }

        #endregion

        #region Статические методы

        /// <summary>
        /// Получение списка актуальных единиц измерения
        /// </summary>
        public static List<Unit> GetActualUnits()
        {
            List<Unit> units = new List<Unit>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ProgramSettings.ConnectionString)))
                {
                    connection.Open();

                    string sql = String.Format(@"SELECT * FROM `UNIT` WHERE `UNIT`.`IS_ANNULATED` = '0'");

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Unit unit = new Unit();

                                unit.ID = Conversion.ToInt(reader["ID"].ToString());
                                unit.Nomination = reader["NOMINATION"].ToString();

                                units.Add(unit);
                            }
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Dialog.ErrorMessage(null, "Ошибка получения списка единиц измерения", ex.Message);
            }

            return units;
        }

        /// <summary>
        /// Получение словаря единиц измерения ID-NOMINATION
        /// </summary>
        public static Dictionary<int, string> GetMeasures()
        {
            Dictionary<int, string> measures = new Dictionary<int, string>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ProgramSettings.ConnectionString)))
                {
                    connection.Open();

                    string sql = String.Format(@"SELECT * FROM `UNIT`");

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int id = Conversion.ToInt(reader["ID"].ToString());
                                string nomination = reader["NOMINATION"].ToString();

                                measures.Add(id, nomination);
                            }
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Dialog.ErrorMessage(null, "Ошибка получения словаря единиц измерения", ex.Message);
            }

            return measures;
        }

        #endregion
    }
}