using MySql.Data.MySqlClient;

namespace GreenLeaf.Classes
{
    /// <summary>
    /// Настройки подключения
    /// </summary>
    public static class ConnectSetting
    {
        /// <summary>
        /// Путь к рабочей папке
        /// </summary>
        public const string WorkFolder = @"C:\ProgramData\GreenLeaf\";

        private static string _server = string.Empty;
        /// <summary>
        /// Имя сервера
        /// </summary>
        public static string Server
        {
            get { return _server; }
            set
            {
                if(_server != value)
                {
                    _server = value;
                    BuildConnectionString();
                }
            }
        }

        private static string _db = string.Empty;
        /// <summary>
        /// Имя базы данных
        /// </summary>
        public static string DB
        {
            get { return _db; }
            set
            {
                if(_db != value)
                {
                    _db = value;
                    BuildConnectionString();
                }
            }
        }

        private static string _connectionString = string.Empty;
        /// <summary>
        /// Строка подключения к БД
        /// </summary>
        public static string ConnectionString
        {
            get { return _connectionString; }
        }

        private static string _adminLogin = string.Empty;
        /// <summary>
        /// Имя учетной записи администратора
        /// </summary>
        public static string AdminLogin
        {
            get { return _adminLogin; }
            set
            {
                if(_adminLogin != value)
                {
                    _adminLogin = value;
                    BuildConnectionString();
                }
            }
        }

        private static string _adminPassword = string.Empty;
        /// <summary>
        /// Пароль учетной записи администратора
        /// </summary>
        public static string AdminPassword
        {
            get { return _adminPassword; }
            set
            {
                if (_adminPassword != value)
                {
                    _adminPassword = value;
                    BuildConnectionString();
                }
            }
        }

        /// <summary>
        /// Построение строки подключения
        /// </summary>
        private static void BuildConnectionString()
        {
            if (AdminPassword != string.Empty)
            {
                try
                {
                    _connectionString = Criptex.Cript("SERVER=" + Server + ";" +
                                            "DATABASE=" + DB + ";" +
                                            "UID=" + AdminLogin + ";" +
                                            "Pwd=" + Criptex.UnCript(AdminPassword) + ";");
                }
                catch { }
            }
        }

        public static class User
        {
            private static string _login = string.Empty;
            /// <summary>
            /// Логин подключенного пользователя
            /// </summary>
            public static string Login
            {
                get { return _login; }
                set
                {
                    if (_login != value)
                    {
                        _login = value;
                        GetUserGrants();
                    }
                }
            }

            /// <summary>
            /// Личные данные пользователя
            /// </summary>
            public static class Person
            {
                private static int _id = 0;
                /// <summary>
                /// ID
                /// </summary>
                public static int ID
                {
                    get { return _id; }
                    set
                    {
                        if(_id != value)
                        {
                            _id = value;
                        }
                    }
                }

                private static string _surname = string.Empty;
                /// <summary>
                /// Фамилия
                /// </summary>
                public static string Surname
                {
                    get { return _surname; }
                    set
                    {
                        if(_surname != value)
                        {
                            _surname = value;
                        }
                    }
                }

                private static string _name = string.Empty;
                /// <summary>
                /// Имя
                /// </summary>
                public static string Name
                {
                    get { return _name; }
                    set
                    {
                        if(_name != value)
                        {
                            _name = value;
                        }
                    }
                }

                private static string _patronymic = string.Empty;
                /// <summary>
                /// Отчество
                /// </summary>
                public static string Patronymic
                {
                    get { return _patronymic; }
                    set
                    {
                        if(_patronymic != value)
                        {
                            _patronymic = value;
                        }
                    }
                }

                private static string _adress = string.Empty;
                /// <summary>
                /// Адрес
                /// </summary>
                public static string Adress
                {
                    get { return _adress; }
                    set
                    {
                        if (_adress != value)
                        {
                            _adress = value;
                        }
                    }
                }

                private static string _phone = string.Empty;
                /// <summary>
                /// Телефон
                /// </summary>
                public static string Phone
                {
                    get { return _phone; }
                    set
                    {
                        if (_phone != value)
                        {
                            _phone = value;
                        }
                    }
                }

                private static bool _sex = false;
                /// <summary>
                /// Пол
                /// </summary>
                public static bool Sex
                {
                    get { return _sex; }
                    set
                    {
                        if(_sex != value)
                        {
                            _sex = value;
                        }
                    }
                }
            }

            /// <summary>
            /// Права пользователя на накладные
            /// </summary>
            public static class GrantInvoice
            {
                private static bool _purchaseInvoice = false;
                /// <summary>
                /// Пользователь может оформлять документ "Приходная накладная"
                /// </summary>
                public static bool PurchaseInvoice
                {
                    get { return _purchaseInvoice; }
                    set
                    {
                        if(_purchaseInvoice != value)
                        {
                            _purchaseInvoice = value;
                        }
                    }
                }

                private static bool _salesInvoice = false;
                /// <summary>
                /// Пользователь может оформлять документ "Расходная накладная"
                /// </summary>
                public static bool SalesInvoice
                {
                    get { return _salesInvoice; }
                    set
                    {
                        if (_salesInvoice != value)
                        {
                            _salesInvoice = value;
                        }
                    }
                }
            }

            /// <summary>
            /// Права пользователя на отчеты
            /// </summary>
            public static class GrantReport
            {
                private static bool _reports = false;
                /// <summary>
                /// Пользователь может просматривать отчеты
                /// </summary>
                public static bool Reports
                {
                    get { return _reports; }
                    set
                    {
                        if (_reports != value)
                        {
                            _reports = value;
                        }
                    }
                }

                private static bool _reportsPurchaseInvoice = false;
                /// <summary>
                /// Пользователь может просматривать приходные накладные
                /// </summary>
                public static bool ReportsPurchaseInvoice
                {
                    get { return _reportsPurchaseInvoice; }
                    set
                    {
                        if (_reportsPurchaseInvoice != value)
                        {
                            _reportsPurchaseInvoice = value;
                        }
                    }
                }

                private static bool _reportsSalesInvoice = false;
                /// <summary>
                /// Пользователь может просматривать расходные накладные
                /// </summary>
                public static bool ReportsSalesInvoice
                {
                    get { return _reportsSalesInvoice; }
                    set
                    {
                        if (_reportsSalesInvoice != value)
                        {
                            _reportsSalesInvoice = value;
                        }
                    }
                }

                private static bool _reportsIncomeExpense = false;
                /// <summary>
                /// Пользователь может просматривать отчет приход-расход
                /// </summary>
                public static bool ReportsIncomeExpense
                {
                    get { return _reportsIncomeExpense; }
                    set
                    {
                        if (_reportsIncomeExpense != value)
                        {
                            _reportsIncomeExpense = value;
                        }
                    }
                }
            }

            /// <summary>
            /// Права пользователя на контрагентов
            /// </summary>
            public static class GrantCounterparty
            {
                private static bool _counterparty = false;
                /// <summary>
                ///  Пользователь может управлять контрагентами
                /// </summary>
                public static bool Counterparty
                {
                    get { return _counterparty; }
                    set
                    {
                        if (_counterparty != value)
                        {
                            _counterparty = value;
                        }
                    }
                }

                private static bool _addCounterparty = false;
                /// <summary>
                ///  Пользователь может создавать контрагента
                /// </summary>
                public static bool AddCounterparty
                {
                    get { return _addCounterparty; }
                    set
                    {
                        if (_addCounterparty != value)
                        {
                            _addCounterparty = value;
                        }
                    }
                }

                private static bool _editCounterparty = false;
                /// <summary>
                ///  Пользователь может редактировать данные контрагента
                /// </summary>
                public static bool EditCounterparty
                {
                    get { return _editCounterparty; }
                    set
                    {
                        if (_editCounterparty != value)
                        {
                            _editCounterparty = value;
                        }
                    }
                }

                private static bool _deleteCounterparty = false;
                /// <summary>
                ///  Пользователь может удалять контрагента
                /// </summary>
                public static bool DeleteCounterparty
                {
                    get { return _deleteCounterparty; }
                    set
                    {
                        if (_deleteCounterparty != value)
                        {
                            _deleteCounterparty = value;
                        }
                    }
                }
            }
            
            /// <summary>
            /// Права пользователя на управление складом
            /// </summary>
            public static class GrantWarehouse
            {
                private static bool _warehouseManagement = false;
                /// <summary>
                /// Пользователь может управлять складом
                /// </summary>
                public static bool WarehouseManagement
                {
                    get { return _warehouseManagement; }
                    set
                    {
                        if (_warehouseManagement != value)
                        {
                            _warehouseManagement = value;
                        }
                    }
                }

                private static bool _warehouseAddProduct = false;
                /// <summary>
                /// Пользователь может добавлять вид товара
                /// </summary>
                public static bool WarehouseAddProduct
                {
                    get { return _warehouseAddProduct; }
                    set
                    {
                        if (_warehouseAddProduct != value)
                        {
                            _warehouseAddProduct = value;
                        }
                    }
                }

                private static bool _warehouseEditProduct = false;
                /// <summary>
                /// Пользователь может редактировать вид товара
                /// </summary>
                public static bool WarehouseEditProduct
                {
                    get { return _warehouseEditProduct; }
                    set
                    {
                        if (_warehouseEditProduct != value)
                        {
                            _warehouseEditProduct = value;
                        }
                    }
                }

                private static bool _warehouseAnnulateProduct = false;
                /// <summary>
                /// Пользователь может аннулировать вид товара
                /// </summary>
                public static bool WarehouseAnnulateProduct
                {
                    get { return _warehouseAnnulateProduct; }
                    set
                    {
                        if (_warehouseAnnulateProduct != value)
                        {
                            _warehouseAnnulateProduct = value;
                        }
                    }
                }

                private static bool _warehouseEditCount = false;
                /// <summary>
                /// Пользователь может редактировать количество товара
                /// </summary>
                public static bool WarehouseEditCount
                {
                    get { return _warehouseEditCount; }
                    set
                    {
                        if (_warehouseEditCount != value)
                        {
                            _warehouseEditCount = value;
                        }
                    }
                }
            }

            /// <summary>
            /// Права пользователя на панель управления
            /// </summary>
            public static class GrantAdminPanel
            {
                private static bool _adminPanel = false;
                /// <summary>
                /// Пользователь имеет доступ к панели управления
                /// </summary>
                public static bool AdminPanel
                {
                    get { return _adminPanel; }
                    set
                    {
                        if (_adminPanel != value)
                        {
                            _adminPanel = value;
                        }
                    }
                }

                private static bool _adminPanelAddAccount = false;
                /// <summary>
                /// Пользователь может добавлять пользователей
                /// </summary>
                public static bool AdminPanelAddAccount
                {
                    get { return _adminPanelAddAccount; }
                    set
                    {
                        if (_adminPanelAddAccount != value)
                        {
                            _adminPanelAddAccount = value;
                        }
                    }
                }

                private static bool _adminPanelEditAccount = false;
                /// <summary>
                /// Пользователь может редактировать пользователей
                /// </summary>
                public static bool AdminPanelEditAccount
                {
                    get { return _adminPanelEditAccount; }
                    set
                    {
                        if (_adminPanelEditAccount != value)
                        {
                            _adminPanelEditAccount = value;
                        }
                    }
                }

                private static bool _adminPanelDeleteAccount = false;
                /// <summary>
                /// Пользователь может удалять пользователей
                /// </summary>
                public static bool AdminPanelDeleteAccount
                {
                    get { return _adminPanelDeleteAccount; }
                    set
                    {
                        if (_adminPanelDeleteAccount != value)
                        {
                            _adminPanelDeleteAccount = value;
                        }
                    }
                }

                private static bool _adminPanelSetNumerator = false;
                /// <summary>
                /// Пользователь может устанавливать значение нумератора
                /// </summary>
                public static bool AdminPanelSetNumerator
                {
                    get { return _adminPanelSetNumerator; }
                    set
                    {
                        if (_adminPanelSetNumerator != value)
                        {
                            _adminPanelSetNumerator = value;
                        }
                    }
                }

                private static bool _adminPanelJournal = false;
                /// <summary>
                /// Пользователь может просматривать журнал событий
                /// </summary>
                public static bool AdminPanelJournal
                {
                    get { return _adminPanelJournal; }
                    set
                    {
                        if (_adminPanelJournal != value)
                        {
                            _adminPanelJournal = value;
                        }
                    }
                }
            }

            /// <summary>
            /// Получение прав подкюченного пользователя
            /// </summary>
            private static void GetUserGrants()
            {
                if (ConnectionString != string.Empty && Login != string.Empty)
                {
                    using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectionString)))
                    {
                        connection.Open();

                        string sql = "SELECT * FROM ACCOUNT WHERE LOGIN = \'" + Login + "\'";
                        MySqlCommand command = new MySqlCommand(sql, connection);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // персональные данные
                                Person.ID = int.Parse(reader["ID"].ToString());
                                Person.Surname = reader["SURNAME"].ToString();
                                Person.Name = reader["NAME"].ToString();
                                Person.Patronymic = reader["PATRONYMIC"].ToString();
                                Person.Adress = reader["ADRESS"].ToString();
                                Person.Phone = reader["PHONE"].ToString();
                                Person.Sex = bool.Parse(reader["SEX"].ToString());

                                // накладные
                                GrantInvoice.PurchaseInvoice = bool.Parse(reader["PURCHASE_INVOICE"].ToString());
                                GrantInvoice.SalesInvoice = bool.Parse(reader["SALES_INVOICE"].ToString());

                                // отчеты
                                GrantReport.Reports = bool.Parse(reader["REPORTS"].ToString());
                                GrantReport.ReportsPurchaseInvoice = bool.Parse(reader["REPORT_PURCHASE_INVOICE"].ToString());
                                GrantReport.ReportsSalesInvoice = bool.Parse(reader["REPORT_SALES_INVOICE"].ToString());
                                GrantReport.ReportsIncomeExpense = bool.Parse(reader["REPORT_INCOME_EXPENSE"].ToString());

                                // контрагенты
                                GrantCounterparty.Counterparty = bool.Parse(reader["COUNTERPARTY"].ToString());
                                GrantCounterparty.AddCounterparty = bool.Parse(reader["COUNTERPARTY_ADD"].ToString());
                                GrantCounterparty.EditCounterparty = bool.Parse(reader["COUNTERPARTY_EDIT"].ToString());
                                GrantCounterparty.DeleteCounterparty = bool.Parse(reader["COUNTERPARTY_DELETE"].ToString());

                                // управление складом
                                GrantWarehouse.WarehouseManagement = bool.Parse(reader["WAREHOUSE"].ToString());
                                GrantWarehouse.WarehouseAddProduct = bool.Parse(reader["WAREHOUSE_ADD_PRODUCT"].ToString());
                                GrantWarehouse.WarehouseEditProduct = bool.Parse(reader["WAREHOUSE_EDIT_PRODUCT"].ToString());
                                GrantWarehouse.WarehouseAnnulateProduct = bool.Parse(reader["WAREHOUSE_ANNULATE_PRODUCT"].ToString());
                                GrantWarehouse.WarehouseEditCount = bool.Parse(reader["WAREHOUSE_EDIT_COUNT"].ToString());

                                // панель управления
                                GrantAdminPanel.AdminPanel = bool.Parse(reader["ADMIN_PANEL"].ToString());
                                GrantAdminPanel.AdminPanelAddAccount = bool.Parse(reader["ADMIN_PANEL_ADD_ACCOUNT"].ToString());
                                GrantAdminPanel.AdminPanelEditAccount = bool.Parse(reader["ADMIN_PANEL_EDIT_ACCOUNT"].ToString());
                                GrantAdminPanel.AdminPanelDeleteAccount = bool.Parse(reader["ADMIN_PANEL_DELETE_ACCOUNT"].ToString());
                                GrantAdminPanel.AdminPanelSetNumerator = bool.Parse(reader["ADMIN_PANEL_SET_NUMERATOR"].ToString());
                                GrantAdminPanel.AdminPanelJournal = bool.Parse(reader["ADMIN_PANEL_JOURNAL"].ToString());
                            }
                        }

                        connection.Close();
                    }
                }
            }
        }
    }
}
