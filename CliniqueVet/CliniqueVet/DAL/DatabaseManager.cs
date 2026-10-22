using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CliniqueVet.DAL
{
    public static class DatabaseManager
    {
        private static string _connectionString = "";

        public static string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public static bool TestConnection(string server, string database)
        {
            try
            {
                string testCS = string.Format(
                    "Server={0};Database={1};Integrated Security=True;Connect Timeout=5;",
                    server, database);
                using (SqlConnection conn = new SqlConnection(testCS))
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

        public static void Initialize(string server, string database)
        {
            _connectionString = string.Format(
                "Server={0};Database={1};Integrated Security=True;",
                server, database);
        }
    }
}
