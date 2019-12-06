using System.Windows;
using GreenLeaf.ViewModel;
using GreenLeaf.Classes;
using System.Linq;
using System.Windows.Input;
using System.Windows.Controls;

namespace GreenLeaf.Windows.Warehouse
{
    /// <summary>
    /// Добавление единицы товара
    /// </summary>
    public partial class AddProductWindow : Window
    {
        private Product product;

        private bool DoEdit;

        /// <summary>
        /// Окно добавления / редактирования единицы товара
        /// </summary>
        /// <param name="edit">редактирование единицы товара</param>
        /// <param name="id">ID редактируемого товара</param>
        public AddProductWindow(bool edit = false, int id = 0)
        {
            InitializeComponent();

            foreach (var pair in MeasureUnit.Units)
                cbUnit.Items.Add(pair.Value);

            cbUnit.SelectedIndex = 0;

            product = new Product();

            DoEdit = edit;

            if(edit)
            {
                product.ID = id;

                btnOkText.Text = "СОХРАНИТЬ";
                this.Title = "Редактирование товара";
                tbCaption.Text = "Редактирование товара";

                if (!product.GetDataByID())
                {
                    this.DialogResult = false;
                    return;
                }
                else if (MeasureUnit.Units.Keys.Contains(product.ID_Unit))
                    cbUnit.SelectedItem = MeasureUnit.Units[product.ID_Unit];

                tbCountInPackage.Text = Conversion.ToString(product.CountInPackage);
                tbCost.Text = Conversion.ToString(product.Cost);
                tbCoupon.Text = Conversion.ToString(product.Coupon);
                tbCostPurchase.Text = Conversion.ToString(product.CostPurchase);
                tbCouponPurchase.Text = Conversion.ToString(product.CouponPurchase);
            }

            this.DataContext = product;
        }

        // Нажатие кнопки Добавить
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Получение количества в упаковке
            double temp = 0;
            if(!double.TryParse(tbCountInPackage.Text.Replace('.',','), out temp))
            {
                Dialog.WarningMessage(this, "Не корректно указано количество в упаковке");
                return;
            }

            product.CountInPackage = temp;

            // Получение стоимости закупки
            if (!double.TryParse(tbCostPurchase.Text.Replace('.', ','), out temp))
            {
                Dialog.WarningMessage(this, "Не корректно указана стоимость закупки");
                return;
            }

            product.CostPurchase = temp;

            // Получение купона при реализации
            if (!double.TryParse(tbCouponPurchase.Text.Replace('.', ','), out temp))
            {
                Dialog.WarningMessage(this, "Не корректно указан купон при закупке");
                return;
            }

            product.CouponPurchase = temp;

            // Получение стоимости реализации
            if (!double.TryParse(tbCost.Text.Replace('.', ','), out temp))
            {
                Dialog.WarningMessage(this, "Не корректно указана стоимость при реализации");
                return;
            }

            product.Cost = temp;

            // Получение купона при реализации
            if (!double.TryParse(tbCoupon.Text.Replace('.', ','), out temp))
            {
                Dialog.WarningMessage(this, "Не корректно указан купон при реализации");
                return;
            }

            product.Coupon = temp;

            // Получение единицы измерения
            if (cbUnit.SelectedItem == null)
            {
                Dialog.WarningMessage(this, "Не указана единица измерения");
                return;
            }

            if (!MeasureUnit.Units.Values.Contains(cbUnit.SelectedItem.ToString()))
            {
                Dialog.WarningMessage(this, "Единица измерения не зарегистрирована в справочнике");
                return;
            }

            foreach (var pair in MeasureUnit.Units)
                if (pair.Value == cbUnit.SelectedItem.ToString())
                {
                    product.ID_Unit = pair.Key;
                    break;
                }

            if (!DoEdit)
                product.CreateProduct();
            else
                product.EditProduct();

            this.DialogResult = true;
        }

        /// <summary>
        /// Ввод текста в числовой TextBox
        /// </summary>
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            e.Handled = NumericTextBoxMethods.DoubleTextBox_PreviewKeyDown(tb.Text, e);
        }

        /// <summary>
        /// Изменение текста в числовом TextBox
        /// </summary>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            NumericTextBoxMethods.DoubleTextBox_TextChanged(tb);
        }
    }
}
