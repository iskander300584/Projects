using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GreenLeaf.ViewModel;
using GreenLeaf.Classes;

namespace GreenLeaf.Windows.InvoiceView
{
    /// <summary>
    /// Список накладных
    /// </summary>
    public partial class InvoiceListWindow : Window
    {
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
            if ((ShowPurchase && ConnectSetting.CurrentUser.ReportsData.ReportPurchaseInvoice) || (!ShowPurchase && ConnectSetting.CurrentUser.ReportsData.ReportSalesInvoice))
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
                AccountList.Add(ConnectSetting.CurrentUser);

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

                if (number != "")
                    InvoiceList = InvoiceList.Where(i => i.Number.ToString().Contains(number)).ToList();
            }
            else
            {
                InvoiceList = Invoice.GetSalesInvoices(from, to, id_acc, id_cp).OrderBy(i => i.CreateDate).ToList();

                if (number != "")
                    InvoiceList = InvoiceList.Where(i => i.Number.ToString().Contains(number)).ToList();
            }

            dgInvoices.ItemsSource = InvoiceList;
        }
    }
}
