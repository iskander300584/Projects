using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GreenLeaf.Classes;

namespace GreenLeaf.Windows.InvoiceView
{
    /// <summary>
    /// Окно редактирования количества товара в накладной
    /// </summary>
    public partial class EditItemCountWindow : Window
    {
        private double newCount = 0;
        /// <summary>
        /// Новое значение количества
        /// </summary>
        public double NewCount
        {
            get { return newCount; }
        }

        private double newCost = 0;
        /// <summary>
        /// Новая стоимость за единицу товара
        /// </summary>
        public double NewCost
        {
            get { return newCost; }
        }

        private double newCoupon = 0;
        /// <summary>
        /// Новый купон за единицу товара
        /// </summary>
        public double NewCoupon
        {
            get { return newCoupon; }
        }

        /// <summary>
        /// Максимальное количество
        /// </summary>
        private double max = -1;

        /// <summary>
        /// Доступно редактирование количества
        /// </summary>
        private bool toChangeCost = false;

        /// <summary>
        /// Окно редактирования количества товара в накладной
        /// </summary>
        /// <param name="currentValue">текущее количество</param>
        /// <param name="productCode">код товара</param>
        /// <param name="nomination">наименование товара</param>
        public EditItemCountWindow(double currentValue, string productCode, string nomination, bool changeCost, double cost = 0, double coupon = 0)
        {
            InitializeComponent();

            tbCount.Text = Conversion.ToString(currentValue);
            tbCode.Text = productCode;
            tbNomination.Text = nomination;

            if(changeCost)
            {
                toChangeCost = true;
                tbCost.Text = Conversion.ToString(cost);
                tbCoupon.Text = Conversion.ToString(coupon);
            }
            else
            {
                dpCost.Visibility = Visibility.Collapsed;
                dpCoupon.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Окно редактирования количества товара в накладной
        /// </summary>
        /// <param name="currentValue">текущее количество</param>
        /// <param name="productCode">код товара</param>
        /// <param name="nomination">наименование товара</param>
        /// <param name="maxCount">максимальное количество</param>
        public EditItemCountWindow(double currentValue, string productCode, string nomination, double maxCount, bool changeCost, double cost = 0, double coupon = 0) :
            this(currentValue, productCode, nomination, changeCost, cost, coupon)
        {
            tbMaxValue.Text = Conversion.ToString(maxCount);
            max = maxCount;

            tbMax.Visibility = Visibility.Visible;
            tbMaxValue.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Нажатие клавиши на TextBox
        /// </summary>
        private void TextBlock_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = NumericTextBoxMethods.DoubleTextBox_PreviewKeyDown(tbCount.Text, e);
        }

        /// <summary>
        /// Ввод текста в TextBox
        /// </summary>
        private void TextBlock_TextChanged(object sender, TextChangedEventArgs e)
        {
            NumericTextBoxMethods.DoubleTextBox_TextChanged(tbCount);
        }

        /// <summary>
        /// Нажатие кнопки Сохранить
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(tbCount.Text.Trim() == "")
            {
                Dialog.WarningMessage(this, "Не указано количество");
                return;
            }

            newCount = Conversion.ToDouble(tbCount.Text.Trim());

            if(newCount <= 0)
            {
                Dialog.WarningMessage(this, "Не корректное количество");
                return;
            }

            if(max != -1 && newCount > max)
            {
                Dialog.WarningMessage(this, "Указанное количество превышает максимально допустимое");
                return;
            }

            if(toChangeCost)
            {
                if (tbCost.Text.Trim() == "")
                {
                    Dialog.WarningMessage(this, "Не указана стоимость единицы товара");
                    return;
                }

                newCost = Conversion.ToDouble(tbCost.Text.Trim());

                if (newCost <= 0)
                {
                    Dialog.WarningMessage(this, "Не корректная стоимость единицы товара");
                    return;
                }


                if (tbCoupon.Text.Trim() == "")
                {
                    Dialog.WarningMessage(this, "Не указан купон за единицу товара");
                    return;
                }

                newCoupon = Conversion.ToDouble(tbCoupon.Text.Trim());

                if (newCoupon <= 0)
                {
                    Dialog.WarningMessage(this, "Не корректный купон за единицу товара");
                    return;
                }
            }

            this.DialogResult = true;
        }
    }
}
