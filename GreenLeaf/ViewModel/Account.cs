using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MySql.Data.MySqlClient;
using GreenLeaf.Classes;

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

        private string _surname = string.Empty;
        /// <summary>
        /// Фамилия
        /// </summary>
        public string Surname
        {
            get { return _surname; }
            set
            {
                if (_surname != value)
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

        private string _visibleName = string.Empty;
        /// <summary>
        /// Отображаемое имя
        /// </summary>
        public string VisibleName
        {
            get { return _visibleName; }
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

        private bool _sex = false;
        /// <summary>
        /// Пол (TRUE - мужской; FALSE - женский)
        /// </summary>
        public bool Sex
        {
            get { return _sex; }
            set
            {
                if(_sex != value)
                {
                    _sex = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _purchaseInvoice = false;
        /// <summary>
        /// Признак доступности создания приходных накладных
        /// </summary>
        public bool PurchaseInvoice
        {
            get { return _purchaseInvoice; }
            set
            {
                if (_purchaseInvoice != value)
                {
                    _purchaseInvoice = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _salesInvoice = false;
        /// <summary>
        /// Признак доступности создания расходных накладных
        /// </summary>
        public bool SalesInvoice
        {
            get { return _salesInvoice; }
            set
            {
                if (_salesInvoice != value)
                {
                    _salesInvoice = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _reports = false;
        /// <summary>
        /// Признак доступности отчетов
        /// </summary>
        public bool Reports
        {
            get { return _reports; }
            set
            {
                if (_reports != value)
                {
                    _reports = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _reportPurchaseInvoice = false;
        /// <summary>
        /// Признак доступности отчетов по приходным накладным
        /// </summary>
        public bool ReportPurchaseInvoice
        {
            get { return _reportPurchaseInvoice; }
            set
            {
                if (_reportPurchaseInvoice != value)
                {
                    _reportPurchaseInvoice = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _reportSalesInvoice = false;
        /// <summary>
        /// Признак доступности отчетов по расходным накладным
        /// </summary>
        public bool ReportSalesInvoice
        {
            get { return _reportSalesInvoice; }
            set
            {
                if (_reportSalesInvoice != value)
                {
                    _reportSalesInvoice = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _reportIncomeExpense = false;
        /// <summary>
        /// Признак доступности отчетов приход-расход
        /// </summary>
        public bool ReportIncomeExpense
        {
            get { return _reportIncomeExpense; }
            set
            {
                if (_reportIncomeExpense != value)
                {
                    _reportIncomeExpense = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _counterparty = false;
        /// <summary>
        /// Признак доступности управления контрагентами
        /// </summary>
        public bool Counterparty
        {
            get { return _counterparty; }
            set
            {
                if (_counterparty != value)
                {
                    _counterparty = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _counterpartyProvider = false;
        /// <summary>
        /// Признак доступности управления поставщиками
        /// </summary>
        public bool CounterpartyProvider
        {
            get { return _counterpartyProvider; }
            set
            {
                if (_counterpartyProvider != value)
                {
                    _counterpartyProvider = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _counterpartyProviderAdd = false;
        /// <summary>
        /// Признак доступности добавления поставщика
        /// </summary>
        public bool CounterpartyProviderAdd
        {
            get { return _counterpartyProviderAdd; }
            set
            {
                if (_counterpartyProviderAdd != value)
                {
                    _counterpartyProviderAdd = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _counterpartyProviderEdit = false;
        /// <summary>
        /// Признак доступности редактирования поставщика
        /// </summary>
        public bool CounterpartyProviderEdit
        {
            get { return _counterpartyProviderEdit; }
            set
            {
                if (_counterpartyProviderEdit != value)
                {
                    _counterpartyProviderEdit = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _counterpartyProviderDelete = false;
        /// <summary>
        /// Признак доступности удаления поставщика
        /// </summary>
        public bool CounterpartyProviderDelete
        {
            get { return _counterpartyProviderDelete; }
            set
            {
                if (_counterpartyProviderDelete != value)
                {
                    _counterpartyProviderDelete = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _counterpartyCustomer = false;
        /// <summary>
        /// Признак доступности управления покупателями
        /// </summary>
        public bool CounterpartyCustomer
        {
            get { return _counterpartyCustomer; }
            set
            {
                if (_counterpartyCustomer != value)
                {
                    _counterpartyCustomer = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _counterpartyCustomerAdd = false;
        /// <summary>
        /// Признак доступности добавления покупателя
        /// </summary>
        public bool CounterpartyCustomerAdd
        {
            get { return _counterpartyCustomerAdd; }
            set
            {
                if (_counterpartyCustomerAdd != value)
                {
                    _counterpartyCustomerAdd = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _counterpartyCustomerEdit = false;
        /// <summary>
        /// Признак доступности редактирования покупателя
        /// </summary>
        public bool CounterpartyCustomerEdit
        {
            get { return _counterpartyCustomerEdit; }
            set
            {
                if (_counterpartyCustomerEdit != value)
                {
                    _counterpartyCustomerEdit = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _counterpartyCustomerDelete = false;
        /// <summary>
        /// Признак доступности удаления покупателя
        /// </summary>
        public bool CounterpartyCustomerDelete
        {
            get { return _counterpartyCustomerDelete; }
            set
            {
                if (_counterpartyCustomerDelete != value)
                {
                    _counterpartyCustomerDelete = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _warehouse = false;
        /// <summary>
        /// Признак доступности управления складом
        /// </summary>
        public bool Warehouse
        {
            get { return _warehouse; }
            set
            {
                if (_warehouse != value)
                {
                    _warehouse = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _warehouseAddProduct = false;
        /// <summary>
        /// Признак доступности добавления вида товара
        /// </summary>
        public bool WarehouseAddProduct
        {
            get { return _warehouseAddProduct; }
            set
            {
                if (_warehouseAddProduct != value)
                {
                    _warehouseAddProduct = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _warehouseEditProduct = false;
        /// <summary>
        /// Признак доступности редактирования вида товара
        /// </summary>
        public bool WarehouseEditProduct
        {
            get { return _warehouseEditProduct; }
            set
            {
                if (_warehouseEditProduct != value)
                {
                    _warehouseEditProduct = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _warehouseAnnulateProduct = false;
        /// <summary>
        /// Признак доступности аннулирования вида товара
        /// </summary>
        public bool WarehouseAnnulateProduct
        {
            get { return _warehouseAnnulateProduct; }
            set
            {
                if (_warehouseAnnulateProduct != value)
                {
                    _warehouseAnnulateProduct = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _warehouseEditCount = false;
        /// <summary>
        /// Признак доступности редактирования количества товара
        /// </summary>
        public bool WarehouseEditCount
        {
            get { return _warehouseEditCount; }
            set
            {
                if (_warehouseEditCount != value)
                {
                    _warehouseEditCount = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _adminPanel = false;
        /// <summary>
        /// Признак доступности панели администратора
        /// </summary>
        public bool AdminPanel
        {
            get { return _adminPanel; }
            set
            {
                if (_adminPanel != value)
                {
                    _adminPanel = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _adminPanelAddAccount = false;
        /// <summary>
        /// Признак доступности добавления пользователя
        /// </summary>
        public bool AdminPanelAddAccount
        {
            get { return _adminPanelAddAccount; }
            set
            {
                if (_adminPanelAddAccount != value)
                {
                    _adminPanelAddAccount = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _adminPanelEditAccount = false;
        /// <summary>
        /// Признак доступности редактирования пользователя
        /// </summary>
        public bool AdminPanelEditAccount
        {
            get { return _adminPanelEditAccount; }
            set
            {
                if (_adminPanelEditAccount != value)
                {
                    _adminPanelEditAccount = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _adminPanelDeleteAccount = false;
        /// <summary>
        /// Признак доступности удаления пользователя
        /// </summary>
        public bool AdminPanelDeleteAccount
        {
            get { return _adminPanelDeleteAccount; }
            set
            {
                if (_adminPanelDeleteAccount != value)
                {
                    _adminPanelDeleteAccount = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _adminPanelSetNumerator = false;
        /// <summary>
        /// Признак доступности установки значения нумератора
        /// </summary>
        public bool AdminPanelSetNumerator
        {
            get { return _adminPanelSetNumerator; }
            set
            {
                if (_adminPanelSetNumerator != value)
                {
                    _adminPanelSetNumerator = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _adminPanelJournal = false;
        /// <summary>
        /// Признак доступности просмотра журнала событий
        /// </summary>
        public bool AdminPanelJournal
        {
            get { return _adminPanelJournal; }
            set
            {
                if (_adminPanelJournal != value)
                {
                    _adminPanelJournal = value;
                    OnPropertyChanged();
                }
            }
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
        /// Получение отображаемого имени
        /// </summary>
        private void GetVisibleName()
        {
            if (_surname.Trim() != string.Empty)
            {
                _visibleName = _surname;

                if (_name.Trim() != "")
                {
                    _visibleName += _name[0] + ".";

                    if (_patronymic.Trim() != "")
                        _visibleName += " " + _patronymic[0] + ".";
                }
            }
            else if (_name.Trim() != string.Empty)
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

                        string sql = "SELECT * FROM ACCOUNT WHERE ID=" + ID.ToString();
                        MySqlCommand command = new MySqlCommand(sql, connection);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Login = reader["LOGIN"].ToString();
                                Password = reader["PASSWORD"].ToString();
                                Surname = reader["SURNAME"].ToString();
                                Name = reader["SURNAME"].ToString();
                                Patronymic = reader["PATRONYMIC"].ToString();

                                string tempS = reader["ADRESS"].ToString();
                                if (tempS != "")
                                    Adress = Criptex.UnCript(tempS);

                                tempS = reader["PHONE"].ToString();
                                if (tempS != "")
                                    Phone = Criptex.UnCript(tempS);

                                tempS = reader["SEX"].ToString();
                                bool tempB = false;
                                if (bool.TryParse(tempS, out tempB))
                                    Sex = tempB;
                                else
                                    Sex = false;

                                tempS = reader["PURCHASE_INVOICE"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    PurchaseInvoice = tempB;
                                else
                                    PurchaseInvoice = false;

                                tempS = reader["SALES_INVOICE"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    SalesInvoice = tempB;
                                else
                                    SalesInvoice = false;

                                tempS = reader["REPORTS"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    Reports = tempB;
                                else
                                    Reports = false;

                                tempS = reader["REPORT_PURCHASE_INVOICE"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    ReportPurchaseInvoice = tempB;
                                else
                                    ReportPurchaseInvoice = false;

                                tempS = reader["REPORT_SALES_INVOICE"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    ReportSalesInvoice = tempB;
                                else
                                    ReportSalesInvoice = false;

                                tempS = reader["REPORT_INCOME_EXPENSE"].ToString();
                                if (bool.TryParse(tempS, out tempB))
                                    ReportIncomeExpense = tempB;
                                else
                                    ReportIncomeExpense = false;

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
    }
}
