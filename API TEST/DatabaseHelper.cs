using System;
using System.Data.SqlClient;

namespace QLNhanVien2
{
    internal class DatabaseHelper
    {
        private static string connectionString =
            @"Server=DESKTOP-QKQN78V\MSSQLSERVER03;
              Database=QLNhanVien2;
              Integrated Security=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

        public static bool TestConnection()
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}