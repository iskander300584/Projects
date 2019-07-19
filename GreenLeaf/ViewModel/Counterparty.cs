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

        private string _visibleName = string.Empty;
        /// <summary>
        /// Отображаемое имя
        /// </summary>
        public string VisibleName
        {
            get { return _visibleName; }
        }

        // Изменение свойств объекта
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        /// <summary>
        /// Получение отображаемого имени
        /// </summary>
        private void GetVisibleName()
        {
            if (_nomination.Trim() != "")
                _visibleName = _nomination;
            else if(_surname.Trim() != string.Empty)
            {
                _visibleName = _surname;

                if(_name.Trim() != "")
                {
                    _visibleName += _name[0] + ".";

                    if (_patronymic.Trim() != "")
                        _visibleName += " " + _patronymic[0] + ".";
                }
            }
            else if(_name.Trim() != string.Empty)
            {
                _visibleName = _name;

                if (_patronymic.Trim() != string.Empty)
                    _visibleName += " " + _patronymic;
            }

            OnPropertyChanged("VisibleName");
        }

        /// <summary>
        /// Получить данные по ID
        /// <para>возвращает TRUE, если данные успешно получены</para>
        /// </summary>
        /// <returns>возвращает TRUE, если данные успешно получены</returns>
        public bool GetDataByID()
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

                        string sql = "SELECT * FROM PRODUCT WHERE ID=" + ID.ToString();
                        MySqlCommand command = new MySqlCommand(sql, connection);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Surname = reader["SURNAME"].ToString();
                                Name = reader["SURNAME"].ToString();
                                Patronymic = reader["PATRONYMIC"].ToString();
                                Nomination = reader["NOMINATION"].ToString();

                                string tempS = reader["ADRESS"].ToString();
                                if (tempS != "")
                                    Adress = Criptex.UnCript(tempS);

                                tempS = reader["PHONE"].ToString();
                                if (tempS != "")
                                    Phone = Criptex.UnCript(tempS);

                                tempS = reader["IS_PROVIDER"].ToString();
                                bool tempB = false;
                                if (bool.TryParse(tempS, out tempB))
                                    IsProvider = tempB;
                                else
                                    IsProvider = false;

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
        /// <returns>возвращает TRUE, если данные успешно получены</returns>
        public bool GetDateByID(int id)
        {
            _id = id;

            return GetDataByID();
        }

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
                using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                {
                    connection.Open();

                    string sql = "SELECT * FROM COUNTERPARTY WHERE IS_PROVIDER=\'" + provider_value + "\'";
                    MySqlCommand command = new MySqlCommand(sql, connection);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Counterparty item = new Counterparty();

                            string tempS = reader["ID"].ToString();
                            int tempI = 0;
                            if (int.TryParse(tempS, out tempI))
                                item.ID = tempI;
                            else
                                item.ID = 0;

                            item.Surname = reader["SURNAME"].ToString();
                            item.Name = reader["SURNAME"].ToString();
                            item.Patronymic = reader["PATRONYMIC"].ToString();
                            item.Nomination = reader["NOMINATION"].ToString();

                            tempS = reader["ADRESS"].ToString();
                            if (tempS != "")
                                item.Adress = Criptex.UnCript(tempS);

                            tempS = reader["PHONE"].ToString();
                            if (tempS != "")
                                item.Phone = Criptex.UnCript(tempS);

                            tempS = reader["IS_PROVIDER"].ToString();
                            bool tempB = false;
                            if (bool.TryParse(tempS, out tempB))
                                item.IsProvider = tempB;
                            else
                                item.IsProvider = false;

                            Counterparties.Add(item);
                        }
                    }

                    connection.Close();
                }

            }
            catch (Exception ex)
            {
                Dialog.ErrorMessage(null, "Ошибка получения данных", ex.Message);
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
        /// <returns></returns>
        public static List<Counterparty> GetCustomerList()
        {
            return GetContragentList(0);
        }
    }
}
