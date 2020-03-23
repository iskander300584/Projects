using GreenLeaf.Classes;
using GreenLeaf.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GreenLeaf.Windows.AdminPanel
{
    /// <summary>
    /// Окно отображения данных пользователя и создания
    /// </summary>
    public partial class AdminAccountWindow : Window
    {
        #region Объявление команд

        /// <summary>
        /// Редактировать
        /// </summary>
        public static RoutedUICommand EditCommand = new RoutedUICommand("Редактировать", "EditCommand", typeof(AdminAccountWindow));

        /// <summary>
        /// Аннулировать
        /// </summary>
        public static RoutedUICommand AnnulateCommand = new RoutedUICommand("Аннулировать", "AnnulateCommand", typeof(AdminAccountWindow));

        /// <summary>
        /// Сбросить пароль
        /// </summary>
        public static RoutedUICommand ResetPasswordCommand = new RoutedUICommand("Сбросить пароль", "ResetPasswordCommand", typeof(AdminAccountWindow));

        #endregion

        #region Поля класса

        /// <summary>
        /// Контекст данных окна
        /// </summary>
        private AdminContext context = new AdminContext();

        #endregion

        /// <summary>
        /// Окно добавления пользователя
        /// </summary>
        public AdminAccountWindow()
        {
            InitializeComponent();

            context.CurrentAccount = new Account();

            this.DataContext = context;
        }

        /// <summary>
        /// Окно отображения данных пользователя
        /// </summary>
        /// <param name="mode">режим работы окна</param>
        /// <param name="account">пользователь</param>
        public AdminAccountWindow(WindowMode mode, Account account)
        {
            InitializeComponent();

            context.CurrentAccount = account;

            context.CurrentAccount.GetFullDataByID();

            context.Mode = mode;

            this.DataContext = context;
        }

        #region Выбор чекбоксов

        /// <summary>
        /// Смена чекбокса доступа к поставщикам
        /// </summary>
        private void CheckBox_MasterProvider_Changed(object sender, RoutedEventArgs e)
        {
            if (!(bool)cbMasterProvider.IsChecked)
            {
                cbSlaveProvider_Add.IsChecked = false;
                cbSlaveProvider_Edit.IsChecked = false;
                cbSlaveProvider_Annulate.IsChecked = false;
            }
        }

        /// <summary>
        /// Смена вложенного чекбокса доступа к поставщикам
        /// </summary>
        private void CheckBox_SlaveProvider_Changed(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;

            if((bool)checkBox.IsChecked)
            {
                cbMasterProvider.IsChecked = true;
            }
        }

        /// <summary>
        /// Смена чекбокса доступа к клиентам
        /// </summary>
        private void CheckBox_MasterCustomer_Changed(object sender, RoutedEventArgs e)
        {
            if (!(bool)cbMasterCustomer.IsChecked)
            {
                cbSlaveCustomer_Add.IsChecked = false;
                cbSlaveCustomer_Edit.IsChecked = false;
                cbSlaveCustomer_Annulate.IsChecked = false;
            }
        }

        /// <summary>
        /// Смена вложенного чекбокса доступа к клиентам
        /// </summary>
        private void CheckBox_SlaveCustomer_Changed(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;

            if ((bool)checkBox.IsChecked)
            {
                cbMasterCustomer.IsChecked = true;
            }
        }


        #endregion

        #region Применить

        /// <summary>
        /// Применить
        /// </summary>
        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            switch(context.Mode)
            {
                case WindowMode.Create:
                    ApplyCreate();
                    break;

                case WindowMode.Edit:
                    ApplyEdit();
                    break;
            }
        }

        /// <summary>
        /// Применить создание пользователя
        /// </summary>
        private void ApplyCreate()
        {
            if (!SetParameters())
                return;

            if(context.CurrentAccount.CreateAccount())
            {
                Dialog.TransparentMessage(this, "Пользователь добавлен");

                context.Mode = WindowMode.Read;
            }
        }

        /// <summary>
        /// Применить редактирование пользователя
        /// </summary>
        private void ApplyEdit()
        {
            if (!SetParameters())
                return;

            if (context.CurrentAccount.EditAccount())
            {
                Dialog.TransparentMessage(this, "Данные сохранены");

                context.Mode = WindowMode.Read;
            }
        }

        /// <summary>
        /// Задать настройки
        /// </summary>
        private bool SetParameters()
        {
            Account acc = context.CurrentAccount;

            if(acc.Login == "")
            {
                Dialog.WarningMessage(this, "Не указан логин пользователя");

                return false;
            }
            else if (acc.PersonalData.Surname == "")
            {
                Dialog.WarningMessage(this, "Не указана фамилия пользователя");

                return false;
            }
            else if(acc.PersonalData.Sex == null)
            {
                Dialog.WarningMessage(this, "Не указан пол пользователя");

                return false;
            }

            acc.ReportsData.Reports = (acc.InvoiceData.PurchaseInvoice || acc.InvoiceData.SalesInvoice || acc.ReportsData.ReportPurchaseInvoice || acc.ReportsData.ReportSalesInvoice || acc.ReportsData.ReportUnIssuePurchaseInvoice || acc.ReportsData.ReportUnIssueSalesInvoice || acc.ReportsData.ReportIncomeExpense);

            acc.CounterpartyData.Counterparty = (acc.CounterpartyData.CounterpartyCustomer || acc.CounterpartyData.CounterpartyProvider);

            acc.WarehouseData.Warehouse = (acc.WarehouseData.WarehouseAddProduct || acc.WarehouseData.WarehouseEditProduct || acc.WarehouseData.WarehouseAnnulateProduct || acc.WarehouseData.WarehouseEditCount);

            acc.AdminPanelData.AdminPanel = (acc.AdminPanelData.AdminPanelAddAccount || acc.AdminPanelData.AdminPanelEditAccount || acc.AdminPanelData.AdminPanelDeleteAccount || acc.AdminPanelData.AdminPanelSetNumerator || acc.AdminPanelData.AdminPanelJournal);

            return true;
        }

        #endregion

        #region Отмена

        /// <summary>
        /// Отмена
        /// </summary>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            switch(context.Mode)
            {
                case WindowMode.Create:
                    CancelCreate();
                    break;

                case WindowMode.Edit:
                    CancelEdit();
                    break;
            }
        }

        /// <summary>
        /// Отменить создание пользователя
        /// </summary>
        private void CancelCreate()
        {
            this.DialogResult = false;
        }

        /// <summary>
        /// Отменить редактирование пользователя
        /// </summary>
        private void CancelEdit()
        {
            if(context.CurrentAccount.GetFullDataByID())
            {
                context.Mode = WindowMode.Read;
            }
        }

        #endregion

        #region Редактировать

        /// <summary>
        /// Проверка возможности редактировать пользователя
        /// </summary>
        private void Edit_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (context != null && context.CurrentAccount != null && context.Mode == WindowMode.Read && ProgramSettings.CurrentUser.AdminPanelData.AdminPanelEditAccount);
        }

        /// <summary>
        /// Редактировать пользователя
        /// </summary>
        private void Edit_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            context.Mode = WindowMode.Edit;
        }

        #endregion

        #region Аннулировать

        /// <summary>
        /// Проверка возможности аннулировать пользователя
        /// </summary>
        private void Annulate_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (context != null && context.CurrentAccount != null && context.Mode == WindowMode.Read && ProgramSettings.CurrentUser.AdminPanelData.AdminPanelDeleteAccount);
        }

        /// <summary>
        /// Аннулировать пользователя
        /// </summary>
        private void Annulate_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (Dialog.QuestionMessage(this, "Пользователь будет аннулирован без возможности восстановления. Продолжить?") != MessageBoxResult.Yes)
                return;

            if(context.CurrentAccount.AnnuateAccount())
            {
                Dialog.TransparentMessage(this, "Пользователь аннулирован");

                this.DialogResult = true;
            }
        }

        #endregion

        #region Сброс пароля

        /// <summary>
        /// Проверка возможности сбросить пароль
        /// </summary>
        private void ResetPassword_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (context != null && context.CurrentAccount != null && context.Mode == WindowMode.Read && ProgramSettings.CurrentUser.AdminPanelData.AdminPanelEditAccount);
        }

        /// <summary>
        /// Сбросить пароль
        /// </summary>
        private void ResetPassword_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (Dialog.QuestionMessage(this, "Сбросить пароль к стандартному \"12345\"?") != MessageBoxResult.Yes)
                return;

            if (context.CurrentAccount.ChangePassword("12345"))
            {
                Dialog.TransparentMessage(this, "Пароль успешно сброшен");
            }
        }

        #endregion
    }
}