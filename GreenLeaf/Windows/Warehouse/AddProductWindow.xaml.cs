using System;
using System.Windows;
using GreenLeaf.ViewModel;
using GreenLeaf.Classes;
using MySql.Data.MySqlClient;
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
            }

            tbCountInPackage.Text = product.CountInPackage.ToString().Replace(',', '.');
            tbCost.Text = product.Cost.ToString().Replace(',', '.');
            tbCoupon.Text = product.Coupon.ToString().Replace(',', '.');

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

            // Получение стоимости
            if (!double.TryParse(tbCost.Text.Replace('.', ','), out temp))
            {
                Dialog.WarningMessage(this, "Не корректно указана стоимость");
                return;
            }

            product.Cost = temp;

            // Получение купона
            if (!double.TryParse(tbCoupon.Text.Replace('.', ','), out temp))
            {
                Dialog.WarningMessage(this, "Не корректно указан купон");
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

            // Добавление единицы товара
            if (!DoEdit)
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                    {
                        connection.Open();

                        string sql = @"INSERT INTO `PRODUCT` (`PRODUCT_CODE`, `NOMINATION`, `COUNT_IN_PACKAGE`, `COST`, `COUPON`, `ID_UNIT`, `COUNT`) VALUES ('" +
                            product.ProductCode + @"', '" + product.Nomination + @"', '" + product.CountInPackage.ToString().Replace(',','.') + @"', '" +
                            product.Cost.ToString().Replace(',', '.') + @"', '" + product.Coupon.ToString().Replace(',', '.') + @"', '" + product.ID_Unit + "', '" + product.Count.ToString().Replace(',', '.') + @"');";

                        using (MySqlCommand command = new MySqlCommand(sql, connection))
                        {
                            command.ExecuteNonQuery();

                            string act = JournalMethods.JournalItemHeader("создал") + "новый вид товара " + product.ProductCode;
                            sql = @"INSERT INTO `JOURNAL` (`DATE`, `ID_ACCOUNT`, `ACT`) VALUES ('" + JournalMethods.CurrentDate() + @"', '" +
                                ConnectSetting.User.Person.ID.ToString() + @"', '" + act + @"');";

                            command.CommandText = sql;
                            command.ExecuteNonQuery();
                        }

                        connection.Close();
                    }

                    this.DialogResult = true;
                }
                catch (Exception ex)
                {
                    Dialog.ErrorMessage(this, "Ошибка сохранения данных", ex.Message);
                    return;
                }
            }
            // Редактирование товара
            else
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                    {
                        connection.Open();

                        string sql = String.Format("UPDATE `PRODUCT` SET `PRODUCT_CODE`=\'{0}\', `NOMINATION`=\'{1}\', `COUNT_IN_PACKAGE`=\'{2}\', `COST`=\'{3}\', `COUPON`=\'{4}\', `ID_UNIT`= \'{5}\' WHERE `PRODUCT`.`ID`={6}", product.ProductCode, product.Nomination, product.CountInPackage.ToString().Replace(',', '.'), product.Cost.ToString().Replace(',', '.'), product.Coupon.ToString().Replace(',', '.'), product.ID_Unit, product.ID);

                        using (MySqlCommand command = new MySqlCommand(sql, connection))
                        {
                            command.ExecuteNonQuery();

                            string act = JournalMethods.JournalItemHeader("изменил") + "вид товара " + product.ProductCode;
                            sql = @"INSERT INTO `JOURNAL` (`DATE`, `ID_ACCOUNT`, `ACT`) VALUES ('" + JournalMethods.CurrentDate() + @"', '" +
                                ConnectSetting.User.Person.ID.ToString() + @"', '" + act + @"');";

                            command.CommandText = sql;
                            command.ExecuteNonQuery();
                        }

                        connection.Close();
                    }

                    this.DialogResult = true;
                }
                catch (Exception ex)
                {
                    Dialog.ErrorMessage(this, "Ошибка сохранения данных", ex.Message);
                    return;
                }
            }
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
