using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using MySql.Data.MySqlClient;
using Microsoft.Win32;
using GreenLeaf.Classes;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace KeyGenerator
{
    /// <summary>
    /// Контекст данных программы
    /// </summary>
    public class Context: INotifyPropertyChanged
    {
        private string _server = "remotemysql.com";
        /// <summary>
        /// Имя сервера
        /// </summary>
        public string Server
        {
            get => _server;
            set
            {
                if(_server != value)
                {
                    _server = value;
                    OnPropertyChanged();

                    _isVerified = false;
                }
            }
        }

        private string _db = string.Empty;
        /// <summary>
        /// БД / Логин
        /// </summary>
        public string DB
        {
            get => _db;
            set
            {
                if(_db != value)
                {
                    _db = value;
                    OnPropertyChanged();

                    _isVerified = false;
                }
            }
        }

        private string _password = string.Empty;
        /// <summary>
        /// Пароль
        /// </summary>
        public string Password
        {
            get => _password;
            set
            {
                if(_password != value)
                {
                    _password = value;
                    OnPropertyChanged();

                    _isVerified = false;
                }
            }
        }

        private bool _isVerified = false;
        /// <summary>
        /// Соединение установлено
        /// </summary>
        public bool IsVerified
        {
            get => _isVerified;
            private set
            {
                if(_isVerified != value)
                {
                    _isVerified = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _owner = string.Empty;
        /// <summary>
        /// Владелец ключа
        /// </summary>
        public string Owner
        {
            get => _owner;
            set
            {
                if(_owner != value)
                {
                    _owner = value;
                    OnPropertyChanged();
                }
            }
        }

        // Изменение свойств объекта
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        /// <summary>
        /// Проверка подключения
        /// </summary>
        /// <returns></returns>
        public bool DoVerify()
        {
            if (_server.Trim() == "" || _db.Trim() == "" || _password.Trim() == "")
                return false;

            string _connectionString = "SERVER=" + _server.Trim() + ";" +
                                            "DATABASE=" + _db.Trim() + ";" +
                                            "UID=" + _db.Trim() + ";" +
                                            "Pwd=" + _password.Trim() + ";";

            // Проверка подключения
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    connection.Close();
                }

                IsVerified = true;
            }
            catch (Exception ex)
            {
                IsVerified = false;
                MessageBox.Show(ex.Message, "Ошибка подключения", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return _isVerified;
        }


        /// <summary>
        /// Генерировать ключ подлючения к БД
        /// </summary>
        public void DoGenerateFile()
        {
            if(_owner.Trim() == "")
            {
                MessageBox.Show("Не указан владелец ключа", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if(!_isVerified)
            {
                if (!DoVerify())
                    return;
            }

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Ключи подключения|*.gcn";
            dialog.FilterIndex = 0;
            dialog.FileName = _db;
            dialog.AddExtension = true;
            dialog.DefaultExt = "gcn";
            dialog.CheckPathExists = true;
            dialog.CheckFileExists = false;
            dialog.Title = "Выбор имени файла";

            if (!(bool)dialog.ShowDialog())
                return;

            string _connectionString = Criptex.Cript("SERVER=" + _server.Trim() + ";" +
                                            "DATABASE=" + _db.Trim() + ";" +
                                            "UID=" + _db.Trim() + ";" +
                                            "Pwd=" + _password.Trim() + ";");

            try
            {
                ConnectionKey key = new ConnectionKey
                {
                    ConnectionString = _connectionString,
                    Date = DateTime.Now,
                    Owner = Criptex.Cript(_owner.Trim())
                };

                using (FileStream fs = new FileStream(dialog.FileName, FileMode.Create))
                {
                    BinaryFormatter serializer = new BinaryFormatter();
                    serializer.Serialize(fs, key);
                    fs.Close();
                }

                MessageBox.Show("Генерация ключа выполнена успешно", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}