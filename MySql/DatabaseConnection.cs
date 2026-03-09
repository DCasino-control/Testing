using System;
using MySql.Data.MySqlClient;
using System.Data;
using System.Windows.Forms;

namespace Testing.Database
{
    public class DatabaseConnection
    {
        private static DatabaseConnection _instance = null;
        private MySqlConnection connection;
        // UPDATE THIS STRING IF NEEDED
        private string connectionString = "server=localhost;user=root;database=umvc_canteen_db;port=3306;password=;";

        private DatabaseConnection()
        {
            connection = new MySqlConnection(connectionString);
        }

        public static DatabaseConnection Instance
        {
            get
            {
                if (_instance == null) _instance = new DatabaseConnection();
                return _instance;
            }
        }

        // 1. Used by LoginForm and CheckoutForm
        public MySqlConnection GetConnection()
        {
            return connection;
        }

        public bool TestConnection()
        {
            try
            {
                if (connection.State == ConnectionState.Closed) connection.Open();
                return true;
            }
            catch (Exception) { return false; }
            finally { if (connection.State == ConnectionState.Open) connection.Close(); }
        }

        // 2. Used by Grids (ProductManagement, Reports)
        public DataTable ExecuteQuery(string query, MySqlParameter[] parameters = null)
        {
            DataTable dt = new DataTable();
            try
            {
                if (connection.State == ConnectionState.Closed) connection.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open) connection.Close();
            }
            return dt;
        }

        // 3. Used by Inserts/Updates (UserManagement, Inventory)
        public int ExecuteNonQuery(string query, MySqlParameter[] parameters = null)
        {
            int rowsAffected = 0;
            try
            {
                if (connection.State == ConnectionState.Closed) connection.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Action Failed: " + ex.Message);
                return -1;
            }
            finally
            {
                if (connection.State == ConnectionState.Open) connection.Close();
            }
            return rowsAffected;
        }

        // 4. Used by CashierDashboard and Legacy Code
        public MySqlDataReader ExecuteReader(string query, MySqlParameter[] parameters = null)
        {
            try
            {
                if (connection.State == ConnectionState.Closed) connection.Open();
                MySqlCommand cmd = new MySqlCommand(query, connection);
                if (parameters != null) cmd.Parameters.AddRange(parameters);

                // CommandBehavior.CloseConnection ensures the connection closes when the Reader closes
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Reader Error: " + ex.Message);
                if (connection.State == ConnectionState.Open) connection.Close();
                return null;
            }
        }
    }
}