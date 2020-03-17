using GreenLeaf.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace GreenLeaf.Windows.AdminPanel
{
    /// <summary>
    /// Окно журнала событий
    /// </summary>
    public partial class AdminJournalWindow : Window
    {
        #region Поля класса

        /// <summary>
        /// Отображаемые события журнала
        /// </summary>
        private List<Journal> JournalList;

        /// <summary>
        /// Список пользователей
        /// </summary>
        private List<Account> Accounts;

        #endregion

        /// <summary>
        /// Окно журнала событий
        /// </summary>
        public AdminJournalWindow()
        {
            InitializeComponent();

            dpFromDate.SelectedDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dpTillDate.SelectedDate = DateTime.Today;

            GetUsers();

            GetData();
        }

        /// <summary>
        /// Получение списка пользователей
        /// </summary>
        private void GetUsers()
        {
            Accounts = Account.GetAllAccountsPersonalData();

            cbUser.Items.Add("Все");

            foreach (Account account in Accounts.OrderBy(a => a.PersonalData.VisibleName))
                cbUser.Items.Add(account.PersonalData.VisibleName);

            cbUser.SelectedIndex = 0;
        }

        /// <summary>
        /// Получение данных
        /// </summary>
        private void GetData()
        {
            Mouse.OverrideCursor = Cursors.Wait;

            dgJournal.ItemsSource = null;

            Account account = null;

            if (cbUser.SelectedIndex != 0)
            {
                account = Accounts.FirstOrDefault(a => a.PersonalData.VisibleName == cbUser.SelectedItem.ToString());
            }

            if (account == null)
                JournalList = Journal.GetJournal((DateTime)dpFromDate.SelectedDate, (DateTime)dpTillDate.SelectedDate);
            else
                JournalList = Journal.GetJournal(account.ID, (DateTime)dpFromDate.SelectedDate, (DateTime)dpTillDate.SelectedDate);

            dgJournal.ItemsSource = JournalList;

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Кнопка Поиск
        /// </summary>
        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            GetData();
        }
    }
}