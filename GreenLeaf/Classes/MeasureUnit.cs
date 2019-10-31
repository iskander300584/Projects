using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace GreenLeaf.Classes
{
    /// <summary>
    /// Единицы измерения
    /// </summary>
    public static class MeasureUnit
    {
        /// <summary>
        /// Словарь единиц измерения ID - Наименование
        /// </summary>
        public static Dictionary<int, string> Units = new Dictionary<int, string>();

        /// <summary>
        /// Получение единиц измерения
        /// </summary>
        public static void GetMeasureUnits()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(Criptex.UnCript(ConnectSetting.ConnectionString)))
                {
                    connection.Open();

                    string sql = "SELECT * FROM UNIT";

                    Units = new Dictionary<int, string>();

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        MySqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            Units.Add(int.Parse(reader["ID"].ToString()), reader["NOMINATION"].ToString());
                        }
                    }

                    connection.Close();
                }
            }
            catch(Exception ex)
            {
                Dialog.ErrorMessage(null, "Ошибка получения единиц измерения", ex.Message);
            }
        }
    }
}
