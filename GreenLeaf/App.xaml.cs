using System;
using System.Windows;
using System.Reflection;
using GreenLeaf.Windows.Authentificate;
using System.IO;
using GreenLeaf.Classes;
using System.Runtime.Serialization.Formatters.Binary;
using MySql.Data.MySqlClient;
using GreenLeaf.Windows;

namespace GreenLeaf
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        App()
        {
            InitializeComponent();
        }

        [STAThread]
        static void Main()
        {
            App app = new App();

            Assembly assembly = typeof(App).Assembly;

            SplashWindow splash = new SplashWindow("СЦ Тольятти", "Ряполов А.Н.", assembly);
            splash.Show();
            DateTime dtStartSplash = DateTime.Now;

            // Проверка наличия рабочей директории
            if(!Directory.Exists(ConnectSetting.WorkFolder))
            {
                Directory.CreateDirectory(ConnectSetting.WorkFolder);
            }

            // Вызов окна настроек подключения
            if(!File.Exists(ConnectSetting.WorkFolder + "conncfg.plg"))
            {
                ConnectWindow connWindow = new ConnectWindow();
                if(!(bool)connWindow.ShowDialog())
                {
                    connWindow.Close();
                    Environment.Exit(0);
                    return;
                }
                connWindow.Close();
            }
            // Получение настроек подключения
            else
            {
                try
                {
                    using (FileStream fs = new FileStream(ConnectSetting.WorkFolder + "conncfg.plg", FileMode.Open))
                    {
                        BinaryFormatter serializer = new BinaryFormatter();
                        SaveConnectionData saveData = (SaveConnectionData)serializer.Deserialize(fs);
                        ConnectSetting.Server = Criptex.UnCript(saveData.Server);
                        ConnectSetting.DB = Criptex.UnCript(saveData.DB);
                        ConnectSetting.AdminLogin = Criptex.UnCript(saveData.AdminLogin);
                        ConnectSetting.AdminPassword = saveData.AdminPassword;
                    }

                    MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString));
                    connection.Open();
                    connection.Close();
                    connection.Dispose();
                    connection = null;
                }
                catch
                {
                    // Запрос настроек подключения
                    ConnectWindow connWindow = new ConnectWindow();
                    if (!(bool)connWindow.ShowDialog())
                    {
                        connWindow.Close();
                        Environment.Exit(0);
                        return;
                    }
                    connWindow.Close();
                }
            }

            // Запуск окна авторизации
            AuthentificateWindow authWindow = new AuthentificateWindow();
            if(!(bool)authWindow.ShowDialog())
            {
                authWindow.Close();
                Environment.Exit(0);
                return;
            }

            ConnectSetting.CurrentUser.Login = authWindow.UserName;
            authWindow.Close();

            ConnectSetting.CurrentUser.GetBaseDataByLogin();
            ConnectSetting.CurrentUser.GetPublicDataByID();

            GetSettings();

            MainWindow mainWindow = new MainWindow(splash, dtStartSplash);
            Current.MainWindow = mainWindow;
            Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            app.Run(mainWindow);
        }

        /// <summary>
        /// Получение настроек программы из БД
        /// </summary>
        private static void GetSettings()
        {
            using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
            {
                string sql = @"SELECT * FROM `SETTINGS`";

                connection.Open();

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            string nomination = string.Empty;
                            string value = string.Empty;

                            try
                            {
                                nomination = reader["NOMINATION"].ToString();
                            }
                            catch
                            {
                                nomination = "";
                            }

                            try
                            {
                                value = reader["VALUE"].ToString();
                            }
                            catch
                            {
                                value = string.Empty;
                            }

                            if(nomination != "")
                            {
                                ConnectSetting.ProgramSettings.Add(nomination, value);
                            }
                        }
                    }
                }

                connection.Close();
            }
        }
    }
}
