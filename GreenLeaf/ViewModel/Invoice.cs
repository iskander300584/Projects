using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MySql.Data.MySqlClient;
using GreenLeaf.Classes;
using System.Linq;
using GreenLeaf.Constants;

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
                if (_date != value)
                {
                    _date = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime _createDate = DateTime.MinValue;
        /// <summary>
        /// Дата создания накладной
        /// </summary>
        public DateTime CreateDate
        {
            get { return _createDate; }
            set
            {
                if (_createDate != value)
                {
                    _createDate = value;
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
                if (_cost != value)
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
                if (_is_issued != value)
                {
                    _is_issued = value;
                    OnPropertyChanged();

                    GetEditEnabled();
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
                if (_is_locked != value)
                {
                    _is_locked = value;
                    OnPropertyChanged();

                    GetEditEnabled();
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

        private bool _editEnabled = true;
        /// <summary>
        /// Доступность для редактирования
        /// </summary>
        public bool EditEnabled
        {
            get { return _editEnabled; }
            set
            {
                if (_editEnabled != value)
                {
                    _editEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        private Account _account = null;
        /// <summary>
        /// Пользователь
        /// </summary>
        public Account AccountUser
        {
            get { return _account; }
        }

        private Counterparty _counterparty = null;
        /// <summary>
        /// Контрагент
        /// </summary>
        public Counterparty CounterpartyUser
        {
            get { return _counterparty; }
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
                if (_items != value)
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

                    string sql = String.Format(@"INSERT INTO `{0}` (`ID_ACCOUNT`, `ID_COUNTERPARTY`, `COST`, `COUPON`, `CREATE_DATE`) VALUES ('{1}', '{2}', '{3}', '{4}', '{5}')", table, _id_account, _id_counterparty, Conversion.ToString(_cost), Conversion.ToString(_coupon), Conversion.ToString(DateTime.Today));

                    CreateDate = DateTime.Today;

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();

                        ID = (int)command.LastInsertedId;
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
                    using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                    {
                        connection.Open();

                        string table = (isPurchase) ? "PURCHASE_INVOICE" : "SALES_INVOICE";

                        string sql = String.Format(@"SELECT * FROM `{0}` WHERE `{0}`.`ID` = {1}", table, ID);

                        using (MySqlCommand command = new MySqlCommand(sql, connection))
                        {
                            using (MySqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Number = Conversion.ToInt(reader["NUMBER"].ToString());
                                    ID_Account = Conversion.ToInt(reader["ID_ACCOUNT"].ToString());
                                    ID_Counterparty = Conversion.ToInt(reader["ID_COUNTERPARTY"].ToString());
                                    Date = Conversion.ToDateTime(reader["DATE"].ToString());
                                    CreateDate = Conversion.ToDateTime(reader["CREATE_DATE"].ToString());
                                    Cost = Conversion.ToDouble(reader["COST"].ToString());
                                    Coupon = Conversion.ToDouble(reader["COUPON"].ToString());

                                    IsIssued = Conversion.ToBool(reader["IS_ISSUED"].ToString());
                                    IsLocked = Conversion.ToBool(reader["IS_LOCKED"].ToString());
                                }
                            }
                        }

                        Items = InvoiceItem.GetInvoiceItemList(_id, _is_purchase, connection);

                        connection.Close();
                    }

                    result = true;
                }
                catch (Exception ex)
                {
                    Dialog.ErrorMessage(null, "Ошибка получения данных накладной", ex.Message);
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
        public bool GetDataByID(int id, bool isPurchase)
        {
            _id = id;

            return GetDataByID(isPurchase);
        }

        /// <summary>
        /// Получение списка элементов
        /// </summary>
        public void GetItems()
        {
            if (_id != 0)
                Items = InvoiceItem.GetInvoiceItemList(_id, _is_purchase);
        }

        /// <summary>
        /// Вычисление стоимости и купона
        /// </summary>
        public void Calc()
        {
            double cost = 0;
            double coupon = 0;

            foreach (InvoiceItem item in Items)
            {
                cost += item.Cost;
                coupon += item.Coupon;
            }

            Cost = cost;
            Coupon = coupon;
        }

        /// <summary>
        /// Получить список товаров, доступных для добавления
        /// </summary>
        public List<Product> GetPossibleProducts()
        {
            List<Product> products = Product.GetActualProductList();

            for (int i = 0; i < products.Count;)
            {
                if (Items.Any(item => item.ID == products[i].ID))
                    products.Remove(products[i]);
                else
                    i++;
            }

            return products;
        }

        /// <summary>
        /// Получение пользователя и контрагента
        /// </summary>
        /// <returns>Возвращает TRUE, если какие-либо данные были получены</returns>
        public bool GetUsers()
        {
            bool result = false;

            if (_id_account != 0)
            {
                try
                {
                    _account = Account.GetAccountByID(_id_account);
                    result = true;

                    OnPropertyChanged("AccountUser");
                }
                catch { }
            }

            if(_id_counterparty != 0)
            {
                try
                {
                    _counterparty = Counterparty.GetCounterpartyByID(_id_counterparty);
                    result = true;

                    OnPropertyChanged("CounterpartyUser");
                }
                catch { }
            }

            return result;
        }

        #endregion

        #region Редактирование данных 

        /// <summary>
        /// Редактировать накладную
        /// </summary>
        /// <returns>возвращает TRUE, если накладная отредактирована успешно</returns>
        public bool EditInvoice()
        {
            bool result = false;

            if (_id != 0)
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                    {
                        connection.Open();

                        string table = (IsPurchase) ? "PURCHASE_INVOICE" : "SALES_INVOICE";

                        string sql = String.Format(@"UPDATE `{0}` SET `ID_ACCOUNT` = '{1}', `ID_COUNTERPARTY` = '{2}', `COST` = '{3}', `COUPON` = '{4}' WHERE `{0}`.`ID` = {5}", table, _id_account, _id_counterparty, Conversion.ToString(_cost), Conversion.ToString(_coupon), _id);

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
            }
            else
            {
                Dialog.ErrorMessage(null, "Не указан ID накладной");
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

            if (_id != 0)
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                    {
                        connection.Open();

                        string table = (IsPurchase) ? "PURCHASE_INVOICE" : "SALES_INVOICE";

                        string sql = String.Format(@"DELETE FROM `{0}` WHERE `{0}`.`ID` = {1}", table, _id);

                        using (MySqlCommand command = new MySqlCommand(sql, connection))
                        {
                            command.ExecuteNonQuery();

                            // Удаление элементов накладной
                            table = (IsPurchase) ? "PURCHASE_INVOICE_UNIT" : "SALES_INVOICE_UNIT";

                            sql = String.Format(@"DELETE FROM `{0}` WHERE `{0}`.`ID_Invoice` = {1}", table, ID);

                            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                            {
                                cmd.ExecuteNonQuery();
                            }

                            //foreach (InvoiceItem item in Items)
                            //{
                            //    sql = String.Format(@"DELETE FROM `{0}` WHERE `{0}`.`ID` = {1}", table, item.ID);

                            //    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                            //    {
                            //        cmd.ExecuteNonQuery();
                            //    }
                            //}
                        }

                        connection.Close();
                    }

                    result = true;
                }
                catch (Exception ex)
                {
                    Dialog.ErrorMessage(null, "Ошибка удаления накладной", ex.Message);
                }
            }
            else
            {
                Dialog.ErrorMessage(null, "Не указан ID накладной");
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

            if (_id != 0)
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                    {
                        connection.Open();

                        IsLocked = true;

                        string table = (IsPurchase) ? "PURCHASE_INVOICE" : "SALES_INVOICE";

                        // Блокировка накладной
                        string sql = String.Format(@"UPDATE `{0}` SET `IS_LOCKED` = '{1}' WHERE `{0}`.`ID` = {2}", table, (IsLocked) ? 1 : 0, _id);
                        using (MySqlCommand command = new MySqlCommand(sql, connection))
                        {
                            command.ExecuteNonQuery();
                        }

                        // Блокировка товаров
                        if (!IsPurchase)
                        {
                            foreach (InvoiceItem item in Items)
                            {
                                sql = String.Format(@"SELECT `LOCKED_COUNT` FROM `PRODUCT` WHERE `PRODUCT`.`ID` = {0}", item.ID_Product);

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

                                lockCount += item.Count;

                                sql = String.Format(@"UPDATE `PRODUCT` SET `LOCKED_COUNT` = '{0}' WHERE `PRODUCT`.`ID` = {1}", lockCount, item.ID_Product);

                                using (MySqlCommand command = new MySqlCommand(sql, connection))
                                {
                                    command.ExecuteNonQuery();
                                }
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
            }
            else
            {
                Dialog.ErrorMessage(null, "Не указан ID накладной");
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

            if (_id != 0)
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                    {
                        connection.Open();

                        IsLocked = false;

                        string table = (IsPurchase) ? "PURCHASE_INVOICE" : "SALES_INVOICE";

                        // Разблокировка накладной
                        string sql = String.Format(@"UPDATE `{0}` SET `IS_LOCKED` = '{1}' WHERE `{0}`.`ID` = {2}", table, (IsLocked) ? 1 : 0, _id);
                        using (MySqlCommand command = new MySqlCommand(sql, connection))
                        {
                            command.ExecuteNonQuery();
                        }

                        // Разблокировка товаров
                        if (!IsPurchase)
                        {
                            foreach (InvoiceItem item in Items)
                            {
                                sql = String.Format(@"SELECT `LOCKED_COUNT` FROM `PRODUCT` WHERE `PRODUCT`.`ID` = {0}", item.ID_Product);

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

                                sql = String.Format(@"UPDATE `PRODUCT` SET `LOCKED_COUNT` = '{0}' WHERE `PRODUCT`.`ID` = {1}", lockCount, item.ID_Product);

                                using (MySqlCommand command = new MySqlCommand(sql, connection))
                                {
                                    command.ExecuteNonQuery();
                                }
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
            }
            else
            {
                Dialog.ErrorMessage(null, "Не указан ID накладной");
            }

            return result;
        }

        /// <summary>
        /// Провести накладную
        /// </summary>
        /// <returns>возвращает TRUE, если накладная проведена успешно</returns>
        public bool IssueInvoice()
        {
            bool result = false;

            if (_id != 0)
            {
                result = EditInvoice();

                if (result)
                {
                    try
                    {
                        using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                        {
                            connection.Open();

                            string sql = string.Empty;

                            if (Number == 0)
                            {
                                // Получение номера
                                string nomination = string.Empty;
                                if (IsPurchase)
                                    nomination = NumeratorNames.PurchaseInvoice;
                                else
                                    nomination = NumeratorNames.SalesInvoice;

                                sql = String.Format(@"SELECT `VALUE` FROM `NUMERATOR` WHERE `NUMERATOR`.`NOMINATION` = '{0}'", nomination);

                                // Получение значения нумератора
                                using (MySqlCommand command = new MySqlCommand(sql, connection))
                                {
                                    using (MySqlDataReader reader = command.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            string tempS = reader["VALUE"].ToString();
                                            int tempI = 0;
                                            if (int.TryParse(tempS, out tempI))
                                                Number = tempI + 1;
                                            else
                                                Number = 1;
                                        }
                                    }
                                }

                                // Обновление значения нумератора
                                sql = String.Format(@"UPDATE `NUMERATOR` SET `VALUE` = '{0}' WHERE `NUMERATOR`.`NOMINATION` = '{1}'", Number, nomination);
                                using (MySqlCommand command = new MySqlCommand(sql, connection))
                                {
                                    command.ExecuteNonQuery();
                                }
                            }

                            IsIssued = true;

                            string table = (IsPurchase) ? "PURCHASE_INVOICE" : "SALES_INVOICE";

                            // Проведение накладной
                            sql = String.Format(@"UPDATE `{0}` SET `NUMBER` = '{1}', `IS_ISSUED` = '{2}', `DATE` = '{3}' WHERE `{0}`.`ID` = {4}", table, Number, Conversion.ToString(IsIssued), Conversion.ToString(DateTime.Today), _id);

                            using (MySqlCommand command = new MySqlCommand(sql, connection))
                            {
                                command.ExecuteNonQuery();
                            }

                            Date = DateTime.Today;

                            // Добавление/уменьшение товаров
                            foreach (InvoiceItem item in Items)
                            {
                                sql = String.Format(@"SELECT `COUNT` FROM `PRODUCT` WHERE `PRODUCT`.`ID` = {0}", item.ID_Product);

                                double count = 0;

                                using (MySqlCommand command = new MySqlCommand(sql, connection))
                                {
                                    using (MySqlDataReader reader = command.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            string tempS = reader["COUNT"].ToString();
                                            double tempD = 0;
                                            if (double.TryParse(tempS, out tempD))
                                                count = tempD;
                                            else
                                                count = 0;
                                        }
                                    }
                                }

                                // Изменение количества товара
                                if (IsPurchase)
                                    count += item.Count;
                                else
                                    count -= item.Count;

                                sql = String.Format(@"UPDATE `PRODUCT` SET `COUNT` = '{0}' WHERE `PRODUCT`.`ID` = {1}", count, item.ID_Product);

                                using (MySqlCommand command = new MySqlCommand(sql, connection))
                                {
                                    command.ExecuteNonQuery();
                                }
                            }

                            string name = (IsPurchase) ? "приходную накладную " : "расходную накладную ";

                            Journal.CreateJournal("провел", name + Number.ToString(), connection);

                            connection.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        result = false;
                        Dialog.ErrorMessage(null, "Ошибка проведения накладной", ex.Message);
                    }
                }
            }
            else
            {
                Dialog.ErrorMessage(null, "Не указан ID накладной");
            }

            return result;
        }

        /// <summary>
        /// Отменить проведение накладной
        /// </summary>
        /// <returns>возвращает TRUE, если отмена проведения накладной выполнена успешно</returns>
        public bool UnIssueInvoice()
        {
            bool result = false;

            if (_id != 0)
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                    {
                        connection.Open();

                        string sql = string.Empty;

                        IsIssued = false;
                        Date = DateTime.MinValue;

                        string table = (IsPurchase) ? "PURCHASE_INVOICE" : "SALES_INVOICE";

                        // Проведение накладной
                        sql = String.Format(@"UPDATE `{0}` SET `IS_ISSUED` = '{1}', `DATE` = '{2}' WHERE `{0}`.`ID` = {3}", table, Conversion.ToString(IsIssued), Conversion.ToString(Date), _id);

                        using (MySqlCommand command = new MySqlCommand(sql, connection))
                        {
                            command.ExecuteNonQuery();
                        }

                        // Добавление/уменьшение товаров
                        foreach (InvoiceItem item in Items)
                        {
                            sql = String.Format(@"SELECT `COUNT` FROM `PRODUCT` WHERE `PRODUCT`.`ID` = {0}", item.ID_Product);

                            double count = 0;

                            using (MySqlCommand command = new MySqlCommand(sql, connection))
                            {
                                using (MySqlDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        string tempS = reader["COUNT"].ToString();
                                        double tempD = 0;
                                        if (double.TryParse(tempS, out tempD))
                                            count = tempD;
                                        else
                                            count = 0;
                                    }
                                }
                            }

                            // Изменение количества товара
                            if (IsPurchase)
                                count -= item.Count;
                            else
                                count += item.Count;

                            sql = String.Format(@"UPDATE `PRODUCT` SET `COUNT` = '{0}' WHERE `PRODUCT`.`ID` = {1}", count, item.ID_Product);

                            using (MySqlCommand command = new MySqlCommand(sql, connection))
                            {
                                command.ExecuteNonQuery();
                            }
                        }

                        string name = (IsPurchase) ? "проведение приходной накладной " : "проведение расходной накладной ";

                        Journal.CreateJournal("отменил", name + Number.ToString(), connection);

                        connection.Close();
                    }
                }
                catch (Exception ex)
                {
                    result = false;
                    Dialog.ErrorMessage(null, "Ошибка отмены проведения накладной", ex.Message);
                }
            }
            else
            {
                Dialog.ErrorMessage(null, "Не указан ID накладной");
            }

            return result;
        }

        /// <summary>
        /// Получение доступности для редактирования
        /// </summary>
        private void GetEditEnabled()
        {
            EditEnabled = !(IsLocked || IsIssued);
        }

        #endregion

        #region Статические методы

        /// <summary>
        /// Получить список накладных БЕЗ элементов накладных
        /// </summary>
        /// <param name="isPurchase">TRUE если приходные накладные, FALSE если расходные</param>
        /// <returns></returns>
        public static List<Invoice> GetInvoices(bool isPurchase, DateTime? from, DateTime? to, int? idAccount, int? idCounterparty)
        {
            List<Invoice> result = new List<Invoice>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                {
                    connection.Open();

                    string table = (isPurchase) ? "PURCHASE_INVOICE" : "SALES_INVOICE";

                    string sql = "SELECT * FROM `" + table + "`";

                    if (from != null && to != null)
                    {
                        string fromDate = String.Format(@"'{0}-{1}-{2}T00:00:00.000'", ((DateTime)from).Year, ((DateTime)from).Month, ((DateTime)from).Day);
                        string toDate = String.Format(@"'{0}-{1}-{2}T23:59:59.000'", ((DateTime)to).Year, ((DateTime)to).Month, ((DateTime)to).Day);

                        sql += " WHERE `" + table + "`.`CREATE_DATE` >= " + fromDate + " AND `" + table + "`.`CREATE_DATE` <= " + toDate;

                        if (idAccount != null && idAccount != 0)
                            sql += " AND `" + table + "`.`ID_ACCOUNT` = " + (int)idAccount;

                        if (idCounterparty != null && idCounterparty != 0)
                            sql += " AND `" + table + "`.`ID_COUNTERPARTY` = " + (int)idCounterparty;
                    }
                    else if (idAccount != null && idAccount != 0)
                    {
                        sql += " WHERE `" + table + "`.`ID_ACCOUNT` = " + (int)idAccount;

                        if (idCounterparty != null && idCounterparty != 0)
                            sql += " AND `" + table + "`.`ID_COUNTERPARTY` = " + (int)idCounterparty;
                    }
                    else if (idCounterparty != null && idCounterparty != 0)
                        sql += " WHERE `" + table + "`.`ID_COUNTERPARTY` = " + (int)idCounterparty;

                    MySqlCommand command = new MySqlCommand(sql, connection);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Invoice invoice = new Invoice();

                            invoice.ID = Conversion.ToInt(reader["ID"].ToString());
                            invoice.Number = Conversion.ToInt(reader["NUMBER"].ToString());
                            invoice.ID_Account = Conversion.ToInt(reader["ID_ACCOUNT"].ToString());
                            invoice.ID_Counterparty = Conversion.ToInt(reader["ID_COUNTERPARTY"].ToString());
                            invoice.Date = Conversion.ToDateTime(reader["DATE"].ToString());
                            invoice.CreateDate = Conversion.ToDateTime(reader["CREATE_DATE"].ToString());
                            invoice.Cost = Conversion.ToDouble(reader["COST"].ToString());
                            invoice.Coupon = Conversion.ToDouble(reader["COUPON"].ToString());
                            invoice.IsIssued = Conversion.ToBool(reader["IS_ISSUED"].ToString());
                            invoice.IsLocked = Conversion.ToBool(reader["IS_LOCKED"].ToString());

                            invoice.IsPurchase = isPurchase;

                            result.Add(invoice);
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Dialog.ErrorMessage(null, "Ошибка получения списка накладных", ex.Message);
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
            invoice.ID_Account = ConnectSetting.CurrentUser.ID;

            if (invoice.CreateInvoice())
                return invoice;
            else
                return null;
        }

        /// <summary>
        /// Получение накладной
        /// </summary>
        /// <param name="isPurchase">признак приходной накладной</param>
        /// <param name="id">ID</param>
        public static Invoice GetInvoiceByID(bool isPurchase, int id)
        {
            Invoice invoice = new Invoice();

            invoice.GetDataByID(id, isPurchase);

            return invoice;
        }

        #endregion
    }
}