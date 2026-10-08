using System;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace QLNhanVien2
{
    internal class DatabaseHelper
    {
        // Tự động đọc chuỗi kết nối từ appsettings.json ("Data Source=nhansu.db")
        private static string GetConnectionString()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            return configuration.GetConnectionString("DefaultConnection") ?? "Data Source=nhansu.db";
        }

        public static SqliteConnection GetConnection()
        {
            return new SqliteConnection(GetConnectionString());
        }

        public static bool TestConnection()
        {
            try
            {
                using (SqliteConnection conn = GetConnection())
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