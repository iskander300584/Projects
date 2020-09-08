using System;
using System.Windows;
using Microsoft.Win32;
using GreenLeaf.Classes;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MySql.Data.MySqlClient;

namespace GreenLeaf.Windows.Authentificate
{
    /// <summary>
    /// Окно выбора файла подключения
    /// </summary>
    public partial class AddConnectionKeyWindow : Window
    {
        /// <summary>
        /// Окно выбора файла подключения
        /// </summary>
        public AddConnectionKeyWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Выбор файла подключения
        /// </summary>
        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Ключи подключения|*.gcn";
            dialog.FilterIndex = 0;
            dialog.FileName = "";
            dialog.Title = "Выбор файла подключения";
            dialog.Multiselect = false;
            dialog.CheckFileExists = true;
            dialog.ShowReadOnly = true;

            if (!(bool)dialog.ShowDialog())
                return;

            // TODO добавить обновление элемента
            tbPath.Text = dialog.FileName;

            using (FileStream fs = new FileStream(dialog.FileName, FileMode.Open))
            {
                BinaryFormatter serializer = new BinaryFormatter();
                ConnectionKey key = (ConnectionKey)serializer.Deserialize(fs);
                fs.Close();

                try
                {
                    using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(key.ConnectionString)))
                    {
                        connection.Open();
                        connection.Close();
                    }
                }
                catch(Exception ex)
                {
                    Dialog.ErrorMessage(this, "Ошибка подключения к базе данных", ex.Message);
                    return;
                }

                ProgramSettings.ConnectionString = key.ConnectionString;

                // Сохранение настроек
                try
                {
                    SaveConnectionData saveData = new SaveConnectionData();
                    saveData.ConnectionString = key.ConnectionString;
                    saveData.Owner = Criptex.UnCript(key.Owner);

                    using (FileStream fsSave = new FileStream(ProgramSettings.WorkFolder + "conncfg.plg", FileMode.Create))
                    {
                        BinaryFormatter serializerSave = new BinaryFormatter();
                        serializerSave.Serialize(fsSave, saveData);
                        fsSave.Close();
                    }

                    this.DialogResult = true;
                }
                catch(Exception ex)
                {
                    Dialog.ErrorMessage(this, "Ошибка сохранения настроек подключения", ex.Message);
                }
            }
        }
    }
}