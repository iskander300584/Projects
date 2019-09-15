using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MySql.Data.MySqlClient;
using GreenLeaf.Classes;

namespace GreenLeaf.ViewModel
{
    /// <summary>
    /// Накладная
    /// </summary>
    public class Invoice : INotifyPropertyChanged
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

        private int _number = 0;
        /// <summary>
        /// Номер накладной
        /// </summary>
        public int Number
        {
            get { return _number; }
            set
            {
                if (_number != value)
                {
                    _number = value;
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

        private int _id_counterparty = 0;
        /// <summary>
        /// ID контрагента
        /// </summary>
        public int ID_Counterparty
        {
            get { return _id_counterparty; }
            set
            {
                if (_id_counterparty != value)
                {
                    _id_counterparty = value;
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
                if(_date != value)
                {
                    _date = value;
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
                if (_coupon != value)
                {
                    _coupon = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _is_issued = false;
        /// <summary>
        /// Проведена
        /// </summary>
        public bool IsIssued
        {
            get { return _is_issued; }
            set
            {
                if(_is_issued != value)
                {
                    _is_issued = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _is_locked = false;
        /// <summary>
        /// Заблокирована
        /// </summary>
        public bool IsLocked
        {
            get { return _is_locked; }
            set
            {
                if(_is_locked != value)
                {
                    _is_locked = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _is_purchase = true;
        /// <summary>
        /// Является приходной накладной
        /// </summary>
        public bool IsPurchase
        {
            get { return _is_purchase; }
            set
            {
                if (_is_purchase != value)
                {
                    _is_purchase = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<InvoiceItem> _items = new List<InvoiceItem>();
        /// <summary>
        /// Список элементов накладной
        /// </summary>
        public List<InvoiceItem> Items
        {
            get { return _items; }
            set
            {
                if(_items != value)
                {
                    _items = value;
                    OnPropertyChanged();

                    Calc();
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
        /// Вычисление стоимости и купона
        /// </summary>
        public void Calc()
        {
            double cost = 0;
            double coupon = 0;

            foreach(InvoiceItem item in Items)
            {
                cost += item.Cost;
                coupon += item.Coupon;
            }

            Cost = cost;
            Coupon = coupon;
        }

        #region Получение данных

        /// <summary>
        /// Получить данные по ID
        /// <para>возвращает TRUE, если данные успешно получены</para>
        /// </summary
        /// <param name="isPurchase">TRUE если приходная накладная, FALSE если расходная</param>
        /// <returns>возвращает TRUE, если данные успешно получены</returns>
        public bool GetDataByID(bool isPurchase)
        {
            _is_purchase = isPurchase;

            bool result = false;

            if (_id != 0)
            {
                try
                {
                    bool getData = false;

                    using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                    {
                        connection.Open();

                        string table = (isPurchase) ? "PURCHASE_INVOICE" : "SALES_INVOICE";

                        string sql = "SELECT * FROM " + table + " WHERE ID=" + ID.ToString();
                        using (MySqlCommand command = new MySqlCommand(sql, connection))
                        {
                            using (MySqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    string tempS = reader["NUMBER"].ToString();
                                    int tempI = 0;
                                    if (int.TryParse(tempS, out tempI))
                                        Number = tempI;
                                    else
                                        Number = 0;

                                    tempS = reader["ID_ACCOUNT"].ToString();
                                    tempI = 0;
                                    if (int.TryParse(tempS, out tempI))
                                        ID_Account = tempI;
                                    else
                                        ID_Account = 0;

                                    tempS = reader["ID_COUNTERPARTY"].ToString();
                                    tempI = 0;
                                    if (int.TryParse(tempS, out tempI))
                                        ID_Counterparty = tempI;
                                    else
                                        ID_Counterparty = 0;

                                    tempS = reader["DATE"].ToString();
                                    DateTime tempDT = DateTime.MinValue;
                                    if (DateTime.TryParse(tempS, out tempDT))
                                        Date = tempDT;
                                    else
                                        Date = DateTime.MinValue;

                                    tempS = reader["COST"].ToString();
                                    double tempD = 0;
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

                                    tempS = reader["IS_ISSUED"].ToString();
                                    bool tempB = false;
                                    if (bool.TryParse(tempS, out tempB))
                                        IsIssued = tempB;
                                    else
                                        IsIssued = false;

                                    tempS = reader["IS_LOCKED"].ToString();
                                    tempB = false;
                                    if (bool.TryParse(tempS, out tempB))
                                        IsLocked = tempB;
                                    else
                                        IsLocked = false;
                                }
                            }
                        }

                        connection.Close();
                    }

                    Items = InvoiceItem.GetInvoiceItemList(_id, _is_purchase);

                    getData = true;

                    result = getData;
                }
                catch (Exception ex)
                {
                    Dialog.ErrorMessage(null, "Ошибка получения данных", ex.Message);
                }
            }
            else
            {
                Dialog.ErrorMessage(null, "Не указан ID накладной");
            }

            return result;
        }

        /// <summary>
        /// Получить данные по ID
        /// <para>возвращает TRUE, если данные успешно получены</para>
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="isPurchase">TRUE если приходная накладная, FALSE если расходная</param>
        /// <returns>возвращает TRUE, если данные успешно получены</returns>
        public bool GetDateByID(int id, bool isPurchase)
        {
            _id = id;

            return GetDataByID(isPurchase);
        }

        /// <summary>
        /// Получение списка элементов
        /// <paramref name="calc">вычислять стоимость и купон</paramref>
        /// </summary>
        public void GetItems(bool calc = true)
        {
            if (_id != 0)
                Items = InvoiceItem.GetInvoiceItemList(_id, _is_purchase);
        }

        #endregion

        /// <summary>
        /// Создание накладной
        /// </summary>
        /// <returns>возвращает TRUE, если накладная создана успешно</returns>
        public bool CreateInvoice()
        {
            bool result = false;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                {
                    connection.Open();

                    string table = (IsPurchase) ? "PURCHASE_INVOICE" : "SALES_INVOICE";

                    string sql = String.Format("INSERT INTO {0} (`ID_ACCOUNT`, `ID_COUNTERPARTY`, `COST`, `COUPON`) VALUES ('{1}', '{2}', '{3}', '{4}')", table, _id_account, _id_counterparty, _cost, _coupon);
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
                Dialog.ErrorMessage(null, "Ошибка создания накладной", ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Редактировать накладную
        /// </summary>
        /// <returns>возвращает TRUE, если накладная отредактирована успешно</returns>
        public bool EditInvoice()
        {
            bool result = false;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                {
                    connection.Open();

                    string table = (IsPurchase) ? "PURCHASE_INVOICE" : "SALES_INVOICE";

                    string sql = String.Format("UPDATE {0} SET `ID_ACCOUNT` = '{1}', `ID_COUNTERPARTY` = '{2}', `COST` = '{3}', `COUPON` = '{4}' WHERE ID = {5}", table, _id_account, _id_counterparty, _cost, _coupon, _id);
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
                Dialog.ErrorMessage(null, "Ошибка редактирования накладной", ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Удалить накладную
        /// </summary>
        /// <returns>возвращает TRUE, если накладная удалена успешно</returns>
        public bool DeleteInvoice()
        {
            bool result = false;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                {
                    connection.Open();

                    string table = (IsPurchase) ? "PURCHASE_INVOICE" : "SALES_INVOICE";

                    string sql = String.Format("DELETE FROM {0} WHERE ID = {1}", table, _id);
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
                Dialog.ErrorMessage(null, "Ошибка удаления накладной", ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Заблокировать накладную
        /// </summary>
        /// <returns>возвращает TRUE, если накладная заблокирована успешно</returns>
        public bool LockInvoice()
        {
            bool result = false;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                {
                    connection.Open();

                    IsLocked = true;

                    string table = (IsPurchase) ? "PURCHASE_INVOICE" : "SALES_INVOICE";

                    // Блокировка накладной
                    string sql = String.Format("UPDATE {0} SET `IS_LOCKED` = '{1}'  WHERE ID = {2}", table, (IsLocked) ? 1 : 0, _id);
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    
                    // Блокировка товаров
                    foreach(InvoiceItem item in Items)
                    {
                        sql = String.Format("SELECT `LOCKED_COUNT` FROM PRODUCT WHERE ID = {0}", item.ID_Product);

                        double lockCount = 0;

                        using (MySqlCommand command = new MySqlCommand(sql, connection))
                        {
                            using (MySqlDataReader reader = command.ExecuteReader())
                            {
                                while(reader.Read())
                                {
                                    string tempS = reader["LOCKED_COUNT"].ToString();
                                    double tempD = 0;
                                    if (double.TryParse(tempS, out tempD))
                                        lockCount = tempD;
                                    else
                                        lockCount = 0;
                                }
                            }
                        }

                        lockCount += item.Count;

                        sql = String.Format("UPDATE PRODUCT SET `LOCKED_COUNT` = '{0}' WHERE ID = {1}", lockCount, item.ID_Product);

                        using (MySqlCommand command = new MySqlCommand(sql, connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }

                    connection.Close();
                }

                result = true;
            }
            catch (Exception ex)
            {
                Dialog.ErrorMessage(null, "Ошибка блокировки накладной", ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Разблокировать накладную
        /// </summary>
        /// <returns>возвращает TRUE, если накладная разблокирована успешно</returns>
        public bool UnLockInvoice()
        {
            bool result = false;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                {
                    connection.Open();

                    IsLocked = false;

                    string table = (IsPurchase) ? "PURCHASE_INVOICE" : "SALES_INVOICE";

                    // Разблокировка накладной
                    string sql = String.Format("UPDATE {0} SET `IS_LOCKED` = '{1}'  WHERE ID = {2}", table, (IsLocked) ? 1 : 0, _id);
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    // Разблокировка товаров
                    foreach (InvoiceItem item in Items)
                    {
                        sql = String.Format("SELECT `LOCKED_COUNT` FROM PRODUCT WHERE ID = {0}", item.ID_Product);

                        double lockCount = 0;

                        using (MySqlCommand command = new MySqlCommand(sql, connection))
                        {
                            using (MySqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    string tempS = reader["LOCKED_COUNT"].ToString();
                                    double tempD = 0;
                                    if (double.TryParse(tempS, out tempD))
                                        lockCount = tempD;
                                    else
                                        lockCount = 0;
                                }
                            }
                        }

                        lockCount -= item.Count;
                        if (lockCount < 0)
                            lockCount = 0;

                        sql = String.Format("UPDATE PRODUCT SET `LOCKED_COUNT` = '{0}' WHERE ID = {1}", lockCount, item.ID_Product);

                        using (MySqlCommand command = new MySqlCommand(sql, connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }

                    connection.Close();
                }

                result = true;
            }
            catch (Exception ex)
            {
                Dialog.ErrorMessage(null, "Ошибка разблокировки накладной", ex.Message);
            }

            return result;
        }

        #region Статические методы

        /// <summary>
        /// Получить список накладных без элементов
        /// </summary>
        /// <param name="isPurchase">TRUE если приходные накладные, FALSE если расходные</param>
        /// <returns></returns>
        private static List<Invoice> GetInvoices(bool isPurchase, DateTime? from, DateTime? to, int? idAccount, int? idCounterparty)
        {
            List<Invoice> result = new List<Invoice>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                {
                    connection.Open();

                    string table = (isPurchase) ? "PURCHASE_INVOICE" : "SALES_INVOICE";

                    string sql = "SELECT * FROM " + table;

                    if (from != null && to != null)
                    {
                        string fromDate = String.Format(@"'{0}-{1}-{2}T00:00:00.000'", ((DateTime)from).Year, ((DateTime)from).Month, ((DateTime)from).Day);
                        string toDate = String.Format(@"'{0}-{1}-{2}T23:59:59.000'", ((DateTime)to).Year, ((DateTime)to).Month, ((DateTime)to).Day);

                        sql += " WHERE DATE >= " + fromDate + " AND DATE <= " + toDate;

                        if (idAccount != null)
                            sql += " AND ID_ACCOUNT = " + (int)idAccount;

                        if (idCounterparty != null)
                            sql += " AND ID_COUNTERPARTY = " + (int)idCounterparty;
                    }
                    else if(idAccount != null)
                    {
                        sql += " WHERE ID_ACCOUNT = " + (int)idAccount;

                        if (idCounterparty != null)
                            sql += " AND ID_COUNTERPARTY = " + (int)idCounterparty;
                    }
                    else if (idCounterparty != null)
                        sql += " WHERE ID_COUNTERPARTY = " + (int)idCounterparty;

                    MySqlCommand command = new MySqlCommand(sql, connection);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Invoice invoice = new Invoice();

                            string tempS = reader["ID"].ToString();
                            int tempI = 0;
                            if (int.TryParse(tempS, out tempI))
                                invoice.ID = tempI;
                            else
                                invoice.ID = 0;

                            tempS = reader["NUMBER"].ToString();
                            tempI = 0;
                            if (int.TryParse(tempS, out tempI))
                                invoice.Number = tempI;
                            else
                                invoice.Number = 0;

                            tempS = reader["ID_ACCOUNT"].ToString();
                            tempI = 0;
                            if (int.TryParse(tempS, out tempI))
                                invoice.ID_Account = tempI;
                            else
                                invoice.ID_Account = 0;

                            tempS = reader["ID_COUNTERPARTY"].ToString();
                            tempI = 0;
                            if (int.TryParse(tempS, out tempI))
                                invoice.ID_Counterparty = tempI;
                            else
                                invoice.ID_Counterparty = 0;

                            tempS = reader["DATE"].ToString();
                            DateTime tempDT = DateTime.MinValue;
                            if (DateTime.TryParse(tempS, out tempDT))
                                invoice.Date = tempDT;
                            else
                                invoice.Date = DateTime.MinValue;

                            tempS = reader["COST"].ToString();
                            double tempD = 0;
                            if (double.TryParse(tempS, out tempD))
                                invoice.Cost = tempD;
                            else
                                invoice.Cost = 0;

                            tempS = reader["COUPON"].ToString();
                            tempD = 0;
                            if (double.TryParse(tempS, out tempD))
                                invoice.Coupon = tempD;
                            else
                                invoice.Coupon = 0;

                            tempS = reader["IS_ISSUED"].ToString();
                            bool tempB = false;
                            if (bool.TryParse(tempS, out tempB))
                                invoice.IsIssued = tempB;
                            else
                                invoice.IsIssued = false;

                            tempS = reader["IS_LOCKED"].ToString();
                            tempB = false;
                            if (bool.TryParse(tempS, out tempB))
                                invoice.IsLocked = tempB;
                            else
                                invoice.IsLocked = false;

                            invoice.IsPurchase = isPurchase;

                            result.Add(invoice);
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Dialog.ErrorMessage(null, "Ошибка получения данных", ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Получить список приходных накладных БЕЗ элементов накладных
        /// </summary>
        /// <returns></returns>
        public static List<Invoice> GetPurchaseInvoices()
        {
            return GetInvoices(true, null, null, null, null);
        }

        /// <summary>
        /// Получить список приходных накладных БЕЗ элементов накладных
        /// </summary>
        /// <param name="from">начало периода</param>
        /// <param name="to">конец периода</param>
        /// <returns></returns>
        public static List<Invoice> GetPurchaseInvoices(DateTime from, DateTime to)
        {
            return GetInvoices(true, from, to, null, null);
        }

        /// <summary>
        /// Получить список приходных накладных БЕЗ элементов накладных
        /// </summary>
        /// <param name="from">начало периода</param>
        /// <param name="to">конец периода</param>
        /// <param name="idAccount">ID пользователя</param>
        /// <returns></returns>
        public static List<Invoice> GetPurchaseInvoices(DateTime from, DateTime to, int idAccount)
        {
            return GetInvoices(true, from, to, idAccount, null);
        }

        /// <summary>
        /// Получить список приходных накладных БЕЗ элементов накладных
        /// </summary>
        /// <param name="from">начало периода</param>
        /// <param name="to">конец периода</param>
        /// <param name="idAccount">ID пользователя</param>
        /// <param name="idCounterparty">ID контрагента</param>
        /// <returns></returns>
        public static List<Invoice> GetPurchaseInvoices(DateTime from, DateTime to, int? idAccount, int idCounterparty)
        {
            return GetInvoices(true, from, to, idAccount, idCounterparty);
        }

        /// <summary>
        /// Получить список расходных накладных БЕЗ элементов накладных
        /// </summary>
        /// <returns></returns>
        public static List<Invoice> GetSalesInvoices()
        {
            return GetInvoices(false, null, null, null, null);
        }

        /// <summary>
        /// Получить список расходных накладных БЕЗ элементов накладных
        /// </summary>
        /// <param name="from">начало периода</param>
        /// <param name="to">конец периода</param>
        /// <returns></returns>
        public static List<Invoice> GetSalesInvoices(DateTime from, DateTime to)
        {
            return GetInvoices(false, from, to, null, null);
        }

        /// <summary>
        /// Получить список расходных накладных БЕЗ элементов накладных
        /// </summary>
        /// <param name="from">начало периода</param>
        /// <param name="to">конец периода</param>
        /// <param name="idAccount">ID пользователя</param>
        /// <returns></returns>
        public static List<Invoice> GetSalesInvoices(DateTime from, DateTime to, int idAccount)
        {
            return GetInvoices(false, from, to, idAccount, null);
        }

        /// <summary>
        /// Получить список расходных накладных БЕЗ элементов накладных
        /// </summary>
        /// <param name="from">начало периода</param>
        /// <param name="to">конец периода</param>
        /// <param name="idAccount">ID пользователя</param>
        /// <param name="idCounterparty">ID контрагента</param>
        /// <returns></returns>
        public static List<Invoice> GetSalesInvoices(DateTime from, DateTime to, int? idAccount, int idCounterparty)
        {
            return GetInvoices(false, from, to, idAccount, idCounterparty);
        }

        /// <summary>
        /// Создание накладной
        /// </summary>
        /// <param name="isPurchase">приходная накладная</param>
        public static Invoice CreateInvoice(bool isPurchase)
        {
            Invoice invoice = new Invoice();
            invoice.IsPurchase = isPurchase;

            if (invoice.CreateInvoice())
                return invoice;
            else
                return null;
        }

        #endregion
    }
}
