using System.Data;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using RGRv5.Models.Tests;

namespace RGRv5
{
    public class DB
    {
        MySqlConnection connection = new MySqlConnection("server=127.0.0.1;port=3306;username=root;password=1234;database=tests");

        public void OpenConnection()
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
        }
        public void CloseConnection()
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

        public MySqlConnection GetConnection()
        {
            return connection;
        }
    }
}
