using System;
using System.Windows.Forms;
using System.Drawing;
using Testing.Database;
using Testing.Models;
using MySql.Data.MySqlClient;
using System.Data;

namespace Testing.Forms
{
    public partial class LoginForm : Form
    {
        private DatabaseConnection db;

        public LoginForm()
        {
            InitializeComponent();
            db = DatabaseConnection.Instance;

            // FIX 1: Removed the manual "btnLogin.Click +=" line because 
            // your Designer file is already doing it (that caused the error).

            chkShowPassword.CheckedChanged += ChkShowPassword_CheckedChanged;
            txtPassword.KeyPress += TxtPassword_KeyPress;

            this.Load += LoginForm_Load;
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            // Optional: Test connection on load
        }

        // FIX 2: Renamed this method to 'btnLogin_Click_1' to match your error message
        private void btnLogin_Click_1(object sender, EventArgs e)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Please enter your username.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please enter your password.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            // Attempt login
            PerformLogin();
        }

        private void PerformLogin()
        {
            // We use a manual connection here to ensure the Adapter works correctly
            using (MySqlConnection con = DatabaseConnection.Instance.GetConnection())
            {
                try
                {
                    con.Open();

                    string query = @"SELECT UserId, Username, FullName, Role, IsActive 
                                   FROM Users 
                                   WHERE Username = @username 
                                   AND Password = @password 
                                   AND IsActive = 1";

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@username", txtUsername.Text.Trim());
                        cmd.Parameters.AddWithValue("@password", txtPassword.Text);

                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            var row = dt.Rows[0];
                            int userId = Convert.ToInt32(row["UserId"]);
                            string username = row["Username"].ToString();
                            string fullName = row["FullName"].ToString();
                            string role = row["Role"].ToString();

                            UpdateLastLogin(userId);
                            LogActivity(userId, "Login", $"User {username} logged in");

                            MessageBox.Show($"Welcome, {fullName}!", "Login Successful",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            this.Hide();

                            // Open appropriate form based on role
                            switch (role)
                            {
                                case "Admin":
                                    var adminDashboard = new AdminDashboard(userId, fullName);
                                    adminDashboard.ShowDialog();
                                    break;
                                case "Cashier":
                                    var cashierDashboard = new CashierDashboard(userId, fullName);
                                    cashierDashboard.ShowDialog();
                                    break;
                                case "InventoryClerk":
                                    var clerkDashboard = new InventoryClerkDashboard(userId, fullName);
                                    clerkDashboard.ShowDialog();
                                    break;
                                default:
                                    MessageBox.Show("Role not recognized: " + role, "Login Error");
                                    break;
                            }
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Invalid username or password.", "Login Failed",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtPassword.Clear();
                            txtPassword.Focus();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Login error: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void UpdateLastLogin(int userId)
        {
            try
            {
                using (MySqlConnection con = DatabaseConnection.Instance.GetConnection())
                {
                    con.Open();
                    string query = "UPDATE Users SET LastLogin = NOW() WHERE UserId = @userId";
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating last login: {ex.Message}");
            }
        }

        private void LogActivity(int userId, string action, string description)
        {
            try
            {
                using (MySqlConnection con = DatabaseConnection.Instance.GetConnection())
                {
                    con.Open();
                    string query = @"INSERT INTO ActivityLogs (UserId, Action, Description) 
                                   VALUES (@userId, @action, @description)";
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        cmd.Parameters.AddWithValue("@action", action);
                        cmd.Parameters.AddWithValue("@description", description);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging activity: {ex.Message}");
            }
        }

        private void ChkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.PasswordChar = chkShowPassword.Checked ? '\0' : '●';
        }

        private void TxtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                // We call the new function name here too
                btnLogin_Click_1(sender, e);
            }
        }

        private void panelLeft_Paint(object sender, PaintEventArgs e)
        {
        }

        private void txtUsername_TextChanged(object sender, EventArgs e)
        {
        }

        private void panelRight_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}