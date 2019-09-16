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

        /// <summary>
        /// Создать новый аккаунт
        /// </summary>
        /// <returns>возвращает TRUE, если аккаунт успешно создан</returns>
        public bool CreateAccount()
        {
            bool result = false;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                {
                    connection.Open();

                    string newPass = Criptex.Cript("12345");

                    string sql = String.Format(@"INSERT INTO `ACCOUNT` (`LOGIN`, `PASSWORD`, `SURNAME`, `NAME`, `PATRONYMIC`, `ADRESS`, `PHONE`, `SEX`, `PURCHASE_INVOICE`, `SALES_INVOICE`, `REPORTS`, `REPORT_PURCHASE_INVOICE`, `REPORT_SALES_INVOICE`, `REPORT_INCOME_EXPENSE`, `COUNTERPARTY`, `COUNTERPARTY_PROVIDER`, `COUNTERPARTY_PROVIDER_ADD`, `COUNTERPARTY_PROVIDER_EDIT`, `COUNTERPARTY_PROVIDER_DELETE`, `COUNTERPARTY_CUSTOMER`, `COUNTERPARTY_CUSTOMER_ADD`, `COUNTERPARTY_CUSTOMER_EDIT`, `COUNTERPARTY_CUSTOMER_DELETE`, `WAREHOUSE`, `WAREHOUSE_ADD_PRODUCT`, `WAREHOUSE_EDIT_PRODUCT`, `WAREHOUSE_ANNULATE_PRODUCT`, `WAREHOUSE_EDIT_COUNT`, `ADMIN_PANEL`, `ADMIN_PANEL_ADD_ACCOUNT`, `ADMIN_PANEL_EDIT_ACCOUNT`, `ADMIN_PANEL_DELETE_ACCOUNT`, `ADMIN_PANEL_SET_NUMERATOR`, `ADMIN_PANEL_JOURNAL`, `IS_ANNULATED`)  VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}', '{30}', '{31}', '{32}', '{33}', '{34}')", Login, newPass, PersonalData.Surname, PersonalData.Name, PersonalData.Patronymic, PersonalData.Adress, PersonalData.Phone, Conversion.ToString(PersonalData.Sex), Conversion.ToString(InvoiceData.PurchaseInvoice), Conversion.ToString(InvoiceData.SalesInvoice), Conversion.ToString(ReportsData.Reports), Conversion.ToString(ReportsData.ReportPurchaseInvoice), Conversion.ToString(ReportsData.ReportSalesInvoice), Conversion.ToString(ReportsData.ReportIncomeExpense), Conversion.ToString(CounterpartyData.Counterparty), Conversion.ToString(CounterpartyData.CounterpartyProvider), Conversion.ToString(CounterpartyData.CounterpartyProviderAdd), Conversion.ToString(CounterpartyData.CounterpartyProviderEdit), Conversion.ToString(CounterpartyData.CounterpartyProviderDelete), Conversion.ToString(CounterpartyData.CounterpartyCustomer), Conversion.ToString(CounterpartyData.CounterpartyCustomerAdd), Conversion.ToString(CounterpartyData.CounterpartyCustomerEdit), Conversion.ToString(CounterpartyData.CounterpartyCustomerDelete), Conversion.ToString(WarehouseData.Warehouse), Conversion.ToString(WarehouseData.WarehouseAddProduct), Conversion.ToString(WarehouseData.WarehouseEditProduct), Conversion.ToString(WarehouseData.WarehouseAnnulateProduct), Conversion.ToString(WarehouseData.WarehouseEditCount), Conversion.ToString(AdminPanelData.AdminPanel), Conversion.ToString(AdminPanelData.AdminPanelAddAccount), Conversion.ToString(AdminPanelData.AdminPanelEditAccount), Conversion.ToString(AdminPanelData.AdminPanelDeleteAccount), Conversion.ToString(AdminPanelData.AdminPanelSetNumerator), Conversion.ToString(AdminPanelData.AdminPanelJournal), 0);

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
                Dialog.ErrorMessage(null, "Ошибка создания пользователя", ex.Message);
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
                    using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                    {
                        connection.Open();

                        string sql = "SELECT * FROM `ACCOUNT` WHERE `ACCOUNT`.`ID` = " + ID.ToString();
                        using (MySqlCommand command = new MySqlCommand(sql, connection))
                        {
                            using (MySqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Login = reader["LOGIN"].ToString();
                                    Password = reader["PASSWORD"].ToString();

                                    PersonalData.Surname = reader["SURNAME"].ToString();
                                    PersonalData.Name = reader["SURNAME"].ToString();
                                    PersonalData.Patronymic = reader["PATRONYMIC"].ToString();
                                    PersonalData.Adress = Conversion.ToUncriptString(reader["ADRESS"].ToString());
                                    PersonalData.Phone = Conversion.ToUncriptString(reader["PHONE"].ToString());
                                    PersonalData.Sex = Conversion.ToBool(reader["SEX"].ToString());

                                    InvoiceData.PurchaseInvoice = Conversion.ToBool(reader["PURCHASE_INVOICE"].ToString());
                                    InvoiceData.SalesInvoice = Conversion.ToBool(reader["SALES_INVOICE"].ToString());

                                    ReportsData.Reports = Conversion.ToBool(reader["REPORTS"].ToString());
                                    ReportsData.ReportPurchaseInvoice = Conversion.ToBool(reader["REPORT_PURCHASE_INVOICE"].ToString());
                                    ReportsData.ReportSalesInvoice = Conversion.ToBool(reader["REPORT_SALES_INVOICE"].ToString());
                                    ReportsData.ReportIncomeExpense = Conversion.ToBool(reader["REPORT_INCOME_EXPENSE"].ToString());

                                    CounterpartyData.Counterparty = Conversion.ToBool(reader["COUNTERPARTY"].ToString());
                                    CounterpartyData.CounterpartyProvider = Conversion.ToBool(reader["COUNTERPARTY_PROVIDER"].ToString());
                                    CounterpartyData.CounterpartyProviderAdd = Conversion.ToBool(reader["COUNTERPARTY_PROVIDER_ADD"].ToString());
                                    CounterpartyData.CounterpartyProviderEdit = Conversion.ToBool(reader["COUNTERPARTY_PROVIDER_EDIT"].ToString());
                                    CounterpartyData.CounterpartyProviderDelete = Conversion.ToBool(reader["COUNTERPARTY_PROVIDER_DELETE"].ToString());
                                    CounterpartyData.CounterpartyCustomer = Conversion.ToBool(reader["COUNTERPARTY_CUSTOMER"].ToString());
                                    CounterpartyData.CounterpartyCustomerAdd = Conversion.ToBool(reader["COUNTERPARTY_CUSTOMER_ADD"].ToString());
                                    CounterpartyData.CounterpartyCustomerEdit = Conversion.ToBool(reader["COUNTERPARTY_CUSTOMER_EDIT"].ToString());
                                    CounterpartyData.CounterpartyCustomerDelete = Conversion.ToBool(reader["COUNTERPARTY_CUSTOMER_DELETE"].ToString());

                                    WarehouseData.Warehouse = Conversion.ToBool(reader["WAREHOUSE"].ToString());
                                    WarehouseData.WarehouseAddProduct = Conversion.ToBool(reader["WAREHOUSE_ADD_PRODUCT"].ToString());
                                    WarehouseData.WarehouseEditProduct = Conversion.ToBool(reader["WAREHOUSE_EDIT_PRODUCT"].ToString());
                                    WarehouseData.WarehouseAnnulateProduct = Conversion.ToBool(reader["WAREHOUSE_ANNULATE_PRODUCT"].ToString());
                                    WarehouseData.WarehouseEditCount = Conversion.ToBool(reader["WAREHOUSE_EDIT_COUNT"].ToString());

                                    AdminPanelData.AdminPanel = Conversion.ToBool(reader["ADMIN_PANEL"].ToString());
                                    AdminPanelData.AdminPanelAddAccount = Conversion.ToBool(reader["ADMIN_PANEL_ADD_ACCOUNT"].ToString());
                                    AdminPanelData.AdminPanelEditAccount = Conversion.ToBool(reader["ADMIN_PANEL_EDIT_ACCOUNT"].ToString());
                                    AdminPanelData.AdminPanelDeleteAccount = Conversion.ToBool(reader["ADMIN_PANEL_DELETE_ACCOUNT"].ToString());
                                    AdminPanelData.AdminPanelSetNumerator = Conversion.ToBool(reader["ADMIN_PANEL_SET_NUMERATOR"].ToString());
                                    AdminPanelData.AdminPanelJournal = Conversion.ToBool(reader["ADMIN_PANEL_JOURNAL"].ToString());

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
                    using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                    {
                        connection.Open();

                        string sql = "SELECT * FROM `ACCOUNT` WHERE `ACCOUNT`.`ID` = " + ID.ToString();
                        using (MySqlCommand command = new MySqlCommand(sql, connection))
                        {
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
                                    PersonalData.Sex = Conversion.ToBool(reader["SEX"].ToString());

                                    InvoiceData.PurchaseInvoice = Conversion.ToBool(reader["PURCHASE_INVOICE"].ToString());
                                    InvoiceData.SalesInvoice = Conversion.ToBool(reader["SALES_INVOICE"].ToString());

                                    ReportsData.Reports = Conversion.ToBool(reader["REPORTS"].ToString());
                                    ReportsData.ReportPurchaseInvoice = Conversion.ToBool(reader["REPORT_PURCHASE_INVOICE"].ToString());
                                    ReportsData.ReportSalesInvoice = Conversion.ToBool(reader["REPORT_SALES_INVOICE"].ToString());
                                    ReportsData.ReportIncomeExpense = Conversion.ToBool(reader["REPORT_INCOME_EXPENSE"].ToString());

                                    CounterpartyData.Counterparty = Conversion.ToBool(reader["COUNTERPARTY"].ToString());
                                    CounterpartyData.CounterpartyProvider = Conversion.ToBool(reader["COUNTERPARTY_PROVIDER"].ToString());
                                    CounterpartyData.CounterpartyProviderAdd = Conversion.ToBool(reader["COUNTERPARTY_PROVIDER_ADD"].ToString());
                                    CounterpartyData.CounterpartyProviderEdit = Conversion.ToBool(reader["COUNTERPARTY_PROVIDER_EDIT"].ToString());
                                    CounterpartyData.CounterpartyProviderDelete = Conversion.ToBool(reader["COUNTERPARTY_PROVIDER_DELETE"].ToString());
                                    CounterpartyData.CounterpartyCustomer = Conversion.ToBool(reader["COUNTERPARTY_CUSTOMER"].ToString());
                                    CounterpartyData.CounterpartyCustomerAdd = Conversion.ToBool(reader["COUNTERPARTY_CUSTOMER_ADD"].ToString());
                                    CounterpartyData.CounterpartyCustomerEdit = Conversion.ToBool(reader["COUNTERPARTY_CUSTOMER_EDIT"].ToString());
                                    CounterpartyData.CounterpartyCustomerDelete = Conversion.ToBool(reader["COUNTERPARTY_CUSTOMER_DELETE"].ToString());

                                    WarehouseData.Warehouse = Conversion.ToBool(reader["WAREHOUSE"].ToString());
                                    WarehouseData.WarehouseAddProduct = Conversion.ToBool(reader["WAREHOUSE_ADD_PRODUCT"].ToString());
                                    WarehouseData.WarehouseEditProduct = Conversion.ToBool(reader["WAREHOUSE_EDIT_PRODUCT"].ToString());
                                    WarehouseData.WarehouseAnnulateProduct = Conversion.ToBool(reader["WAREHOUSE_ANNULATE_PRODUCT"].ToString());
                                    WarehouseData.WarehouseEditCount = Conversion.ToBool(reader["WAREHOUSE_EDIT_COUNT"].ToString());

                                    AdminPanelData.AdminPanel = Conversion.ToBool(reader["ADMIN_PANEL"].ToString());
                                    AdminPanelData.AdminPanelAddAccount = Conversion.ToBool(reader["ADMIN_PANEL_ADD_ACCOUNT"].ToString());
                                    AdminPanelData.AdminPanelEditAccount = Conversion.ToBool(reader["ADMIN_PANEL_EDIT_ACCOUNT"].ToString());
                                    AdminPanelData.AdminPanelDeleteAccount = Conversion.ToBool(reader["ADMIN_PANEL_DELETE_ACCOUNT"].ToString());
                                    AdminPanelData.AdminPanelSetNumerator = Conversion.ToBool(reader["ADMIN_PANEL_SET_NUMERATOR"].ToString());
                                    AdminPanelData.AdminPanelJournal = Conversion.ToBool(reader["ADMIN_PANEL_JOURNAL"].ToString());

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
                    using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                    {
                        connection.Open();

                        string sql = "SELECT `ADRESS`, `PHONE` FROM `ACCOUNT` WHERE `ACCOUNT`.`ID` = " + ID.ToString();
                        using (MySqlCommand command = new MySqlCommand(sql, connection))
                        {
                            using (MySqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    PersonalData.Adress = Conversion.ToUncriptString(reader["ADRESS"].ToString());
                                    PersonalData.Phone = Conversion.ToUncriptString(reader["PHONE"].ToString());
                                }
                            }
                        }

                        connection.Close();
                    }

                    result = true;
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
                using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                {
                    connection.Open();

                    string sql = "SELECT `ID`, `PASSWORD`, `IS_ANNULATED` FROM `ACCOUNT` WHERE `ACCOUNT`.`LOGIN` = \'" + Login + "\'";
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ID = Conversion.ToInt(reader["ID"].ToString());

                                Password = reader["PASSWORD"].ToString();

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
                    using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                    {
                        connection.Open();

                        string sql = String.Format(@"UPDATE `ACCOUNT` SET `LOGIN` = '{0}', `SURNAME` = '{1}', `NAME` = '{2}', `PATRONYMIC` = '{3}', `ADRESS` = '{4}', `PHONE` = '{5}', `SEX` = '{6}', `PURCHASE_INVOICE` = '{7}', `SALES_INVOICE` = '{8}', `REPORTS` = '{9}', `REPORT_PURCHASE_INVOICE` = '{10}', `REPORT_SALES_INVOICE` = '{11}', `REPORT_INCOME_EXPENSE` = '{12}', `COUNTERPARTY` = '{13}', `COUNTERPARTY_PROVIDER` = '{14}', `COUNTERPARTY_PROVIDER_ADD` = '{15}', `COUNTERPARTY_PROVIDER_EDIT` = '{16}', `COUNTERPARTY_PROVIDER_DELETE` = '{17}', `COUNTERPARTY_CUSTOMER` = '{18}', `COUNTERPARTY_CUSTOMER_ADD` = '{19}', `COUNTERPARTY_CUSTOMER_EDIT` = '{20}', `COUNTERPARTY_CUSTOMER_DELETE` = '{21}', `WAREHOUSE` = '{22}', `WAREHOUSE_ADD_PRODUCT` = '{23}', `WAREHOUSE_EDIT_PRODUCT` = '{24}', `WAREHOUSE_ANNULATE_PRODUCT` = '{25}', `WAREHOUSE_EDIT_COUNT` = '{26}', `ADMIN_PANEL` = '{27}', `ADMIN_PANEL_ADD_ACCOUNT` = '{28}', `ADMIN_PANEL_EDIT_ACCOUNT` = '{29}', `ADMIN_PANEL_DELETE_ACCOUNT` = '{30}', `ADMIN_PANEL_SET_NUMERATOR` = '{31}', `ADMIN_PANEL_JOURNAL` = '{32}' WHERE `ACCOUNT`.`ID` = {33}", Login, PersonalData.Surname, PersonalData.Name, PersonalData.Patronymic, PersonalData.Adress, PersonalData.Phone, Conversion.ToString(PersonalData.Sex), Conversion.ToString(InvoiceData.PurchaseInvoice), Conversion.ToString(InvoiceData.SalesInvoice), Conversion.ToString(ReportsData.Reports), Conversion.ToString(ReportsData.ReportPurchaseInvoice), Conversion.ToString(ReportsData.ReportSalesInvoice), Conversion.ToString(ReportsData.ReportIncomeExpense), Conversion.ToString(CounterpartyData.Counterparty), Conversion.ToString(CounterpartyData.CounterpartyProvider), Conversion.ToString(CounterpartyData.CounterpartyProviderAdd), Conversion.ToString(CounterpartyData.CounterpartyProviderEdit), Conversion.ToString(CounterpartyData.CounterpartyProviderDelete), Conversion.ToString(CounterpartyData.CounterpartyCustomer), Conversion.ToString(CounterpartyData.CounterpartyCustomerAdd), Conversion.ToString(CounterpartyData.CounterpartyCustomerEdit), Conversion.ToString(CounterpartyData.CounterpartyCustomerDelete), Conversion.ToString(WarehouseData.Warehouse), Conversion.ToString(WarehouseData.WarehouseAddProduct), Conversion.ToString(WarehouseData.WarehouseEditProduct), Conversion.ToString(WarehouseData.WarehouseAnnulateProduct), Conversion.ToString(WarehouseData.WarehouseEditCount), Conversion.ToString(AdminPanelData.AdminPanel), Conversion.ToString(AdminPanelData.AdminPanelAddAccount), Conversion.ToString(AdminPanelData.AdminPanelEditAccount), Conversion.ToString(AdminPanelData.AdminPanelDeleteAccount), Conversion.ToString(AdminPanelData.AdminPanelSetNumerator), Conversion.ToString(AdminPanelData.AdminPanelJournal), ID);

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
                    using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                    {
                        connection.Open();

                        string newPass = Criptex.Cript(newPassword);

                        string sql = String.Format(@"UPDATE `ACCOUNT` SET `PASSWORD` = '{0}' WHERE `ACCOUNT`.`ID` = {1}", newPass, ID);

                        using (MySqlCommand command = new MySqlCommand(sql, connection))
                        {
                            command.ExecuteNonQuery();
                        }

                        Password = newPass;

                        connection.Close();
                    }

                    result = true;
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
                    using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                    {
                        connection.Open();

                        string sql = @"UPDATE `ACCOUNT` SET `IS_ANNULATED` = '1' WHERE `ACCOUNT`.`ID` = " + ID.ToString();

                        using (MySqlCommand command = new MySqlCommand(sql, connection))
                        {
                            command.ExecuteNonQuery();
                        }

                        IsAnnulated = true;

                        connection.Close();
                    }

                    result = true;
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

                    string sql = @"SELECT `ID`, `LOGIN`, `SURNAME`, `NAME`, `PATRONYMIC` FROM `ACCOUNT` WHERE `ACCOUNT`.`IS_ANNULATED` = '0'";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Account account = new Account();

                                account.ID = Conversion.ToInt(reader["ID"].ToString());

                                account.Login = reader["LOGIN"].ToString();
                                account.PersonalData.Surname = reader["SURNAME"].ToString();
                                account.PersonalData.Name = reader["NAME"].ToString();
                                account.PersonalData.Patronymic = reader["PATRONYMIC"].ToString();

                                result.Add(account);
                            }
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
