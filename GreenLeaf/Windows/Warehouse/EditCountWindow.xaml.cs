using System.Windows;
using GreenLeaf.Classes;
using System.Linq;
using System.Windows.Input;
using System.Windows.Controls;

namespace GreenLeaf.Windows.Warehouse
{
    /// <summary>
    /// Окно редактирования количества
    /// </summary>
    public partial class EditCountWindow : Window
    {
        public double Count = 0;

        public int ID_Unit = 0;

        /// <summary>
        /// Окно редактирования количества
        /// </summary>
        public EditCountWindow(double count, int id_unit)
        {
            InitializeComponent();

            Count = count;
            tbCount.Text = Count.ToString();

            ID_Unit = id_unit;

            foreach (var pair in MeasureUnit.Units)
                cbUnit.Items.Add(pair.Value);

            if (MeasureUnit.Units.Keys.Contains(ID_Unit))
                cbUnit.SelectedItem = MeasureUnit.Units[ID_Unit];
            else
                cbUnit.SelectedIndex = 0;
        }

        /// <summary>
        /// Кнопка Сохранить
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            double temp = 0;
            if(!double.TryParse(tbCount.Text.Trim().Replace('.',','), out temp))
            {
                Dialog.WarningMessage(this, "Не корректное значение количества");
                return;
            }

            if(temp < 0)
            {
                Dialog.WarningMessage(this, "Количество не может быть меньше 0");
                return;
            }

            if(cbUnit.SelectedItem == null)
            {
                Dialog.WarningMessage(this, "Не указана единица измерения");
                return;
            }

            if(!MeasureUnit.Units.Values.Contains(cbUnit.SelectedItem.ToString()))
            {
                Dialog.WarningMessage(this, "Единица измерения не зарегистрирована в справочнике");
                return;
            }

            Count = temp;

            foreach(var pair in MeasureUnit.Units)
                if(pair.Value == cbUnit.SelectedItem.ToString())
                {
                    ID_Unit = pair.Key;
                    break;
                }

            this.DialogResult = true;
        }

        /// <summary>
        /// Попытка ввода количества
        /// </summary>
        private void tbCount_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = NumericTextBoxMethods.DoubleTextBox_PreviewKeyDown(tbCount.Text, e);
        }

        /// <summary>
        /// Ввод количества
        /// </summary>
        private void tbCount_TextChanged(object sender, TextChangedEventArgs e)
        {
            NumericTextBoxMethods.DoubleTextBox_TextChanged(tbCount);
        }
    }
}
