using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GreenLeaf.ViewModel;
using GreenLeaf.Classes;

namespace GreenLeaf.Windows.InvoiceView
{
    /// <summary>
    /// Список накладных
    /// </summary>
    public partial class InvoiceListWindow : Window
    {
        #region Объявление команд

        /// <summary>
        /// Загрузка данных
        /// </summary>
        public static RoutedUICommand LoadData = new RoutedUICommand("Загрузить данные", "LoadData", typeof(InvoiceListWindow));

        /// <summary>
        /// Открыть накладную
        /// </summary>
        public static RoutedUICommand OpenInvoice = new RoutedUICommand("Открыть накладную", "OpenInvoice", typeof(InvoiceListWindow));

        /// <summary>
        /// Сбросить фильтры
        /// </summary>
        public static RoutedUICommand ResetFilter = new RoutedUICommand("Сбросить фильтры", "ResetFilter", typeof(InvoiceListWindow));

        #endregion

        /// <summary>
        /// Признак отображения приходных накладных
        /// </summary>
        private bool ShowPurchase = true;

        /// <summary>
        /// Список отображаемых накладных
        /// </summary>
        private List<Invoice> InvoiceList = new List<Invoice>();

        /// <summary>
        /// Список контрагентов
        /// </summary>
        private List<Counterparty> CounterpartyList = new List<Counterparty>();

        /// <summary>
        /// Список пользователей
        /// </summary>
        private List<Account> AccountList = new List<Account>();

        /// <summary>
        /// Список накладных
        /// </summary>
        /// <param name="showPurchase">отображать приходные накладные</param>
        public InvoiceListWindow(bool showPurchase)
        {
            InitializeComponent();

            this.Title = (showPurchase) ? "Отчет по приходным накладным" : "Отчет по расходным накладным";

            ShowPurchase = showPurchase;

            dpToPeriod.SelectedDate = DateTime.Today;

            dpFromPeriod.SelectedDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            GetCounterparties();

            GetAccounts();

            GetData();
        }

        /// <summary>
        /// Получить список контрагентов
        /// </summary>
        private void GetCounterparties()
        {
            Counterparty all = new Counterparty { Nomination = "Все" };
            CounterpartyList.Add(all);

            if (ShowPurchase)
            {
                foreach (Counterparty cp in Counterparty.GetProviderList().OrderBy(c => c.VisibleName))
                    CounterpartyList.Add(cp);
            }
            else
            {
                foreach (Counterparty cp in Counterparty.GetCustomerList().OrderBy(c => c.VisibleName))
                    CounterpartyList.Add(cp);
            }

            cbCounterparty.ItemsSource = CounterpartyList;
            cbCounterparty.SelectedIndex = 0;
        }

        /// <summary>
        /// Получить список аккаунтов
        /// </summary>
        private void GetAccounts()
        {
            if ((ShowPurchase && ProgramSettings.CurrentUser.ReportsData.ReportPurchaseInvoice) || (!ShowPurchase && ProgramSettings.CurrentUser.ReportsData.ReportSalesInvoice))
            {
                Account all = new Account();
                all.PersonalData.Surname = "Все";

                AccountList.Add(all);

                foreach (Account acc in Account.GetAccountsByRoles(ShowPurchase).OrderBy(a => a.PersonalData.VisibleName))
                {
                    AccountList.Add(acc);
                }
            }
            else
                AccountList.Add(ProgramSettings.CurrentUser);

            cbUser.ItemsSource = AccountList;
            cbUser.SelectedIndex = 0;
        }

        /// <summary>
        /// Получение списка накладных
        /// </summary>
        private void GetData()
        {
            DateTime from = (DateTime)dpFromPeriod.SelectedDate;
            DateTime to = (DateTime)dpToPeriod.SelectedDate;
            string number = tbNumber.Text.Trim();
            int id_acc = (cbUser.SelectedItem as Account).ID;
            int id_cp = (cbCounterparty.SelectedItem as Counterparty).ID;

            dgInvoices.ItemsSource = null;

            if (ShowPurchase)
            {
                InvoiceList = Invoice.GetPurchaseInvoices(from, to, id_acc, id_cp).OrderBy(i => i.CreateDate).ToList();
            }
            else
            {
                InvoiceList = Invoice.GetSalesInvoices(from, to, id_acc, id_cp).OrderBy(i => i.CreateDate).ToList();
            }

            if (number != "")
                InvoiceList = InvoiceList.Where(i => i.Number.ToString().Contains(number)).ToList();

            double summ = 0;
            double coupon = 0;

            foreach (Invoice inv in InvoiceList)
            {
                inv.GetUsers();
                summ += inv.Cost;
                coupon += inv.Coupon;
            }

            dgInvoices.ItemsSource = InvoiceList;

            tbCost.Text = Conversion.ToFinance(summ);
            tbCoupon.Text = Conversion.ToFinance(coupon);
        }

        /// <summary>
        /// Загрузить данные
        /// </summary>
        private void LoadData_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            GetData();

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Проверка возможности открыть накладную
        /// </summary>
        private void OpenInvoice_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (dgInvoices != null && dgInvoices.SelectedItem != null) ? true : false;
        }

        /// <summary>
        /// Открыть накладную
        /// </summary>
        private void OpenInvoice_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            ShowInvoice();
        }

        /// <summary>
        /// Открыть накладную
        /// </summary>
        private void ShowInvoice()
        {
            Mouse.OverrideCursor = Cursors.Wait;

            Invoice invoice = dgInvoices.SelectedItem as Invoice;

            InvoiceWindow view = new InvoiceWindow(invoice.IsPurchase, invoice.ID);
            view.Owner = this;

            Mouse.OverrideCursor = null;

            view.ShowDialog();

            Mouse.OverrideCursor = Cursors.Wait;

            view.Close();

            GetData();

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Проверка возможности сбросить фильтры
        /// </summary>
        private void ResetFilter_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((cbCounterparty != null && cbCounterparty.SelectedIndex != 0) || (cbUser != null && cbUser.SelectedIndex != 0) || (tbNumber != null && tbNumber.Text != "") || (dpFromPeriod != null && ((DateTime)dpFromPeriod.SelectedDate).Date != new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).Date) || (dpToPeriod != null && ((DateTime)dpToPeriod.SelectedDate).Date != DateTime.Today.Date)) ? true : false;
        }

        /// <summary>
        /// Сбросить фильтры
        /// </summary>
        private void ResetFilter_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            dpToPeriod.SelectedDate = DateTime.Today;

            dpFromPeriod.SelectedDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            tbNumber.Text = "";

            cbCounterparty.SelectedIndex = 0;

            cbUser.SelectedIndex = 0;

            GetData();

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Двойной клик по накладной
        /// </summary>
        private void dgInvoices_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgInvoices.SelectedItem != null)
                ShowInvoice();
        }
    }
}
