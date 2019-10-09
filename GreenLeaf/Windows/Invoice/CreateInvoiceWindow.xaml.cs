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
        #region Объявление команд

        /// <summary>
        /// Команда блокировки накладной
        /// </summary>
        public static RoutedUICommand DoLockInvoice = new RoutedUICommand("Блокировка накладной", "DoLockInvoice", typeof(CreateInvoiceWindow));

        /// <summary>
        /// Команда проведения накладной
        /// </summary>
        public static RoutedUICommand DoIssueInvoice = new RoutedUICommand("Проведение накладной", "DoIssueInvoice", typeof(CreateInvoiceWindow));

        /// <summary>
        /// Команда удаления накладной
        /// </summary>
        public static RoutedUICommand DoDeleteInvoice = new RoutedUICommand("Удаление накладной", "DoDeleteInvoice", typeof(CreateInvoiceWindow));

        /// <summary>
        /// Команда добавления элемента накладной
        /// </summary>
        public static RoutedUICommand DoAddItem = new RoutedUICommand("Добавление элемента накладной", "DoAddItem", typeof(CreateInvoiceWindow));

        /// <summary>
        /// Команда редактирования элемента накладной
        /// </summary>
        public static RoutedUICommand DoEditItem = new RoutedUICommand("Редактирование элемента накладной", "DoEditItem", typeof(CreateInvoiceWindow));

        /// <summary>
        /// Команда удаления элемента накладной
        /// </summary>
        public static RoutedUICommand DoDeleteItem = new RoutedUICommand("Удаление элемента накладной", "DoDeleteItem", typeof(CreateInvoiceWindow));

        #endregion

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
        /// Заполнить список доступного товара
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

        /// <summary>
        /// Проверка возможности блокировки накладной
        /// </summary>
        private void DoLockInvoice_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (CurrentInvoice != null && !CurrentInvoice.IsIssued) ? true : false;
        }

        /// <summary>
        /// Блокировка (разблокировка) накладной
        /// </summary>
        private void DoLockInvoice_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            bool res = false;

            if (CurrentInvoice.IsLocked)
                res = CurrentInvoice.UnLockInvoice();
            else
                res = CurrentInvoice.LockInvoice();

            if(res)
                Dialog.TransparentMessage(this, "Операция выполнена");

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Проверка возможности проведения накладной
        /// </summary>
        private void DoIssueInvoice_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (CurrentInvoice != null && !CurrentInvoice.IsLocked && (!CurrentInvoice.IsIssued || (CurrentInvoice.IsPurchase && ConnectSetting.CurrentUser.ReportsData.ReportUnIssuePurchaseInvoice) || (!CurrentInvoice.IsPurchase && ConnectSetting.CurrentUser.ReportsData.ReportUnIssueSalesInvoice))) ? true : false;
        }

        /// <summary>
        /// Проведение (отмена проведения) накладной
        /// </summary>
        private void DoIssueInvoice_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            bool res = false;

            if (CurrentInvoice.IsIssued)
            {
                if (Dialog.QuestionMessage(this, "Отменить проведение накладной?", "Внимание!") == MessageBoxResult.Yes)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    res = CurrentInvoice.UnIssueInvoice();
                }
            }
            else
            {
                if (Dialog.QuestionMessage(this, "Провести накладную?", "Внимание!") == MessageBoxResult.Yes)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    res = CurrentInvoice.IssueInvoice();
                }
            }

            if(res)
                Dialog.TransparentMessage(this, "Операция выполнена");

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Проверка возможности удаления накладной
        /// </summary>
        private void DoDeleteInvoice_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (CurrentInvoice != null && !CurrentInvoice.IsLocked && !CurrentInvoice.IsIssued) ? true : false;
        }

        /// <summary>
        /// Удаление накладной
        /// </summary>
        private void DoDeleteInvoice_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if(Dialog.QuestionMessage(this, "Удалить накладную?", "Внимание!") == MessageBoxResult.Yes)
            {
                Mouse.OverrideCursor = Cursors.Wait;

                if (CurrentInvoice.DeleteInvoice())
                {
                    Dialog.TransparentMessage(this, "Операция выполнена");

                    Mouse.OverrideCursor = null;
                    this.DialogResult = false;
                }

                Mouse.OverrideCursor = null;
            }
        }

        /// <summary>
        /// Проверка возможности добавления элемента накладной
        /// </summary>
        private void DoAddItem_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (CurrentInvoice != null && !CurrentInvoice.IsLocked && !CurrentInvoice.IsIssued) ? true : false;
        }

        /// <summary>
        /// Добавление элемента накладной
        /// </summary>
        private void DoAddItem_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            Product product = ProductList.FirstOrDefault(p => p.ProductCode == cbProduct.SelectedItem.ToString());
            if(product == null)
            {
                Dialog.WarningMessage(this, "Не удалось получить выбранный товар");
                return;
            }

            double count = 0;
            if(!double.TryParse(tbCount.Text.Trim(), out count))
            {
                Dialog.WarningMessage(this, "Ошибка получения количества товара");
                return;
            }

            Mouse.OverrideCursor = Cursors.Wait;

            CurrentInvoice.Items.Add(InvoiceItem.CreateItem(CurrentInvoice.ID, product.ID, count, CurrentInvoice.IsPurchase));

            GetFreeProductList();

            dataGrid.Items.Refresh();

            Dialog.TransparentMessage(this, "Операция выполнена");

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Проверка возможности редактирования элемента накладной
        /// </summary>
        private void DoEditItem_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (CurrentInvoice != null && !CurrentInvoice.IsLocked && !CurrentInvoice.IsIssued && dataGrid.SelectedItem != null) ? true : false;
        }

        /// <summary>
        /// Редактирование элемента накладной       TODO
        /// </summary>
        private void DoEditItem_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            
        }

        /// <summary>
        /// Проверка возможности удаления элемента накладной
        /// </summary>
        private void DoDeleteItem_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (CurrentInvoice != null && !CurrentInvoice.IsLocked && !CurrentInvoice.IsIssued && dataGrid.SelectedItem != null) ? true : false;
        }

        /// <summary>
        /// Удаление элемента накладной     TODO
        /// </summary>
        private void DoDeleteItem_Execute(object sender, ExecutedRoutedEventArgs e)
        {

        }
    }
}