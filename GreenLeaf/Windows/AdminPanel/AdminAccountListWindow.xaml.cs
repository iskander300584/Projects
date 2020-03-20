using GreenLeaf.Classes;
using GreenLeaf.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace GreenLeaf.Windows.AdminPanel
{
    /// <summary>
    /// Окно отображения списка пользователей
    /// </summary>
    public partial class AdminAccountListWindow : Window
    {
        #region Объявление команд

        /// <summary>
        /// Команда добавления пользователя
        /// </summary>
        public static RoutedUICommand AddAccount = new RoutedUICommand("Добавить пользователя", "AddAccount", typeof(AdminAccountListWindow));

        /// <summary>
        /// Команда отображения данных пользователя
        /// </summary>
        public static RoutedUICommand OpenAccount = new RoutedUICommand("Открыть карточку", "OpenAccount", typeof(AdminAccountListWindow));

        /// <summary>
        /// Команда редактирования пользователя
        /// </summary>
        public static RoutedUICommand EditAccount = new RoutedUICommand("Редактировать пользователя", "EditAccount", typeof(AdminAccountListWindow));

        /// <summary>
        /// Команда аннулирования пользователя
        /// </summary>
        public static RoutedUICommand AnnulateAccount = new RoutedUICommand("Аннулировать пользователя", "AnnulateAccount", typeof(AdminAccountListWindow));

        #endregion

        #region Поля класса

        /// <summary>
        /// Список отображаемых пользователей
        /// </summary>
        private List<Account> AccountList;

        #endregion

        /// <summary>
        /// Окно отображения списка пользователей
        /// </summary>
        public AdminAccountListWindow()
        {
            InitializeComponent();

            GetData();
        }

        /// <summary>
        /// Получение данных
        /// </summary>
        private void GetData()
        {
            Mouse.OverrideCursor = Cursors.Wait;

            dgAccounts.ItemsSource = null;

            AccountList = Account.GetActualAccountList(tbSearch.Text);

            dgAccounts.ItemsSource = AccountList;

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Нажатие кнопки "Поиск"
        /// </summary>
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            GetData();
        }

        /// <summary>
        /// Проверка возможности создания пользователя
        /// </summary>
        private void AddAccount_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ProgramSettings.CurrentUser.AdminPanelData.AdminPanelAddAccount;
        }

        /// <summary>
        /// Создание пользователя
        /// </summary>
        private void AddAccount_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            AdminAccountWindow view = new AdminAccountWindow();
            view.Owner = this;

            view.ShowDialog();
            view.Close();

            GetData();
        }

        /// <summary>
        /// Проверка возможности открыть карточку пользователя
        /// </summary>
        private void OpenAccount_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (dgAccounts != null && dgAccounts.SelectedItem != null);
        }

        /// <summary>
        /// Открыть карточку
        /// </summary>
        private void OpenAccount_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            Account account = (Account)dgAccounts.SelectedItem;

            AdminAccountWindow view = new AdminAccountWindow(WindowMode.Read, account);
            view.Owner = this;

            view.ShowDialog();
            view.Close();

            dgAccounts.Items.Refresh();
        }

        /// <summary>
        /// Проверка возможности редактирования пользователя
        /// </summary>
        private void EditAccount_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (dgAccounts != null && dgAccounts.SelectedItem != null && ProgramSettings.CurrentUser.AdminPanelData.AdminPanelEditAccount);
        }

        /// <summary>
        /// Редактирование пользователя
        /// </summary>
        private void EditAccount_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            Account account = (Account)dgAccounts.SelectedItem;

            AdminAccountWindow view = new AdminAccountWindow(WindowMode.Edit, account);
            view.Owner = this;

            view.ShowDialog();
            view.Close();

            dgAccounts.Items.Refresh();
        }

        /// <summary>
        /// Проверка возможности аннулирования пользователя
        /// </summary>
        private void AnnulateAccount_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (dgAccounts != null && dgAccounts.SelectedItem != null && ProgramSettings.CurrentUser.AdminPanelData.AdminPanelDeleteAccount);
        }

        /// <summary>
        /// Аннулирование пользователя
        /// </summary>
        private void AnnulateAccount_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (Dialog.QuestionMessage(this, "Пользователь будет аннулирован без возможности восстановления. Продолжить?") != MessageBoxResult.Yes)
                return;

            Mouse.OverrideCursor = Cursors.Wait;

            Account account = (Account)dgAccounts.SelectedItem;

            try
            {
                if (account.AnnuateAccount())
                {
                    GetData();

                    Dialog.TransparentMessage(this, "Операция выполнена");
                }
            }
            catch (Exception ex)
            {
                Mouse.OverrideCursor = null;

                Dialog.ErrorMessage(this, "Ошибка аннулирования пользователя", ex.Message);
            }

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Двойной щелчок по пользователю
        /// </summary>
        private void dgAccount_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgAccounts.SelectedItem == null)
                return;

            Account account = (Account)dgAccounts.SelectedItem;

            AdminAccountWindow view = new AdminAccountWindow(WindowMode.Read, account);
            view.Owner = this;

            view.ShowDialog();
            view.Close();

            dgAccounts.Items.Refresh();
        }
    }
}