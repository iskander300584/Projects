using GreenLeaf.Classes;
using GreenLeaf.ViewModel;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GreenLeaf.Windows.AdminPanel
{
    /// <summary>
    /// Окно настроек программы
    /// </summary>
    public partial class AdminSettingsWindow : Window
    {
        #region Поля класса

        /// <summary>
        /// Контекст данных окна
        /// </summary>
        private SettingsContext context;

        /// <summary>
        /// Значение нумератора приходных накладных
        /// </summary>
        private int PurshaseValue = 0;

        /// <summary>
        /// Значение нумератора расходных накладных
        /// </summary>
        private int SalesValue = 0;

        #endregion

        /// <summary>
        /// Окно настроек программы
        /// </summary>
        public AdminSettingsWindow()
        {
            InitializeComponent();

            context = new SettingsContext();

            // Получение нумераторов            
            foreach(Numerator numerator in Numerator.GetNumerators())
            {
                if(numerator.Nomination == "Приходная накладная")
                {
                    context.NumeratorPurchase_ID = numerator.ID;
                    context.NumeratorPurchase_Value = numerator.Value;
                    PurshaseValue = numerator.Value;
                }
                else if(numerator.Nomination == "Расходная накладная")
                {
                    context.NumeratorSales_ID = numerator.ID;
                    context.NumeratorSales_Value = numerator.Value;
                    SalesValue = numerator.Value;
                }
            }

            // Получение настроек программы
            context.SettingsCollection = new Dictionary<string, string>();

            foreach(var pair in ProgramSettings.Settings)
            {
                context.SettingsCollection.Add(pair.Key, pair.Value);
            }

            this.DataContext = context;
        }

        /// <summary>
        /// Предварительная обработка ввода данных в числовое поле
        /// </summary>
        private void TextBlock_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = NumericTextBoxMethods.IntTextBox_PreviewKeyDown(((TextBox)sender).Text, e);
        }

        /// <summary>
        /// Обработка ввода данных в числовое поле
        /// </summary>
        private void TextBlock_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = NumericTextBoxMethods.IntTextBox_PreviewKeyDown(((TextBox)sender).Text, e);
        }

        /// <summary>
        /// Нажатие кнопки "Применить"
        /// </summary>
        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            bool changed = false;

            Mouse.OverrideCursor = Cursors.Wait;

            try
            {
                // Сохранение нумератора приходных накладных
                if (context.NumeratorPurchase_Value != PurshaseValue)
                {
                    if (Numerator.SetNumeratorValue(context.NumeratorPurchase_Value, context.NumeratorPurchase_ID))
                    {
                        PurshaseValue = context.NumeratorPurchase_Value;
                        changed = true;
                    }
                }

                // Сохранение нумератора расходных накладных
                if (context.NumeratorSales_Value != SalesValue)
                {
                    if (Numerator.SetNumeratorValue(context.NumeratorPurchase_Value, context.NumeratorPurchase_ID))
                    {
                        SalesValue = context.NumeratorSales_Value;
                        changed = true;
                    }
                }

                // Сохранение настроек
                if (context.SettingsCollection.Any(p => p.Value != ProgramSettings.Settings[p.Key]))
                {
                    using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ProgramSettings.ConnectionString)))
                    {
                        connection.Open();

                        foreach (var pair in context.SettingsCollection)
                        {
                            if (pair.Value != ProgramSettings.Settings[pair.Key])
                            {
                                string sql = String.Format(@"UPDATE `SETTINGS` SET `VALUE` = '{0}' WHERE `SETTINGS`.`NOMINATION` = '{1}'", pair.Value, pair.Key);

                                using (MySqlCommand command = new MySqlCommand(sql, connection))
                                {
                                    command.ExecuteNonQuery();
                                }

                                ProgramSettings.Settings[pair.Key] = pair.Value;

                                changed = true;
                            }
                        }

                        connection.Close();
                    }
                }

                if (changed)
                    Dialog.TransparentMessage(this, "Настройки сохранены");
            }
            catch(Exception ex)
            {
                Mouse.OverrideCursor = null;
                Dialog.ErrorMessage(this, "Ошибка сохранения настроек", ex.Message);
            }

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Двойной клик по настройке
        /// </summary>
        private void Setting_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(lvSettings.SelectedItem != null)
            {
                var pair = (KeyValuePair<string, string>)lvSettings.SelectedItem;

                AdminSetSettingWindow view = new AdminSetSettingWindow(pair.Key, pair.Value);
                view.Owner = this;

                if((bool)view.ShowDialog())
                {
                    context.SettingsCollection[pair.Key] = view.Setting;

                    lvSettings.Items.Refresh();
                }

                try
                {
                    view.Close();
                }
                catch { }
            }
        }
    }
}