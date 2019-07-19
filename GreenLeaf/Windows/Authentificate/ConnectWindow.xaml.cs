using System;
using System.Windows;
using GreenLeaf.Classes;
using MySql.Data.MySqlClient;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace GreenLeaf.Windows.Authentificate
{
    /// <summary>
    /// Окно настроек подключения к БД
    /// </summary>
    public partial class ConnectWindow : Window
    {
        public string Server = string.Empty;

        public string DB = string.Empty;

        public string AdminLogin = string.Empty;

        public string AdminPassword = string.Empty;

        /// <summary>
        /// Окно настроек подключения к БД
        /// </summary>
        public ConnectWindow()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            // Получение настроек подключения
            ConnectSetting.Server = tbServer.Text;
            ConnectSetting.DB = tbDB.Text;
            ConnectSetting.AdminLogin = tbLogin.Text;

            ConnectSetting.AdminPassword = Criptex.Cript(pbPassword.Password);

            // Проверка подключения
            MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString));

            bool connected = true;

            try
            {
                connection.Open();
                connection.Close();
            }
            catch(Exception ex)
            {
                connected = false;
                Dialog.ErrorMessage(this, "Ошибка подключения", ex.Message);
            }

            connection = null;

            if(!connected)
            {
                return;
            }

            // Сохранение настроек
            try
            {
                SaveConnectionData saveData = new SaveConnectionData();
                saveData.Server = Criptex.Cript(ConnectSetting.Server);
                saveData.DB = Criptex.Cript(ConnectSetting.DB);
                saveData.AdminLogin = Criptex.Cript(ConnectSetting.AdminLogin);
                saveData.AdminPassword = ConnectSetting.AdminPassword;

                using (FileStream fs = new FileStream(ConnectSetting.WorkFolder + "conncfg.plg", FileMode.Create))
                {
                    BinaryFormatter serializer = new BinaryFormatter();
                    serializer.Serialize(fs, saveData);
                }

                this.DialogResult = true;
            }
            catch(Exception ex)
            {
                Dialog.ErrorMessage(this, "Ошибка сохранения настроек подключения", ex.Message);
                return;
            }
        }
    }
}
