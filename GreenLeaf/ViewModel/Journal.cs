﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MySql.Data.MySqlClient;
using GreenLeaf.Classes;

namespace GreenLeaf.ViewModel
{
    public class Journal : INotifyPropertyChanged
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

        private DateTime _date = DateTime.MinValue;
        /// <summary>
        /// Дата накладной
        /// </summary>
        public DateTime Date
        {
            get { return _date; }
            set
            {
                if (_date != value)
                {
                    _date = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _id_account = 0;
        /// <summary>
        /// ID исполнителя
        /// </summary>
        public int ID_Account
        {
            get { return _id_account; }
            set
            {
                if (_id_account != value)
                {
                    _id_account = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _act = string.Empty;
        /// <summary>
        /// Событие
        /// </summary>
        public string Act
        {
            get { return _act; }
            set
            {
                if(_act != value)
                {
                    _act = value;
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
        /// Создать запись в журнале событий
        /// </summary>
        /// <returns>возвращает TRUE, если запись создана успешно</returns>
        public bool CreateJournal()
        {
            bool result = false;

            return result;
        }

        #region Статические методы

        /// <summary>
        /// Получить список записей журнала
        /// </summary>
        /// <param name="idAccount">ID исполнителя</param>
        /// <param name="from">дата начала периода</param>
        /// <param name="to">дата окончания периода</param>
        private static List<Journal> GetJournalList(int? idAccount, DateTime? from, DateTime? to)
        {
            List<Journal> list = new List<Journal>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                {
                    connection.Open();

                    string fromDate = string.Empty;
                    string toDate = string.Empty;

                    if(from != null && to != null)
                    {
                        fromDate = String.Format(@"'{0}-{1}-{2}T00:00:00.000'", ((DateTime)from).Year, ((DateTime)from).Month, ((DateTime)from).Day);
                        toDate = String.Format(@"'{0}-{1}-{2}T00:00:00.000'", ((DateTime)to).Year, ((DateTime)to).Month, ((DateTime)to).Day);
                    }

                    string sql = String.Format(@"SELECT * FROM `JOURNAL`");

                    if(idAccount != null)
                    {
                        sql += " WHERE `JOURNAL`.`ID_ACCOUNT` = " + (int)idAccount;

                        if(from != null && to != null)
                        {
                            sql += " AND `JOURNAL`.`DATE` >= " + fromDate + " AND `JOURNAL`.`DATE` <= " + toDate;
                        }
                    }
                    else if(from != null && to != null)
                    {
                        sql += " WHERE `JOURNAL`.`DATE` >= " + fromDate + " AND `JOURNAL`.`DATE` <= " + toDate;
                    }

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Journal journal = new Journal();

                                journal.ID = Conversion.ToInt(reader["ID"].ToString());
                                journal.Date = Conversion.ToDateTime(reader["DATE"].ToString());
                                journal.ID_Account = Conversion.ToInt(reader["ID_ACCOUNT"].ToString());
                                journal.Act = reader["ACT"].ToString();

                                list.Add(journal);
                            }
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Dialog.ErrorMessage(null, "Ошибка получения журнала событий", ex.Message);
            }

            return list;
        }

        /// <summary>
        /// Получить список записей журнала
        /// </summary>
        public static List<Journal> GetJournal()
        {
            return GetJournalList(null, null, null);
        }

        /// <summary>
        /// Получить список записей журнала
        /// </summary>
        /// <param name="idAccount">ID исполнителя</param>
        public static List<Journal> GetJournal(int idAccount)
        {
            return GetJournalList(idAccount, null, null);
        }

        /// <summary>
        /// Получить список записей журнала
        /// </summary>
        /// <param name="from">дата начала периода</param>
        /// <param name="to">дата окончания периода</param>
        public static List<Journal> GetJournal(DateTime from, DateTime to)
        {
            return GetJournalList(null, from, to);
        }

        /// <summary>
        /// Получить список записей журнала
        /// </summary>
        /// <param name="idAccount">ID исполнителя</param>
        /// <param name="from">дата начала периода</param>
        /// <param name="to">дата окончания периода</param>
        /// <returns></returns>
        public static List<Journal> GetJournal(int idAccount, DateTime from, DateTime to)
        {
            return GetJournalList(idAccount, from, to);
        }

        #endregion
    }
}
