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
        private double newValue = 0;
        /// <summary>
        /// Новое значение количества
        /// </summary>
        public double NewValue
        {
            get { return newValue; }
        }

        /// <summary>
        /// Максимальное количество
        /// </summary>
        private double max = -1;

        /// <summary>
        /// Окно редактирования количества товара в накладной
        /// </summary>
        /// <param name="currentValue">текущее количество</param>
        /// <param name="productCode">код товара</param>
        /// <param name="nomination">наименование товара</param>
        public EditItemCountWindow(double currentValue, string productCode, string nomination)
        {
            InitializeComponent();

            tbCount.Text = Conversion.ToString(currentValue);
            tbCode.Text = productCode;
            tbNomination.Text = nomination;
        }

        /// <summary>
        /// Окно редактирования количества товара в накладной
        /// </summary>
        /// <param name="currentValue">текущее количество</param>
        /// <param name="productCode">код товара</param>
        /// <param name="nomination">наименование товара</param>
        /// <param name="maxCount">максимальное количество</param>
        public EditItemCountWindow(double currentValue, string productCode, string nomination, double maxCount) :
            this(currentValue, productCode, nomination)
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

            newValue = Conversion.ToDouble(tbCount.Text.Trim());

            if(newValue <= 0)
            {
                Dialog.WarningMessage(this, "Не корректное количество");
                return;
            }

            if(max != -1 && newValue > max)
            {
                Dialog.WarningMessage(this, "Указанное количество превышает максимально допустимое");
                return;
            }

            this.DialogResult = true;
        }
    }
}
