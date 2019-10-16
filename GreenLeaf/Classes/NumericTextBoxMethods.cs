using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Controls;

namespace GreenLeaf.Classes
{
    /// <summary>
    /// Методы обработки данных для числовых TextBox
    /// </summary>
    public static class NumericTextBoxMethods
    {
        /// <summary>
        /// Список доступных кнопок для дробных чисел
        /// </summary>
        private static List<Key> DoubleDigits = new List<Key> { Key.D0, Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9,
                Key.NumPad0, Key.NumPad1, Key.NumPad2, Key.NumPad3, Key.NumPad4, Key.NumPad5, Key.NumPad6, Key.NumPad7, Key.NumPad8, Key.NumPad9, Key.Back, Key.Delete, Key.Left, Key.Right, Key.OemPeriod, Key.OemComma, Key.Decimal, Key.OemQuestion, Key.Tab, Key.LeftCtrl, Key.RightCtrl, Key.C, Key.V, Key.Enter };

        /// <summary>
        /// Список доступных кнопок для целых чисел
        /// </summary>
        private static List<Key> IntDigits = new List<Key> { Key.D0, Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9,
                Key.NumPad0, Key.NumPad1, Key.NumPad2, Key.NumPad3, Key.NumPad4, Key.NumPad5, Key.NumPad6, Key.NumPad7, Key.NumPad8, Key.NumPad9, Key.Back, Key.Delete, Key.Left, Key.Right, Key.Tab, Key.LeftCtrl, Key.RightCtrl, Key.C, Key.V, Key.Enter };

        /// <summary>
        /// Обработчик PreviewKeyDown для дробных значений
        /// <para>возвращает значение для e.Handle</para>
        /// </summary>
        /// <param name="text">текст в TextBox</param>
        /// <param name="e">параметр клавиатуры</param>
        /// <returns>возвращает значение для e.Handle</returns>
        public static bool DoubleTextBox_PreviewKeyDown(string text, KeyEventArgs e)
        {
            bool result = false;

            if ((!DoubleDigits.Contains(e.Key) || e.Key == Key.Space) || (e.Key == Key.C && !(e.KeyboardDevice.Modifiers == ModifierKeys.Control)) || (e.Key == Key.V && !(e.KeyboardDevice.Modifiers == ModifierKeys.Control)))
                result = true;

            if (e.Key == Key.OemPeriod || e.Key == Key.OemComma || e.Key == Key.Decimal || e.Key == Key.OemQuestion)
            {
                if (text.Contains("."))
                    result = true;
            }

            return result;
        }

        /// <summary>
        /// Обработчик PreviewKeyDown для целых значений
        /// <para>возвращает значение для e.Handle</para>
        /// </summary>
        /// <param name="text">текст в TextBox</param>
        /// <param name="e">параметр клавиатуры</param>
        /// <returns>возвращает значение для e.Handle</returns>
        public static bool IntTextBox_PreviewKeyDown(string text, KeyEventArgs e)
        {
            bool result = false;

            if ((!IntDigits.Contains(e.Key) || e.Key == Key.Space) || (e.Key == Key.C && !(e.KeyboardDevice.Modifiers == ModifierKeys.Control)) || (e.Key == Key.V && !(e.KeyboardDevice.Modifiers == ModifierKeys.Control)))
                result = true;

            return result;
        }

        /// <summary>
        /// Обработчик TextChanged для дробных значений
        /// <para>возвращает измененный текст для TextBox</para>
        /// </summary>
        /// <param name="text">текст в TextBox</param>
        /// <returns>возвращает измененный текст для TextBox</returns>
        public static void DoubleTextBox_TextChanged(TextBox textBox)
        {
            if (textBox.Text != "")
            {
                textBox.Text = textBox.Text.Replace(',', '.');
                textBox.Text = textBox.Text.Replace('?', '.');
                textBox.Text = textBox.Text.Replace('/', '.');
                textBox.Text = textBox.Text.Replace('ю', '.');
                textBox.Text = textBox.Text.Replace('Ю', '.');
                textBox.Text = textBox.Text.Replace('б', '.');
                textBox.Text = textBox.Text.Replace('Б', '.');
                textBox.Text = textBox.Text.Replace('>', '.');
                textBox.SelectionStart = textBox.Text.Length;
                textBox.SelectionLength = textBox.Text.Length;
            }
        }
    }
}
