using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MySql.Data.MySqlClient;
using GreenLeaf.Classes;
using System.Collections.Generic;

namespace GreenLeaf.ViewModel
{
    /// <summary>
    /// Контрагент
    /// </summary>
    public class Counterparty : INotifyPropertyChanged
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

        private string _code = string.Empty;
        /// <summary>
        /// Код контрагента
        /// </summary>
        public string Code
        {
            get { return _code; }
            set
            {
                if(_code != value)
                {
                    _code = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _surname = string.Empty;
        /// <summary>
        /// Фамилия
        /// </summary>
        public string Surname
        {
            get { return _surname; }
            set
            {
                if(_surname != value)
                {
                    _surname = value;
                    OnPropertyChanged();

                    GetVisibleName();
                }
            }
        }

        private string _name = string.Empty;
        /// <summary>
        /// Имя
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();

                    GetVisibleName();
                }
            }
        }

        private string _patronymic = string.Empty;
        /// <summary>
        /// Отчество
        /// </summary>
        public string Patronymic
        {
            get { return _patronymic; }
            set
            {
                if (_patronymic != value)
                {
                    _patronymic = value;
                    OnPropertyChanged();

                    GetVisibleName();
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

                    GetVisibleName();
                }
            }
        }

        private string _adress = string.Empty;
        /// <summary>
        /// Адрес
        /// </summary>
        public string Adress
        {
            get { return _adress; }
            set
            {
                if (_adress != value)
                {
                    _adress = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _phone = string.Empty;
        /// <summary>
        /// Телефон
        /// </summary>
        public string Phone
        {
            get { return _phone; }
            set
            {
                if (_phone != value)
                {
                    _phone = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isProvider = false;
        /// <summary>
        /// Поставщик
        /// </summary>
        public bool IsProvider
        {
            get { return _isProvider; }
            set
            {
                if(_isProvider != value)
                {
                    _isProvider = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isAnnulated = false;
        /// <summary>
        /// Контрагент аннулирован
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

        private string _visibleName = string.Empty;
        /// <summary>
        /// Отображаемое имя
        /// </summary>
        public string VisibleName
        {
            get { return _visibleName; }
        }

        private string _fullVisibleName = string.Empty;
        /// <summary>
        /// Полное отображаемое имя
        /// </summary>
        public string FullVisibleName
        {
            get { return _fullVisibleName; }
        }

        // Изменение свойств объекта
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        /// <summary>
        /// Создать контрагента
        /// </summary>
        /// <returns>возвращает TRUE, если контрагент успешно создан</returns>
        public bool CreateCounterparty()
        {
            bool result = false;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ProgramSettings.ConnectionString)))
                {
                    connection.Open();

                    string sql = String.Format(@"INSERT INTO `COUNTERPARTY` (`SURNAME`, `NAME`, `PATRONYMIC`, `ADRESS`, `PHONE`, `NOMINATION`, `IS_PROVIDER`, `IS_ANNULATED`, `CODE`)  VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}')", Surname, Name, Patronymic, Conversion.ToCriptString(Adress), Conversion.ToCriptString(Phone), Nomination, Conversion.ToString(IsProvider), 0, Code);

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();

                        ID = (int)command.LastInsertedId;
                    }

                    string name = (Nomination != "") ? Nomination : Surname + " " + Name;

                    Journal.CreateJournal("создал", "контрагента " + name, connection);

                    connection.Close();
                }

                result = true;
            }
            catch (Exception ex)
            {
                Dialog.ErrorMessage(null, "Ошибка создания контрагента", ex.Message);
            }

            return result;
        }

        #region Получение данных

        /// <summary>
        /// Получить все данные по ID
        /// <para>возвращает TRUE, если данные успешно получены</para>
        /// </summary>
        /// <returns>возвращает TRUE, если данные успешно получены</returns>
        public bool GetFullDataByID()
        {
            bool result = false;

            if (_id != 0)
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ProgramSettings.ConnectionString)))
                    {
                        connection.Open();

                        string sql = "SELECT * FROM `COUNTERPARTY` WHERE `COUNTERPARTY`.`ID` = " + ID.ToString();

                        using (MySqlCommand command = new MySqlCommand(sql, connection))
                        {
                            using (MySqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Code = reader["CODE"].ToString();
                                    Surname = reader["SURNAME"].ToString();
                                    Name = reader["NAME"].ToString();
                                    Patronymic = reader["PATRONYMIC"].ToString();
                                    Nomination = reader["NOMINATION"].ToString();

                                    Adress = Conversion.ToUncriptString(reader["ADRESS"].ToString());
                                    Phone = Conversion.ToUncriptString(reader["PHONE"].ToString());

                                    IsProvider = Conversion.ToBool(reader["IS_PROVIDER"].ToString());
                                    IsAnnulated = Conversion.ToBool(reader["IS_ANNULATED"].ToString());
                                }
                            }
                        }

                        connection.Close();
                    }

                    result = true;
                }
                catch (Exception ex)
                {
                    Dialog.ErrorMessage(null, "Ошибка получения данных о контрагенте", ex.Message);
                }
            }
            else
            {
                Dialog.ErrorMessage(null, "Не указан ID контрагента");
            }

            return result;
        }

        /// <summary>
        /// Получить все данные по ID
        /// <para>возвращает TRUE, если данные успешно получены</para>
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>возвращает TRUE, если данные успешно получены</returns>
        public bool GetFullDataByID(int id)
        {
            _id = id;

            return GetFullDataByID();
        }

        /// <summary>
        /// Получить не защищенные данные по ID
        /// <para>возвращает TRUE, если данные успешно получены</para>
        /// </summary>
        /// <returns>возвращает TRUE, если данные успешно получены</returns>
        public bool GetPublicDataByID()
        {
            bool result = false;

            if (_id != 0)
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ProgramSettings.ConnectionString)))
                    {
                        connection.Open();

                        string sql = "SELECT * FROM `COUNTERPARTY` WHERE `COUNTERPARTY`.`ID` = " + ID.ToString();

                        using (MySqlCommand command = new MySqlCommand(sql, connection))
                        {
                            using (MySqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Code = reader["CODE"].ToString();
                                    Surname = reader["SURNAME"].ToString();
                                    Name = reader["NAME"].ToString();
                                    Patronymic = reader["PATRONYMIC"].ToString();
                                    Nomination = reader["NOMINATION"].ToString();

                                    Adress = string.Empty;
                                    Phone = string.Empty;

                                    IsProvider = Conversion.ToBool(reader["IS_PROVIDER"].ToString());
                                    IsAnnulated = Conversion.ToBool(reader["IS_ANNULATED"].ToString());
                                }
                            }
                        }

                        connection.Close();
                    }

                    result = true;
                }
                catch (Exception ex)
                {
                    Dialog.ErrorMessage(null, "Ошибка получения данных о контрагенте", ex.Message);
                }
            }
            else
            {
                Dialog.ErrorMessage(null, "Не указан ID контрагента");
            }

            return result;
        }

        /// <summary>
        /// Получить не защищенные данные по ID
        /// <para>возвращает TRUE, если данные успешно получены</para>
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>возвращает TRUE, если данные успешно получены</returns>
        public bool GetPublicDataByID(int id)
        {
            _id = id;

            return GetPublicDataByID();
        }

        /// <summary>
        /// Получение отображаемого имени
        /// </summary>
        private void GetVisibleName()
        {
            if (_nomination.Trim() != "")
            {
                _visibleName = _nomination;
                _fullVisibleName = _nomination;
            }
            else if (_surname.Trim() != "")
            {
                _visibleName = _surname;
                _fullVisibleName = _surname;

                if (_name.Trim() != "")
                {
                    _visibleName += " " + _name[0] + ".";
                    _fullVisibleName += " " + _name;

                    if (_patronymic.Trim() != "")
                    {
                        _visibleName += _patronymic[0] + ".";
                        _fullVisibleName += " " + _patronymic;
                    }
                }
            }
            else if (_name.Trim() != "")
            {
                _visibleName = _name;

                if (_patronymic.Trim() != string.Empty)
                    _visibleName += " " + _patronymic;

                _fullVisibleName = _visibleName;
            }

            OnPropertyChanged("VisibleName");
            OnPropertyChanged("FullVisibleName");
        }

        #endregion

        #region Редактирование данных

        /// <summary>
        /// Аннулировать контрагента
        /// </summary>
        /// <returns>возвращает TRUE, если контрагент успешно аннулирован</returns>
        public bool AnnuateCounterparty()
        {
            bool result = false;

            if (_id != 0)
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ProgramSettings.ConnectionString)))
                    {
                        connection.Open();

                        string sql = @"UPDATE `COUNTERPARTY` SET `IS_ANNULATED` = '1' WHERE `COUNTERPARTY`.`ID` = " + ID.ToString();

                        using (MySqlCommand command = new MySqlCommand(sql, connection))
                        {
                            command.ExecuteNonQuery();
                        }

                        IsAnnulated = true;

                        string name = (Nomination != "") ? Nomination : Surname + " " + Name;

                        Journal.CreateJournal("аннулировал", "контрагента " + name, connection);

                        connection.Close();
                    }

                    result = true;
                }
                catch (Exception ex)
                {
                    Dialog.ErrorMessage(null, "Ошибка аннулирования контрагента", ex.Message);
                }
            }
            else
            {
                Dialog.ErrorMessage(null, "Не указан ID контрагента");
            }

            return result;
        }

        /// <summary>
        /// Редактировать данные контрагента
        /// </summary>
        /// <returns>возвращает TRUE, если контрагент успешно отредактирован</returns>
        public bool EditCounterparty()
        {
            bool result = false;

            if (_id != 0)
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ProgramSettings.ConnectionString)))
                    {
                        connection.Open();

                        string sql = String.Format(@"UPDATE `COUNTERPARTY` SET `SURNAME` = '{0}', `NAME` = '{1}', `PATRONYMIC` = '{2}', `ADRESS` = '{3}', `PHONE` = '{4}', `NOMINATION` = '{5}', `IS_PROVIDER` = '{6}', `CODE` = '{7}' WHERE `COUNTERPARTY`.`ID` = {8}", Surname, Name, Patronymic, Conversion.ToCriptString(Adress), Conversion.ToCriptString(Phone), Nomination, Conversion.ToString(IsProvider), Code, ID);

                        using (MySqlCommand command = new MySqlCommand(sql, connection))
                        {
                            command.ExecuteNonQuery();
                        }

                        string name = (Nomination != "") ? Nomination : Surname + " " + Name;

                        Journal.CreateJournal("изменил", "данные контрагента " + name, connection);

                        connection.Close();
                    }

                    result = true;
                }
                catch (Exception ex)
                {
                    Dialog.ErrorMessage(null, "Ошибка редактирования контрагента", ex.Message);
                }
            }
            else
            {
                Dialog.ErrorMessage(null, "Не указан ID контрагента");
            }

            return result;
        }

        #endregion

        #region Статические методы

        /// <summary>
        /// Получить список контрагентов
        /// </summary>
        /// <param name="provider_value">значение поля IS_PROVIDER</param>
        /// <returns></returns>
        private static List<Counterparty> GetContragentList(int provider_value)
        {
            List<Counterparty> Counterparties = new List<Counterparty>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ProgramSettings.ConnectionString)))
                {
                    connection.Open();

                    string sql = "SELECT * FROM `COUNTERPARTY` WHERE `COUNTERPARTY`.`IS_PROVIDER` = \'" + provider_value + "\' AND `COUNTERPARTY`.`IS_ANNULATED` = \'0\'";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Counterparty item = new Counterparty();

                                item.ID = Conversion.ToInt(reader["ID"].ToString());
                                item.Code = reader["CODE"].ToString();
                                item.Surname = reader["SURNAME"].ToString();
                                item.Name = reader["NAME"].ToString();
                                item.Patronymic = reader["PATRONYMIC"].ToString();
                                item.Nomination = reader["NOMINATION"].ToString();

                                item.Adress = Conversion.ToUncriptString(reader["ADRESS"].ToString());
                                item.Phone = Conversion.ToUncriptString(reader["PHONE"].ToString());

                                item.IsProvider = Conversion.ToBool(reader["IS_PROVIDER"].ToString());

                                Counterparties.Add(item);
                            }
                        }
                    }

                    connection.Close();
                }

            }
            catch (Exception ex)
            {
                Dialog.ErrorMessage(null, "Ошибка получения данных о контрагенте", ex.Message);
            }

            return Counterparties;
        }

        /// <summary>
        /// Получить список поставщиков
        /// </summary>
        /// <returns></returns>
        public static List<Counterparty> GetProviderList()
        {
            return GetContragentList(1);
        }

        /// <summary>
        /// Получить список клиентов
        /// </summary>
        public static List<Counterparty> GetCustomerList()
        {
            return GetContragentList(0);
        }

        /// <summary>
        /// Получить контрагента с незащищенными данными по ID
        /// </summary>
        /// <param name="id">ID</param>
        public static Counterparty GetCounterpartyByID(int id)
        {
            Counterparty counterparty = new Counterparty();

            if (counterparty.GetPublicDataByID(id))
                return counterparty;
            else
                return null;
        }

        #endregion
    }
}