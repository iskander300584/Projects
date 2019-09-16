using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MySql.Data.MySqlClient;
using GreenLeaf.Classes;
using GreenLeaf.Classes.Account;
using System.Collections.Generic;

namespace GreenLeaf.ViewModel
{
    /// <summary>
    /// Аккаунт пользователя
    /// </summary>
    public class Account : INotifyPropertyChanged
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

        private string _login = string.Empty;
        /// <summary>
        /// Логин пользователя
        /// </summary>
        public string Login
        {
            get { return _login; }
            set
            {
                if (_login != value)
                {
                    _login = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _password = string.Empty;
        /// <summary>
        /// Зашифрованный пароль пользователя
        /// </summary>
        public string Password
        {
            get { return _password; }
            set
            {
                if(_password != value)
                {
                    _password = value;
                    OnPropertyChanged();
                }
            }
        }

        private PersonalData _personalData = new PersonalData();
        /// <summary>
        /// Персональные данные пользователя
        /// </summary>
        public PersonalData PersonalData
        {
            get { return _personalData; }
        }

        private InvoiceData _invoiceData = new InvoiceData();
        /// <summary>
        /// Данные о доступе к накладным
        /// </summary>
        public InvoiceData InvoiceData
        {
            get { return _invoiceData; }
        }

        private ReportsData _reportsData = new ReportsData();
        /// <summary>
        /// Данные о доступе к отчетам
        /// </summary>
        public ReportsData ReportsData
        {
            get { return _reportsData; }
        }

        private CounterpartyData _counterpartyData = new CounterpartyData();
        /// <summary>
        /// Данные о доступе к контрагентам
        /// </summary>
        public CounterpartyData CounterpartyData
        {
            get { return _counterpartyData; }
        }

        private WarehouseData _warehouseData = new WarehouseData();
        /// <summary>
        /// Данные о доступе к управлению складом
        /// </summary>
        public WarehouseData WarehouseData
        {
            get { return _warehouseData; }
        }

        private AdminPanelData _adminPanelData = new AdminPanelData();
        /// <summary>
        /// Данные о доступе к административной панели
        /// </summary>
        public AdminPanelData AdminPanelData
        {
            get { return _adminPanelData; }
        }

        private bool _isAnnulated = false;
        /// <summary>
        /// Признак аннулированного аккаунта
        /// </summary>
        public bool IsAnnulated
        {
            get { return _isAnnulated; }
            set
            {
                if (_isAnnulated != value)
                {
                    _isAnnulated = value;
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
                    bool getData = false;

                    using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                    {
                        connection.Open();

                        string sql = "SELECT * FROM ACCOUNT WHERE ID = " + ID.ToString();
                        MySqlCommand command = new MySqlCommand(sql, connection);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Login = reader["LOGIN"].ToString();
                                Password = reader["PASSWORD"].ToString();
                                PersonalData.Surname = reader["SURNAME"].ToString();
                                PersonalData.Name = reader["SURNAME"].ToString();
                                PersonalData.Patronymic = reader["PATRONYMIC"].ToString();

                                string tempS = reader["ADRESS"].ToString();
                                if (tempS != "")
                                    PersonalData.Adress = Criptex.UnCript(tempS);

                                tempS = reader["PHONE"].ToString();
                                if (tempS != "")
                                    PersonalData.Phone = Criptex.UnCript(tempS);

                                tempS = reader["SEX"].ToString();
                                bool tempB = false;
                                if (bool.TryParse(tempS, out tempB))
                                    PersonalData.Sex = tempB;
                                else
                                    PersonalData.Sex = false;

                                tempS = reader["PURCHASE_INVOICE"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    InvoiceData.PurchaseInvoice = tempB;
                                else
                                    InvoiceData.PurchaseInvoice = false;

                                tempS = reader["SALES_INVOICE"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    InvoiceData.SalesInvoice = tempB;
                                else
                                    InvoiceData.SalesInvoice = false;

                                tempS = reader["REPORTS"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    ReportsData.Reports = tempB;
                                else
                                    ReportsData.Reports = false;

                                tempS = reader["REPORT_PURCHASE_INVOICE"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    ReportsData.ReportPurchaseInvoice = tempB;
                                else
                                    ReportsData.ReportPurchaseInvoice = false;

                                tempS = reader["REPORT_SALES_INVOICE"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    ReportsData.ReportSalesInvoice = tempB;
                                else
                                    ReportsData.ReportSalesInvoice = false;

                                tempS = reader["REPORT_INCOME_EXPENSE"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    ReportsData.ReportIncomeExpense = tempB;
                                else
                                    ReportsData.ReportIncomeExpense = false;

                                tempS = reader["COUNTERPARTY"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    CounterpartyData.Counterparty = tempB;
                                else
                                    CounterpartyData.Counterparty = false;

                                tempS = reader["COUNTERPARTY_PROVIDER"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    CounterpartyData.CounterpartyProvider = tempB;
                                else
                                    CounterpartyData.CounterpartyProvider = false;

                                tempS = reader["COUNTERPARTY_PROVIDER_ADD"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    CounterpartyData.CounterpartyProviderAdd = tempB;
                                else
                                    CounterpartyData.CounterpartyProviderAdd = false;

                                tempS = reader["COUNTERPARTY_PROVIDER_EDIT"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    CounterpartyData.CounterpartyProviderEdit = tempB;
                                else
                                    CounterpartyData.CounterpartyProviderEdit = false;

                                tempS = reader["COUNTERPARTY_PROVIDER_DELETE"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    CounterpartyData.CounterpartyProviderDelete = tempB;
                                else
                                    CounterpartyData.CounterpartyProviderDelete = false;

                                tempS = reader["COUNTERPARTY_CUSTOMER"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    CounterpartyData.CounterpartyCustomer = tempB;
                                else
                                    CounterpartyData.CounterpartyCustomer = false;

                                tempS = reader["COUNTERPARTY_CUSTOMER_ADD"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    CounterpartyData.CounterpartyCustomerAdd = tempB;
                                else
                                    CounterpartyData.CounterpartyCustomerAdd = false;

                                tempS = reader["COUNTERPARTY_CUSTOMER_EDIT"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    CounterpartyData.CounterpartyCustomerEdit = tempB;
                                else
                                    CounterpartyData.CounterpartyCustomerEdit = false;

                                tempS = reader["COUNTERPARTY_CUSTOMER_DELETE"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    CounterpartyData.CounterpartyCustomerDelete = tempB;
                                else
                                    CounterpartyData.CounterpartyCustomerDelete = false;

                                tempS = reader["WAREHOUSE"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    WarehouseData.Warehouse = tempB;
                                else
                                    WarehouseData.Warehouse = false;

                                tempS = reader["WAREHOUSE_ADD_PRODUCT"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    WarehouseData.WarehouseAddProduct = tempB;
                                else
                                    WarehouseData.WarehouseAddProduct = false;

                                tempS = reader["WAREHOUSE_EDIT_PRODUCT"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    WarehouseData.WarehouseEditProduct = tempB;
                                else
                                    WarehouseData.WarehouseEditProduct = false;

                                tempS = reader["WAREHOUSE_ANNULATE_PRODUCT"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    WarehouseData.WarehouseAnnulateProduct = tempB;
                                else
                                    WarehouseData.WarehouseAnnulateProduct = false;

                                tempS = reader["WAREHOUSE_EDIT_COUNT"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    WarehouseData.WarehouseEditCount = tempB;
                                else
                                    WarehouseData.WarehouseEditCount = false;

                                tempS = reader["ADMIN_PANEL"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    AdminPanelData.AdminPanel = tempB;
                                else
                                    AdminPanelData.AdminPanel = false;

                                tempS = reader["ADMIN_PANEL_ADD_ACCOUNT"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    AdminPanelData.AdminPanelAddAccount = tempB;
                                else
                                    AdminPanelData.AdminPanelAddAccount = false;

                                tempS = reader["ADMIN_PANEL_EDIT_ACCOUNT"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    AdminPanelData.AdminPanelEditAccount = tempB;
                                else
                                    AdminPanelData.AdminPanelEditAccount = false;

                                tempS = reader["ADMIN_PANEL_DELETE_ACCOUNT"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    AdminPanelData.AdminPanelDeleteAccount = tempB;
                                else
                                    AdminPanelData.AdminPanelDeleteAccount = false;

                                tempS = reader["ADMIN_PANEL_SET_NUMERATOR"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    AdminPanelData.AdminPanelSetNumerator = tempB;
                                else
                                    AdminPanelData.AdminPanelSetNumerator = false;

                                tempS = reader["ADMIN_PANEL_JOURNAL"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    AdminPanelData.AdminPanelJournal = tempB;
                                else
                                    AdminPanelData.AdminPanelJournal = false;

                                tempS = reader["IS_ANNULATED"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    IsAnnulated = tempB;
                                else
                                    IsAnnulated = false;

                                getData = true;
                            }
                        }

                        connection.Close();
                    }

                    result = getData;
                }
                catch (Exception ex)
                {
                    Dialog.ErrorMessage(null, "Ошибка получения данных о пользователе", ex.Message);
                }
            }
            else
            {
                Dialog.ErrorMessage(null, "Не указан ID пользователя");
            }

            return result;
        }

        /// <summary>
        /// Получить все данные по ID
        /// <para>возвращает TRUE, если данные успешно получены</para>
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>возвращает TRUE, если данные успешно получены</returns>
        public bool GetFullDateByID(int id)
        {
            _id = id;

            return GetFullDataByID();
        }

        /// <summary>
        /// Получить не защищаемые данные по ID
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
                    bool getData = false;

                    using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                    {
                        connection.Open();

                        string sql = "SELECT * FROM ACCOUNT WHERE ID = " + ID.ToString();
                        MySqlCommand command = new MySqlCommand(sql, connection);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Login = reader["LOGIN"].ToString();
                                Password = reader["PASSWORD"].ToString();
                                PersonalData.Surname = reader["SURNAME"].ToString();
                                PersonalData.Name = reader["SURNAME"].ToString();
                                PersonalData.Patronymic = reader["PATRONYMIC"].ToString();

                                PersonalData.Adress = string.Empty;
                                PersonalData.Phone = string.Empty;

                                string tempS = reader["SEX"].ToString();
                                bool tempB = false;
                                if (bool.TryParse(tempS, out tempB))
                                    PersonalData.Sex = tempB;
                                else
                                    PersonalData.Sex = false;

                                tempS = reader["PURCHASE_INVOICE"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    InvoiceData.PurchaseInvoice = tempB;
                                else
                                    InvoiceData.PurchaseInvoice = false;

                                tempS = reader["SALES_INVOICE"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    InvoiceData.SalesInvoice = tempB;
                                else
                                    InvoiceData.SalesInvoice = false;

                                tempS = reader["REPORTS"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    ReportsData.Reports = tempB;
                                else
                                    ReportsData.Reports = false;

                                tempS = reader["REPORT_PURCHASE_INVOICE"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    ReportsData.ReportPurchaseInvoice = tempB;
                                else
                                    ReportsData.ReportPurchaseInvoice = false;

                                tempS = reader["REPORT_SALES_INVOICE"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    ReportsData.ReportSalesInvoice = tempB;
                                else
                                    ReportsData.ReportSalesInvoice = false;

                                tempS = reader["REPORT_INCOME_EXPENSE"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    ReportsData.ReportIncomeExpense = tempB;
                                else
                                    ReportsData.ReportIncomeExpense = false;

                                tempS = reader["COUNTERPARTY"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    CounterpartyData.Counterparty = tempB;
                                else
                                    CounterpartyData.Counterparty = false;

                                tempS = reader["COUNTERPARTY_PROVIDER"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    CounterpartyData.CounterpartyProvider = tempB;
                                else
                                    CounterpartyData.CounterpartyProvider = false;

                                tempS = reader["COUNTERPARTY_PROVIDER_ADD"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    CounterpartyData.CounterpartyProviderAdd = tempB;
                                else
                                    CounterpartyData.CounterpartyProviderAdd = false;

                                tempS = reader["COUNTERPARTY_PROVIDER_EDIT"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    CounterpartyData.CounterpartyProviderEdit = tempB;
                                else
                                    CounterpartyData.CounterpartyProviderEdit = false;

                                tempS = reader["COUNTERPARTY_PROVIDER_DELETE"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    CounterpartyData.CounterpartyProviderDelete = tempB;
                                else
                                    CounterpartyData.CounterpartyProviderDelete = false;

                                tempS = reader["COUNTERPARTY_CUSTOMER"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    CounterpartyData.CounterpartyCustomer = tempB;
                                else
                                    CounterpartyData.CounterpartyCustomer = false;

                                tempS = reader["COUNTERPARTY_CUSTOMER_ADD"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    CounterpartyData.CounterpartyCustomerAdd = tempB;
                                else
                                    CounterpartyData.CounterpartyCustomerAdd = false;

                                tempS = reader["COUNTERPARTY_CUSTOMER_EDIT"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    CounterpartyData.CounterpartyCustomerEdit = tempB;
                                else
                                    CounterpartyData.CounterpartyCustomerEdit = false;

                                tempS = reader["COUNTERPARTY_CUSTOMER_DELETE"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    CounterpartyData.CounterpartyCustomerDelete = tempB;
                                else
                                    CounterpartyData.CounterpartyCustomerDelete = false;

                                tempS = reader["WAREHOUSE"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    WarehouseData.Warehouse = tempB;
                                else
                                    WarehouseData.Warehouse = false;

                                tempS = reader["WAREHOUSE_ADD_PRODUCT"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    WarehouseData.WarehouseAddProduct = tempB;
                                else
                                    WarehouseData.WarehouseAddProduct = false;

                                tempS = reader["WAREHOUSE_EDIT_PRODUCT"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    WarehouseData.WarehouseEditProduct = tempB;
                                else
                                    WarehouseData.WarehouseEditProduct = false;

                                tempS = reader["WAREHOUSE_ANNULATE_PRODUCT"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    WarehouseData.WarehouseAnnulateProduct = tempB;
                                else
                                    WarehouseData.WarehouseAnnulateProduct = false;

                                tempS = reader["WAREHOUSE_EDIT_COUNT"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    WarehouseData.WarehouseEditCount = tempB;
                                else
                                    WarehouseData.WarehouseEditCount = false;

                                tempS = reader["ADMIN_PANEL"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    AdminPanelData.AdminPanel = tempB;
                                else
                                    AdminPanelData.AdminPanel = false;

                                tempS = reader["ADMIN_PANEL_ADD_ACCOUNT"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    AdminPanelData.AdminPanelAddAccount = tempB;
                                else
                                    AdminPanelData.AdminPanelAddAccount = false;

                                tempS = reader["ADMIN_PANEL_EDIT_ACCOUNT"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    AdminPanelData.AdminPanelEditAccount = tempB;
                                else
                                    AdminPanelData.AdminPanelEditAccount = false;

                                tempS = reader["ADMIN_PANEL_DELETE_ACCOUNT"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    AdminPanelData.AdminPanelDeleteAccount = tempB;
                                else
                                    AdminPanelData.AdminPanelDeleteAccount = false;

                                tempS = reader["ADMIN_PANEL_SET_NUMERATOR"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    AdminPanelData.AdminPanelSetNumerator = tempB;
                                else
                                    AdminPanelData.AdminPanelSetNumerator = false;

                                tempS = reader["ADMIN_PANEL_JOURNAL"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    AdminPanelData.AdminPanelJournal = tempB;
                                else
                                    AdminPanelData.AdminPanelJournal = false;

                                tempS = reader["IS_ANNULATED"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    IsAnnulated = tempB;
                                else
                                    IsAnnulated = false;

                                getData = true;
                            }
                        }

                        connection.Close();
                    }

                    result = getData;
                }
                catch (Exception ex)
                {
                    Dialog.ErrorMessage(null, "Ошибка получения данных о пользователе", ex.Message);
                }
            }
            else
            {
                Dialog.ErrorMessage(null, "Не указан ID пользователя");
            }

            return result;
        }

        /// <summary>
        /// Получить не защищаемые данные по ID
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
        /// Получить защищаемые данные по ID
        /// </summary>
        /// <returns>возвращает TRUE, если данные успешно получены</returns>
        public bool GetProtectedDataByID()
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

                        string sql = "SELECT `ADRESS`, `PHONE` FROM ACCOUNT WHERE ID = " + ID.ToString();
                        MySqlCommand command = new MySqlCommand(sql, connection);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string tempS = reader["ADRESS"].ToString();
                                if (tempS != "")
                                    PersonalData.Adress = Criptex.UnCript(tempS);

                                tempS = reader["PHONE"].ToString();
                                if (tempS != "")
                                    PersonalData.Phone = Criptex.UnCript(tempS);

                                getData = true;
                            }
                        }

                        connection.Close();
                    }

                    result = getData;
                }
                catch (Exception ex)
                {
                    Dialog.ErrorMessage(null, "Ошибка получения защищаемых данных о пользователе", ex.Message);
                }
            }
            else
            {
                Dialog.ErrorMessage(null, "Не указан ID пользователя");
            }

            return result;
        }

        /// <summary>
        /// Получить базовые данные по логину
        /// </summary>
        /// <returns>возвращает TRUE, если данные успешно получены</returns>
        public bool GetBaseDataByLogin()
        {
            bool result = false;

            try
            {
                bool getData = false;

                using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                {
                    connection.Open();

                    string sql = "SELECT `ID`, `PASSWORD`, `IS_ANNULATED` FROM ACCOUNT WHERE LOGIN = \'" + Login.ToString() + "\'";
                    MySqlCommand command = new MySqlCommand(sql, connection);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string tempS = reader["ID"].ToString();
                            int tempI = 0;
                            if (int.TryParse(tempS, out tempI))
                                ID = tempI;
                            else
                                ID = 0;

                            Password = reader["PASSWORD"].ToString();

                            tempS = reader["IS_ANNULATED"].ToString();
                            bool tempB = false;
                            if (bool.TryParse(tempS, out tempB))
                                IsAnnulated = tempB;
                            else
                                IsAnnulated = false;

                            getData = true;
                        }
                    }

                    connection.Close();
                }

                result = getData;
            }
            catch (Exception ex)
            {
                Dialog.ErrorMessage(null, "Ошибка получения данных по логину пользователя", ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Получить базовые данные по логину
        /// </summary>
        /// <param name="login">логин</param>
        /// <returns>возвращает TRUE, если данные успешно получены</returns>
        public bool GetBaseDataByLogin(string login)
        {
            _login = login;

            return GetBaseDataByLogin();
        }

        #endregion

        /// <summary>
        /// Создать новый аккаунт
        /// </summary>
        /// <returns>возвращает TRUE, если аккаунт успешно создан</returns>
        public bool CreateAccount()
        {
            bool result = false;

            try
            {
                bool getData = false;

                using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                {
                    connection.Open();

                    string newPass = Criptex.Cript("12345");

                    string sql = String.Format(@"INSERT INTO ACCOUNT (`LOGIN`, `PASSWORD`, `SURNAME`, `NAME`, `PATRONYMIC`, `ADRESS`, `PHONE`, `SEX`, `PURCHASE_INVOICE`, `SALES_INVOICE`, `REPORTS`, `REPORT_PURCHASE_INVOICE`, `REPORT_SALES_INVOICE`, `REPORT_INCOME_EXPENSE`, `COUNTERPARTY`, `COUNTERPARTY_PROVIDER`, `COUNTERPARTY_PROVIDER_ADD`, `COUNTERPARTY_PROVIDER_EDIT`, `COUNTERPARTY_PROVIDER_DELETE`, `COUNTERPARTY_CUSTOMER`, `COUNTERPARTY_CUSTOMER_ADD`, `COUNTERPARTY_CUSTOMER_EDIT`, `COUNTERPARTY_CUSTOMER_DELETE`, `WAREHOUSE`, `WAREHOUSE_ADD_PRODUCT`, `WAREHOUSE_EDIT_PRODUCT`, `WAREHOUSE_ANNULATE_PRODUCT`, `WAREHOUSE_EDIT_COUNT`, `ADMIN_PANEL`, `ADMIN_PANEL_ADD_ACCOUNT`, `ADMIN_PANEL_EDIT_ACCOUNT`, `ADMIN_PANEL_DELETE_ACCOUNT`, `ADMIN_PANEL_SET_NUMERATOR`, `ADMIN_PANEL_JOURNAL`, `IS_ANNULATED`)  VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}', '{30}', '{31}', '{32}', '{33}', '{34}')", Login, newPass, PersonalData.Surname, PersonalData.Name, PersonalData.Patronymic, PersonalData.Adress, PersonalData.Phone, ToInt(PersonalData.Sex), ToInt(InvoiceData.PurchaseInvoice), ToInt(InvoiceData.SalesInvoice), ToInt(ReportsData.Reports), ToInt(ReportsData.ReportPurchaseInvoice), ToInt(ReportsData.ReportSalesInvoice), ToInt(ReportsData.ReportIncomeExpense), ToInt(CounterpartyData.Counterparty), ToInt(CounterpartyData.CounterpartyProvider), ToInt(CounterpartyData.CounterpartyProviderAdd), ToInt(CounterpartyData.CounterpartyProviderEdit), ToInt(CounterpartyData.CounterpartyProviderDelete), ToInt(CounterpartyData.CounterpartyCustomer), ToInt(CounterpartyData.CounterpartyCustomerAdd), ToInt(CounterpartyData.CounterpartyCustomerEdit), ToInt(CounterpartyData.CounterpartyCustomerDelete), ToInt(WarehouseData.Warehouse), ToInt(WarehouseData.WarehouseAddProduct), ToInt(WarehouseData.WarehouseEditProduct), ToInt(WarehouseData.WarehouseAnnulateProduct), ToInt(WarehouseData.WarehouseEditCount), ToInt(AdminPanelData.AdminPanel), ToInt(AdminPanelData.AdminPanelAddAccount), ToInt(AdminPanelData.AdminPanelEditAccount), ToInt(AdminPanelData.AdminPanelDeleteAccount), ToInt(AdminPanelData.AdminPanelSetNumerator), ToInt(AdminPanelData.AdminPanelJournal), 0);
                    MySqlCommand command = new MySqlCommand(sql, connection);

                    ID = command.ExecuteNonQuery();

                    connection.Close();
                }

                getData = GetBaseDataByLogin();

                result = getData;
            }
            catch (Exception ex)
            {
                Dialog.ErrorMessage(null, "Ошибка обработки данных", ex.Message);
            }

            return result;
        }

        #region Редактирование данных

        /// <summary>
        /// Реадктировать аккаунт
        /// </summary>
        /// <returns>возвращает TRUE, если аккаунт успешно отредактирован</returns>
        public bool EditAccount()
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

                        string sql = String.Format(@"UPDATE ACCOUNT SET `LOGIN` = '{0}', `SURNAME` = '{1}', `NAME` = '{2}', `PATRONYMIC` = '{3}', `ADRESS` = '{4}', `PHONE` = '{5}', `SEX` = '{6}', `PURCHASE_INVOICE` = '{7}', `SALES_INVOICE` = '{8}', `REPORTS` = '{9}', `REPORT_PURCHASE_INVOICE` = '{10}', `REPORT_SALES_INVOICE` = '{11}', `REPORT_INCOME_EXPENSE` = '{12}', `COUNTERPARTY` = '{13}', `COUNTERPARTY_PROVIDER` = '{14}', `COUNTERPARTY_PROVIDER_ADD` = '{15}', `COUNTERPARTY_PROVIDER_EDIT` = '{16}', `COUNTERPARTY_PROVIDER_DELETE` = '{17}', `COUNTERPARTY_CUSTOMER` = '{18}', `COUNTERPARTY_CUSTOMER_ADD` = '{19}', `COUNTERPARTY_CUSTOMER_EDIT` = '{20}', `COUNTERPARTY_CUSTOMER_DELETE` = '{21}', `WAREHOUSE` = '{22}', `WAREHOUSE_ADD_PRODUCT` = '{23}', `WAREHOUSE_EDIT_PRODUCT` = '{24}', `WAREHOUSE_ANNULATE_PRODUCT` = '{25}', `WAREHOUSE_EDIT_COUNT` = '{26}', `ADMIN_PANEL` = '{27}', `ADMIN_PANEL_ADD_ACCOUNT` = '{28}', `ADMIN_PANEL_EDIT_ACCOUNT` = '{29}', `ADMIN_PANEL_DELETE_ACCOUNT` = '{30}', `ADMIN_PANEL_SET_NUMERATOR` = '{31}', `ADMIN_PANEL_JOURNAL` = '{32}' WHERE `ID` = {33}", Login, PersonalData.Surname, PersonalData.Name, PersonalData.Patronymic, PersonalData.Adress, PersonalData.Phone, ToInt(PersonalData.Sex), ToInt(InvoiceData.PurchaseInvoice), ToInt(InvoiceData.SalesInvoice), ToInt(ReportsData.Reports), ToInt(ReportsData.ReportPurchaseInvoice), ToInt(ReportsData.ReportSalesInvoice), ToInt(ReportsData.ReportIncomeExpense), ToInt(CounterpartyData.Counterparty), ToInt(CounterpartyData.CounterpartyProvider), ToInt(CounterpartyData.CounterpartyProviderAdd), ToInt(CounterpartyData.CounterpartyProviderEdit), ToInt(CounterpartyData.CounterpartyProviderDelete), ToInt(CounterpartyData.CounterpartyCustomer), ToInt(CounterpartyData.CounterpartyCustomerAdd), ToInt(CounterpartyData.CounterpartyCustomerEdit), ToInt(CounterpartyData.CounterpartyCustomerDelete), ToInt(WarehouseData.Warehouse), ToInt(WarehouseData.WarehouseAddProduct), ToInt(WarehouseData.WarehouseEditProduct), ToInt(WarehouseData.WarehouseAnnulateProduct), ToInt(WarehouseData.WarehouseEditCount), ToInt(AdminPanelData.AdminPanel), ToInt(AdminPanelData.AdminPanelAddAccount), ToInt(AdminPanelData.AdminPanelEditAccount), ToInt(AdminPanelData.AdminPanelDeleteAccount), ToInt(AdminPanelData.AdminPanelSetNumerator), ToInt(AdminPanelData.AdminPanelJournal), ID);
                        MySqlCommand command = new MySqlCommand(sql, connection);

                        command.ExecuteNonQuery();

                        connection.Close();

                        getData = true;
                    }

                    result = getData;
                }
                catch (Exception ex)
                {
                    Dialog.ErrorMessage(null, "Ошибка редактирования данных пользователя", ex.Message);
                }
            }
            else
            {
                Dialog.ErrorMessage(null, "Не указан ID пользователя");
            }

            return result;
        }

        /// <summary>
        /// Сменить пароль
        /// </summary>
        /// <param name="newPassword">новый пароль</param>
        /// <returns>возвращает TRUE, если пароль успешно сменен</returns>
        public bool ChangePassword(string newPassword)
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

                        string newPass = Criptex.Cript(newPassword);

                        string sql = @"UPDATE ACCOUNT SET `PASSWORD` = '" + newPass + @"' WHERE ID = " + ID.ToString();
                        MySqlCommand command = new MySqlCommand(sql, connection);

                        command.ExecuteNonQuery();

                        Password = newPass;

                        getData = true;

                        connection.Close();
                    }

                    result = getData;
                }
                catch (Exception ex)
                {
                    Dialog.ErrorMessage(null, "Ошибка редактирования пароля пользователя", ex.Message);
                }
            }
            else
            {
                Dialog.ErrorMessage(null, "Не указан ID пользователя");
            }

            return result;
        }

        /// <summary>
        /// Аннулировать аккаунт
        /// </summary>
        /// <returns>возвращает TRUE, если аккаунт успешно аннулирован</returns>
        public bool AnnuateAccount()
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

                        string sql = @"UPDATE ACCOUNT SET `IS_ANNULATED` = '1' WHERE ID = " + ID.ToString();
                        MySqlCommand command = new MySqlCommand(sql, connection);

                        command.ExecuteNonQuery();

                        IsAnnulated = true;

                        getData = true;

                        connection.Close();
                    }

                    result = getData;
                }
                catch (Exception ex)
                {
                    Dialog.ErrorMessage(null, "Ошибка аннулирования пользователя", ex.Message);
                }
            }
            else
            {
                Dialog.ErrorMessage(null, "Не указан ID пользователя");
            }

            return result;
        }

        /// <summary>
        /// Преобразовать логическое значение в TinyInt
        /// </summary>
        /// <param name="value">значение</param>
        /// <returns>возвращает 1, если TRUE и 0, если FALSE</returns>
        private int ToInt(bool value)
        {
            return (value) ? 1 : 0;
        }

        #endregion

        #region Статические методы

        /// <summary>
        /// Получить список не аннулированных аккаунтов с незащищенными персональными данными
        /// </summary>
        public static List<Account> GetNotAnnuledAccountsPersonalDate()
        {
            List<Account> result = new List<Account>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                {
                    connection.Open();

                    string sql = @"SELECT `ID`, `LOGIN`, `SURNAME`, `NAME`, `PATRONYMIC` FROM ACCOUNT WHERE IS_ANNULATED = '0'";
                    MySqlCommand command = new MySqlCommand(sql, connection);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Account account = new Account();

                            string tempS = reader["ID"].ToString();
                            int tempI = 0;
                            if (int.TryParse(tempS, out tempI))
                                account.ID = tempI;
                            else
                                account.ID = 0;

                            account.Login = reader["LOGIN"].ToString();
                            account.PersonalData.Surname = reader["SURNAME"].ToString();
                            account.PersonalData.Name = reader["NAME"].ToString();
                            account.PersonalData.Patronymic = reader["PATRONYMIC"].ToString();

                            result.Add(account);
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Dialog.ErrorMessage(null, "Ошибка получения данных о пользователе", ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Получить аккаунт с незащищенными данными по ID
        /// </summary>
        /// <param name="id">ID</param>
        public static Account GetAccountByID(int id)
        {
            Account account = new Account();

            if (account.GetPublicDataByID(id))
                return account;
            else
                return null;
        }

        /// <summary>
        /// Получить аккаунт с базовыми данными по логину
        /// </summary>
        /// <param name="login">логин</param>
        public static Account GetBaseAccountByLogin(string login)
        {
            Account account = new Account();

            if (account.GetBaseDataByLogin(login))
                return account;
            else
                return null;
        }

        #endregion
    }
}
