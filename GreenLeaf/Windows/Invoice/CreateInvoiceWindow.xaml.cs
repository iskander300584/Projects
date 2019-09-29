using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GreenLeaf.Classes;
using GreenLeaf.ViewModel;

namespace GreenLeaf.Windows.InvoiceView
{
    /// <summary>
    /// Окно создания / редактирования накладной
    /// </summary>
    public partial class CreateInvoiceWindow : Window
    {
        /// <summary>
        /// Накладная
        /// </summary>
        private ViewModel.Invoice CurrentInvoice = new ViewModel.Invoice();

        /// <summary>
        /// Список контрагентов
        /// </summary>
        private List<Counterparty> Counterparties = new List<Counterparty>();

        /// <summary>
        /// Список не аннулированного товара
        /// </summary>
        private List<Product> ProductList = new List<Product>();

        /// <summary>
        /// Окно создания / редактирования накладной
        /// </summary>
        /// <param name="isPurchase">TRUE - приходная накладная, FALSE - расходная накладная</param>
        /// <param name="id">ID редактируемой накладной, если 0, создается новая накладная</param>
        public CreateInvoiceWindow(bool isPurchase, int id=0)
        {
            InitializeComponent();

            this.Title = (isPurchase) ? "Приходная накладная" : "Расходная накладная";

            if(id == 0)
            {
                // Создание накладной
                CurrentInvoice = Invoice.CreateInvoice(isPurchase);
            }
            else
            {
                // Загрузка накладной
                CurrentInvoice = Invoice.GetInvoiceByID(isPurchase, id);

                tbTotalCost.Text = String.Format(@"{0:#.##}", CurrentInvoice.Cost).Replace(',','.');
                tbTotalCoupon.Text = String.Format(@"{0:#.##}", CurrentInvoice.Coupon).Replace(',', '.');
            }

            // Загрузка контрагентов
            if (isPurchase)
                Counterparties = Counterparty.GetProviderList().OrderBy(c => c.VisibleName).ToList();
            else
                Counterparties = Counterparty.GetCustomerList().OrderBy(c => c.VisibleName).ToList();

            foreach (Counterparty customer in Counterparties)
                cbCounterparty.Items.Add(customer.VisibleName);

            if (CurrentInvoice.ID_Counterparty != 0)
            {
                Counterparty currCounterparty = Counterparties.FirstOrDefault(c => c.ID == CurrentInvoice.ID_Counterparty);

                if (currCounterparty != null)
                    cbCounterparty.SelectedItem = currCounterparty.VisibleName;
                else
                    cbCounterparty.SelectedIndex = 0;
            }
            else
                cbCounterparty.SelectedIndex = 0;

            // Загрузка списка товара
            ProductList = Product.GetActualProductList();

            GetFreeProductList();

            tbExecutor.Text = Account.GetAccountByID(CurrentInvoice.ID_Account).PersonalData.VisibleName;

            if (CurrentInvoice.Date != DateTime.MinValue)
                tbDate.Text = String.Format(@"{0}.{1}.{2}", (CurrentInvoice.Date.Day < 10) ? "0" + CurrentInvoice.Date.Day.ToString() : CurrentInvoice.Date.Day.ToString(), (CurrentInvoice.Date.Month < 10) ? "0" + CurrentInvoice.Date.Month.ToString() : CurrentInvoice.Date.Month.ToString(), CurrentInvoice.Date.Year);

            dataGrid.ItemsSource = CurrentInvoice.Items;
        }

        /// <summary>
        /// Получить список доступного товара
        /// </summary>
        private void GetFreeProductList()
        {
            cbProduct.Items.Clear();

            // Заполнение списка не использованного товара
            foreach (Product product in ProductList)
            {
                InvoiceItem item = CurrentInvoice.Items.FirstOrDefault(i => i.ID_Product == product.ID);

                if (item == null)
                    cbProduct.Items.Add(product.ProductCode);
            }
        }

        /// <summary>
        /// Ввод текста в количество
        /// </summary>
        private void TextBlock_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = NumericTextBoxMethods.DoubleTextBox_PreviewKeyDown(tbCount.Text, e);
        }

        /// <summary>
        /// Изменение количества
        /// </summary>
        private void TextBlock_TextChanged(object sender, TextChangedEventArgs e)
        {
            NumericTextBoxMethods.DoubleTextBox_TextChanged(tbCount);
        }
    }
}