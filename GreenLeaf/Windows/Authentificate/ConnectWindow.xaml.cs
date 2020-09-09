using System;
using System.Windows;
using GreenLeaf.Classes;
using MySql.Data.MySqlClient;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace GreenLeaf.Windows.Authentificate
{
    // НЕ ИСПОЛЬЗУЕТСЯ
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
            //ProgramSettings.Server = tbServer.Text;
            //ProgramSettings.DB = tbDB.Text;
            //ProgramSettings.AdminLogin = tbLogin.Text;

            //ProgramSettings.AdminPassword = Criptex.Cript(pbPassword.Password);

            // Проверка подключения
            MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ProgramSettings.ConnectionString));

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
                //saveData.Server = Criptex.Cript(ProgramSettings.Server);
                //saveData.DB = Criptex.Cript(ProgramSettings.DB);
                //saveData.AdminLogin = Criptex.Cript(ProgramSettings.AdminLogin);
                //saveData.AdminPassword = ProgramSettings.AdminPassword;

                using (FileStream fs = new FileStream(ProgramSettings.WorkFolder + "conncfg.plg", FileMode.Create))
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
