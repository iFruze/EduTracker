using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

namespace WpfApp2
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            try 
            {
                string connectString = @"Data Source=localhost;Initial Catalog=master;Integrated Security=True";
                if (!DatabaseExists("TeachHours", connectString))
                {
                    string scriptPath = "script.sql"; // путь к вашему SQL-скрипту

                    string script = File.ReadAllText(scriptPath);

                    string[] commands = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

                    using (SqlConnection connection = new SqlConnection(connectString))
                    {
                        connection.Open();
                        foreach (string command in commands)
                        {
                            if (!string.IsNullOrWhiteSpace(command))
                            {
                                using (SqlCommand sqlCommand = new SqlCommand(command, connection))
                                {
                                    sqlCommand.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при развёртывании базы данных. Вероятнее всего у вас не установлен Sql Server. Пожалуйста, устанвоите его и повторите попытку.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                //MessageBox.Show(ex.Message);
                Shutdown();
            }
        }
        bool DatabaseExists(string databaseName, string connectionStringMaster)
        {
            using (SqlConnection connection = new SqlConnection(connectionStringMaster))
            {
                connection.Open();
                string query = $"SELECT database_id FROM sys.databases WHERE name = @name";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", databaseName);
                    object result = command.ExecuteScalar();
                    return (result != null);
                }
            }
        }
    }
}
