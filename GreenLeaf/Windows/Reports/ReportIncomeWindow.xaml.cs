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
using GreenLeaf.Windows.InvoiceView;

namespace GreenLeaf.Windows.Reports
{
    /// <summary>
    /// Окно отчета прихода / расхода
    /// </summary>
    public partial class ReportIncomeWindow : Window
    {
        /// <summary>
        /// Список отображаемых накладных
        /// </summary>
        private List<Invoice> InvoiceList = new List<Invoice>();

        /// <summary>
        /// Окно отчета прихода / расхода
        /// </summary>
        public ReportIncomeWindow()
        {
            InitializeComponent();

            dpFromDate.SelectedDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dpTillDate.SelectedDate = DateTime.Today;

            GetData();
        }

        /// <summary>
        /// Получение данных
        /// </summary>
        private void GetData()
        {
            dgInvoices.ItemsSource = null;

            InvoiceList.Clear();

            // Получение списка приходных накладных
            List<Invoice> temp = Invoice.GetPurchaseInvoices((DateTime)dpFromDate.SelectedDate, (DateTime)dpTillDate.SelectedDate).Where(i => i.IsIssued).ToList();

            double summPurchase = 0;
            double couponPurchase = 0;

            foreach(Invoice inv in temp)
            {
                inv.GetUsers();

                summPurchase += inv.Cost;
                couponPurchase += inv.Coupon;

                InvoiceList.Add(inv);
            }

            tbConsumption.Text = Conversion.ToFinance(summPurchase);
            tbConsumptionCoupon.Text = Conversion.ToFinance(couponPurchase);

            // Получение списка расходных накладных
            temp = Invoice.GetSalesInvoices((DateTime)dpFromDate.SelectedDate, (DateTime)dpTillDate.SelectedDate).Where(i => i.IsIssued).ToList();

            double summSales = 0;
            double couponSales = 0;

            foreach (Invoice inv in temp)
            {
                inv.GetUsers();

                summSales += inv.Cost;
                couponSales += inv.Coupon;

                InvoiceList.Add(inv);
            }

            tbIncome.Text = Conversion.ToFinance(summSales);
            tbIncomeCoupon.Text = Conversion.ToFinance(couponSales);

            // Вычисление баланса
            double summBalance = summSales - summPurchase;
            double couponBalance = couponSales - couponPurchase;

            tbBalance.Text = Conversion.ToFinance(summBalance);
            tbBalanceCoupon.Text = Conversion.ToFinance(couponBalance);

            // Настройка цвета баланса
            if (summBalance == 0)
                tbBalance.Foreground = Brushes.Black;
            else if (summBalance > 0)
                tbBalance.Foreground = Brushes.DarkGreen;
            else
                tbBalance.Foreground = Brushes.Red;

            if (couponBalance == 0)
                tbBalanceCoupon.Foreground = Brushes.Black;
            else if (couponBalance > 0)
                tbBalanceCoupon.Foreground = Brushes.DarkGreen;
            else
                tbBalanceCoupon.Foreground = Brushes.Red;

            // Сортировка списка
            InvoiceList = InvoiceList.OrderBy(i => i.Date).ToList();

            dgInvoices.ItemsSource = InvoiceList;
        }

        /// <summary>
        /// Двойной щелчок по накладной
        /// </summary>
        private void dgInvoices_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(dgInvoices.SelectedItem != null)
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
        }
    }
}