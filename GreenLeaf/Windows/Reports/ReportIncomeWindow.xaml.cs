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
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;
using GreenLeaf.Constants;

namespace GreenLeaf.Windows.Reports
{
    /// <summary>
    /// Окно отчета прихода / расхода
    /// </summary>
    public partial class ReportIncomeWindow : Window
    {
        #region Объявление команд

        /// <summary>
        /// Команда Открыть
        /// </summary>
        public static RoutedUICommand OpenCommand = new RoutedUICommand("Открыть", "OpenCommand", typeof(ReportIncomeWindow));

        #endregion

        #region Поля класса

        /// <summary>
        /// Список отображаемых накладных
        /// </summary>
        private List<Invoice> InvoiceList = new List<Invoice>();

        /// <summary>
        /// Сумма по приходным накладным
        /// </summary>
        private double SummPurchase = 0;

        /// <summary>
        /// Купон по приходным накладным
        /// </summary>
        private double CouponPurchase = 0;

        /// <summary>
        /// Сумма по расходным накладным
        /// </summary>
        private double SummSales = 0;

        /// <summary>
        /// Купон по расходным накладным
        /// </summary>
        private double CouponSales = 0;

        /// <summary>
        /// Сумма по балансу
        /// </summary>
        private double SummBalance = 0;

        /// <summary>
        /// Купон по балансу
        /// </summary>
        private double CouponBalance = 0;

        #endregion

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

        #region Получение данных

        /// <summary>
        /// Получение данных
        /// </summary>
        private void GetData()
        {
            Mouse.OverrideCursor = Cursors.Wait;

            dgInvoices.ItemsSource = null;

            InvoiceList.Clear();

            // Получение списка приходных накладных
            List<Invoice> temp = Invoice.GetPurchaseInvoices((DateTime)dpFromDate.SelectedDate, (DateTime)dpTillDate.SelectedDate).Where(i => i.IsIssued).ToList();

            SummPurchase = 0;
            CouponPurchase = 0;

            foreach(Invoice inv in temp)
            {
                SummPurchase += inv.Cost;
                CouponPurchase += inv.Coupon;

                InvoiceList.Add(inv);
            }

            tbConsumption.Text = Conversion.ToFinance(SummPurchase);
            tbConsumptionCoupon.Text = Conversion.ToFinance(CouponPurchase);

            // Получение списка расходных накладных
            temp = Invoice.GetSalesInvoices((DateTime)dpFromDate.SelectedDate, (DateTime)dpTillDate.SelectedDate).Where(i => i.IsIssued).ToList();

            SummSales = 0;
            CouponSales = 0;

            foreach (Invoice inv in temp)
            {
                SummSales += inv.Cost;
                CouponSales += inv.Coupon;

                InvoiceList.Add(inv);
            }

            tbIncome.Text = Conversion.ToFinance(SummSales);
            tbIncomeCoupon.Text = Conversion.ToFinance(CouponSales);

            // Вычисление баланса
            SummBalance = SummSales - SummPurchase;
            CouponBalance = CouponSales - CouponPurchase;

            tbBalance.Text = Conversion.ToFinance(SummBalance);
            tbBalanceCoupon.Text = Conversion.ToFinance(CouponBalance);

            // Настройка цвета баланса
            if (SummBalance == 0)
                tbBalance.Foreground = Brushes.Black;
            else if (SummBalance > 0)
                tbBalance.Foreground = Brushes.DarkGreen;
            else
                tbBalance.Foreground = Brushes.Red;

            if (CouponBalance == 0)
                tbBalanceCoupon.Foreground = Brushes.Black;
            else if (CouponBalance > 0)
                tbBalanceCoupon.Foreground = Brushes.DarkGreen;
            else
                tbBalanceCoupon.Foreground = Brushes.Red;

            // Сортировка списка
            InvoiceList = InvoiceList.OrderBy(i => i.Date).ToList();

            dgInvoices.ItemsSource = InvoiceList;

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Кнопка Поиск
        /// </summary>
        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            GetData();
        }

        #endregion

        #region Открытие накладной

        /// <summary>
        /// Проверка возможности нажатия кнопки Открыть
        /// </summary>
        private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (dgInvoices != null && dgInvoices.SelectedItem != null) ? true : false;
        }

        /// <summary>
        /// Нажатие кнопки Открыть
        /// </summary>
        private void Open_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            Open();
        }

        /// <summary>
        /// Двойной щелчок по накладной
        /// </summary>
        private void dgInvoices_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgInvoices.SelectedItem != null)
            {
                Open();
            }
        }

        /// <summary>
        /// Открыть накладную
        /// </summary>
        private void Open()
        {
            Mouse.OverrideCursor = Cursors.Wait;

            Invoice invoice = dgInvoices.SelectedItem as Invoice;

            InvoiceWindow view = new InvoiceWindow(invoice.IsPurchase, invoice.ID);
            view.Owner = this;

            Mouse.OverrideCursor = null;

            view.ShowDialog();

            view.Close();

            GetData();
        }

        #endregion
    }
}