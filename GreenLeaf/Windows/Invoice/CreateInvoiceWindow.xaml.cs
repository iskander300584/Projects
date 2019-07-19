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
using GreenLeaf.Classes;
using GreenLeaf.ViewModel;

namespace GreenLeaf.Windows.Invoice
{
    /// <summary>
    /// Окно создания / редактирования накладной
    /// </summary>
    public partial class CreateInvoiceWindow : Window
    {
        /// <summary>
        /// ID редактируемой накладной
        /// </summary>
        private int ID = 0;

        /// <summary>
        /// Признак приходной накладной
        /// </summary>
        private bool IsPurchase = true;

        /// <summary>
        /// Редактируемая накладная
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
        /// Свободные коды товара
        /// </summary>
        private List<string> FreeProductCodes = new List<string>();

        /// <summary>
        /// Использованные коды товара
        /// </summary>
        private List<string> UsedProductCodes = new List<string>();

        /// <summary>
        /// Окно создания / редактирования накладной
        /// </summary>
        /// <param name="isPurchase">TRUE - приходная накладная, FALSE - расходная накладная</param>
        /// <param name="id">ID редактируемой накладной, если 0, создается новая накладная</param>
        public CreateInvoiceWindow(bool isPurchase, int id=0)
        {
            InitializeComponent();

            this.Title = (isPurchase) ? "Приходная накладная" : "Расходная накладная";

            IsPurchase = isPurchase;
            CurrentInvoice.IsPurchase = IsPurchase;

            ID = id;
            
            // Загрузка списка контрагентов
            LoadCounterparties();

            // Получение данных по редактируемой накладной
            if (ID != 0)
                LoadEditedPurchaseDate();

            // Загрузка списка товаров
            LoadProducts();

            // Загрузка исполнителя, клиента и даты
            LoadExecutor();

            dataGrid.ItemsSource = CurrentInvoice.Items;
        }

        /// <summary>
        /// Получение данных по редактируемой накладной
        /// </summary>
        private void LoadEditedPurchaseDate()
        {
            CurrentInvoice.ID = ID;
            CurrentInvoice.GetDataByID(IsPurchase);
        }

        /// <summary>
        /// Загрузить список клиентов
        /// </summary>
        private void LoadCounterparties()
        {
            Counterparties = Counterparty.GetCustomerList().OrderBy(c => c.VisibleName).ToList();

            foreach (Counterparty customer in Counterparties)
                cbCounterparty.Items.Add(customer.VisibleName);

            if (Counterparties.Count == 1)
                cbCounterparty.SelectedIndex = 0;
        }

        /// <summary>
        /// Загрузить данные о товаре
        /// </summary>
        private void LoadProducts()
        {
            ProductList = Product.GetActualProductList();

            // Заполнение списка использованного товара
            foreach (InvoiceItem item in CurrentInvoice.Items)
                UsedProductCodes.Add(item.Product.ProductCode);

            // Заполнение списка не использованного товара
            foreach (Product product in ProductList)
                if (!UsedProductCodes.Contains(product.ProductCode))
                {
                    FreeProductCodes.Add(product.ProductCode);
                    cbProduct.Items.Add(product.ProductCode);
                }
        }

        // TODO

        /// <summary>
        /// Загрузить данные об исполнителе, клиенте и дате
        /// </summary>
        private void LoadExecutor()
        {

        }

        /// <summary>
        /// Ввод текста в количество
        /// </summary>
        private void TextBlock_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = NumericTextBoxMethods.DoubleTextBox_PreviewKeyDown(tbCount.Text, e);
        }

        // TODO расчет суммы

        /// <summary>
        /// Изменение количества
        /// </summary>
        private void TextBlock_TextChanged(object sender, TextChangedEventArgs e)
        {
            NumericTextBoxMethods.DoubleTextBox_TextChanged(tbCount);
        }
    }
}
