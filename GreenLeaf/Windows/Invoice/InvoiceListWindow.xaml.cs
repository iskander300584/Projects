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

        private void GetAccounts()
        {

        }
    }
}
