using System;
using System.Windows;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using GreenLeaf.Classes;
using MySql.Data.MySqlClient;

namespace GreenLeaf.Windows.Authentificate
{
    /// <summary>
    /// Окно авторизации
    /// </summary>
    public partial class AuthentificateWindow : Window
    {
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string UserName = string.Empty;

        /// <summary>
        /// Окно авторизации
        /// </summary>
        public AuthentificateWindow()
        {
            InitializeComponent();

            try
            {
                if (File.Exists(ConnectSetting.WorkFolder + "settings.plg"))
                {
                    using (FileStream fs = new FileStream(ConnectSetting.WorkFolder + "settings.plg", FileMode.Open))
                    {
                        BinaryFormatter serializer = new BinaryFormatter();
                        AuthentificateSettings auth = (AuthentificateSettings)serializer.Deserialize(fs);

                        tbLogin.Text = Criptex.UnCript(auth.UserName);
                    }

                    pbPassword.Focus();
                }
                else
                    tbLogin.Focus();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Кнопка Применить
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            if(tbLogin.Text.Trim() == "")
            {
                Dialog.WarningMessage(this, "Не указано имя пользователя");
                return;
            }

            try
            {
                bool accept = false;

                using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                {
                    connection.Open();

                    string sql = "SELECT * FROM ACCOUNT WHERE LOGIN = \'" + tbLogin.Text.Trim() + "\'";
                    MySqlCommand command = new MySqlCommand(sql, connection);

                    MySqlDataReader reader = command.ExecuteReader();
                    if(reader == null)
                    {
                        Dialog.WarningMessage(this, "Учетная запись пользователя не зарегистрирована");
                        return;
                    }

                    while (reader.Read())
                    {
                        if (Criptex.UnCript(reader["PASSWORD"].ToString()) == pbPassword.Password.Trim())
                        {
                            accept = true;
                            break;
                        }
                    }

                    connection.Close();
                }

                if(!accept)
                {
                    Dialog.WarningMessage(this, "Не верно указано имя пользователя или пароль");
                    return;
                }

                UserName = tbLogin.Text.Trim();

                // Сохранение настроек аутентификации
                try
                {
                    AuthentificateSettings auth = new AuthentificateSettings();
                    auth.UserName = Criptex.Cript(tbLogin.Text.Trim());

                    using (FileStream fs = new FileStream(ConnectSetting.WorkFolder + "settings.plg", FileMode.Create))
                    {
                        BinaryFormatter serializer = new BinaryFormatter();
                        serializer.Serialize(fs, auth);
                    }
                }
                catch { }

                this.DialogResult = true;
            }
            catch(Exception ex)
            {
                Dialog.ErrorMessage(this, "Ошибка аутентификации", ex.Message);
                return;
            }
        }
    }
}
