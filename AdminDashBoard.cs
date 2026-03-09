using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Testing.Database;

namespace Testing.Forms
{
    public partial class AdminDashboard : Form
    {
        private int currentUserId;
        private string currentUserName;
        private DatabaseConnection db;

        // UI Components
        private Panel panelTop;
        private Panel panelSidebar;
        private Panel panelMain;
        private Button btnProducts;
        private Button btnInventory;
        private Button btnSuppliers;
        private Button btnUsers;
        private Button btnReports;
        private Button btnPOS;
        private Button btnLogout;

        public AdminDashboard(int userId, string userName)
        {
            currentUserId = userId;
            currentUserName = userName;
            db = DatabaseConnection.Instance;

            InitializeComponent();
            ShowDashboardHome(); // This now loads REAL data from the database
        }

        private void InitializeComponent()
        {
            // Form Properties
            this.Text = "UMVC Canteen - Admin Dashboard";
            this.Size = new Size(1280, 720);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.AdminDashboard_Load);

            // Panels
            panelSidebar = new Panel { Dock = DockStyle.Left, Width = 250, BackColor = Color.FromArgb(52, 73, 94) };
            panelMain = new Panel { Dock = DockStyle.Fill, BackColor = Color.WhiteSmoke };

            this.Controls.Add(panelMain);
            this.Controls.Add(panelSidebar);

            // Title
            Label lblTitle = new Label
            {
                Text = "UMVC CANTEEN",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(0, 20),
                Size = new Size(250, 40),
                TextAlign = ContentAlignment.MiddleCenter
            };
            panelSidebar.Controls.Add(lblTitle);

            // Sidebar Buttons
            btnProducts = CreateSidebarButton("Manage Products", 100);
            btnProducts.Click += (s, e) => OpenProductManagement();

            btnInventory = CreateSidebarButton("Inventory Stock", 160);
            btnInventory.Click += (s, e) => OpenInventoryManagement();

            btnSuppliers = CreateSidebarButton("Manage Suppliers", 220);
            btnSuppliers.Click += (s, e) => OpenSupplierManagement();

            btnUsers = CreateSidebarButton("Manage Users", 280);
            btnUsers.Click += (s, e) => OpenUserManagement();

            btnReports = CreateSidebarButton("Sales Reports", 340);
            btnReports.Click += (s, e) => OpenReports();

            btnPOS = CreateSidebarButton("Open POS (Cashier)", 400);
            btnPOS.Click += (s, e) => OpenPOS();

            btnLogout = CreateSidebarButton("Logout", 550);
            btnLogout.BackColor = Color.FromArgb(192, 57, 43);
            btnLogout.Click += (s, e) => Logout();
        }

        private Button CreateSidebarButton(string text, int y)
        {
            Button btn = new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(250, 50),
                Location = new Point(0, y),
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0)
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.MouseEnter += (s, e) => { if (btn != btnLogout) btn.BackColor = Color.FromArgb(41, 128, 185); };
            btn.MouseLeave += (s, e) => { if (btn != btnLogout) btn.BackColor = Color.FromArgb(52, 73, 94); };
            panelSidebar.Controls.Add(btn);
            return btn;
        }

        // ==========================================
        // 🚀 FUNCTIONAL DASHBOARD LOGIC
        // ==========================================
        private void ShowDashboardHome()
        {
            panelMain.Controls.Clear();

            // Header Labels
            Label lblDashboard = new Label { Text = "Dashboard Overview", Font = new Font("Segoe UI", 20, FontStyle.Bold), Location = new Point(20, 20), AutoSize = true };
            panelMain.Controls.Add(lblDashboard);

            Label lblWelcome = new Label { Text = $"Welcome back, {currentUserName}! Here is today's summary.", Font = new Font("Segoe UI", 12), Location = new Point(25, 60), AutoSize = true, ForeColor = Color.Gray };
            panelMain.Controls.Add(lblWelcome);

            // Refresh Button (To manually update the numbers)
            Button btnRefresh = new Button { Text = "Refresh Data", Location = new Point(1080, 30), Width = 120, Height = 35, BackColor = Color.FromArgb(41, 128, 185), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            btnRefresh.Click += (s, e) => ShowDashboardHome();
            panelMain.Controls.Add(btnRefresh);

            // --- DATABASE QUERIES ---
            string salesValue = "₱0.00";
            string transValue = "0";
            string lowStockValue = "0";
            string prodValue = "0";

            try
            {
                // 1. TODAY'S SALES
                // Sums 'TotalAmount' where the TransactionDate is Today
                string querySales = "SELECT SUM(TotalAmount) FROM Transactions WHERE DATE(TransactionDate) = CURDATE() AND Status = 'Completed'";
                DataTable dtSales = db.ExecuteQuery(querySales);
                if (dtSales.Rows.Count > 0 && dtSales.Rows[0][0] != DBNull.Value)
                {
                    decimal total = Convert.ToDecimal(dtSales.Rows[0][0]);
                    salesValue = "₱" + total.ToString("N2");
                }

                // 2. TOTAL TRANSACTIONS (TODAY)
                // Counts how many transactions happened today
                string queryTrans = "SELECT COUNT(*) FROM Transactions WHERE DATE(TransactionDate) = CURDATE() AND Status = 'Completed'";
                DataTable dtTrans = db.ExecuteQuery(queryTrans);
                if (dtTrans.Rows.Count > 0 && dtTrans.Rows[0][0] != DBNull.Value)
                {
                    transValue = dtTrans.Rows[0][0].ToString();
                }

                // 3. LOW STOCK ITEMS
                // Counts products where StockQuantity is less than CriticalStockLevel
                string queryLow = "SELECT COUNT(*) FROM Products WHERE StockQuantity <= CriticalStockLevel AND IsActive = 1";
                DataTable dtLow = db.ExecuteQuery(queryLow);
                if (dtLow.Rows.Count > 0 && dtLow.Rows[0][0] != DBNull.Value)
                {
                    lowStockValue = dtLow.Rows[0][0].ToString();
                }

                // 4. TOTAL PRODUCTS
                string queryProd = "SELECT COUNT(*) FROM Products WHERE IsActive = 1";
                DataTable dtProd = db.ExecuteQuery(queryProd);
                if (dtProd.Rows.Count > 0 && dtProd.Rows[0][0] != DBNull.Value)
                {
                    prodValue = dtProd.Rows[0][0].ToString();
                }
            }
            catch (Exception ex)
            {
                // Just in case the database connection fails
                Console.WriteLine("Dashboard Error: " + ex.Message);
            }

            // --- CREATE CARDS WITH REAL DATA ---
            CreateMetricCard("Today's Sales", salesValue, 20, 100, Color.FromArgb(46, 204, 113));
            CreateMetricCard("Transactions Today", transValue, 320, 100, Color.FromArgb(52, 152, 219));
            CreateMetricCard("Low Stock Items", lowStockValue, 620, 100, Color.FromArgb(231, 76, 60));
            CreateMetricCard("Total Products", prodValue, 920, 100, Color.FromArgb(155, 89, 182));
        }

        private void CreateMetricCard(string title, string value, int x, int y, Color color)
        {
            Panel card = new Panel { Size = new Size(280, 150), Location = new Point(x, y), BackColor = color };

            // Add a slight visual improvement (bottom border)
            Panel strip = new Panel { Dock = DockStyle.Bottom, Height = 10, BackColor = ControlPaint.Dark(color) };
            card.Controls.Add(strip);

            Label lblTitle = new Label { Text = title, Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = Color.WhiteSmoke, Location = new Point(15, 15), AutoSize = true };
            card.Controls.Add(lblTitle);

            Label lblValue = new Label { Text = value, Font = new Font("Segoe UI", 28, FontStyle.Bold), ForeColor = Color.White, Location = new Point(15, 50), AutoSize = true };
            card.Controls.Add(lblValue);

            panelMain.Controls.Add(card);
        }

        // Navigation Methods - Added "ShowDashboardHome()" after closing dialogs so numbers update immediately
        private void OpenProductManagement() { var f = new ProductManagementForm(); f.ShowDialog(); ShowDashboardHome(); }
        private void OpenInventoryManagement() { var f = new InventoryManagementForm(currentUserId); f.ShowDialog(); ShowDashboardHome(); }
        private void OpenSupplierManagement() { var f = new SupplierManagementForm(); f.ShowDialog(); }
        private void OpenUserManagement() { var f = new UserManagementForm(); f.ShowDialog(); }
        private void OpenReports() { var f = new ReportsForm(); f.ShowDialog(); }
        private void OpenPOS() { var f = new CashierDashboard(currentUserId, currentUserName); f.ShowDialog(); ShowDashboardHome(); } // Updates sales after POS closes

        private void Logout()
        {
            if (MessageBox.Show("Are you sure you want to logout?", "Confirm Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Hide();
                this.Close();
            }
        }

        private void AdminDashboard_Load(object sender, EventArgs e) { }
    }
}