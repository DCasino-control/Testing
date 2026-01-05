using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Testing.Models;
using Testing.Models.models;

namespace Testing
{
    public class AdminDashboard : Form
    {
        private Admin admin;

        public AdminDashboard(Admin admin)
        {
            this.admin = admin;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = $"Admin Dashboard - {admin.FullName}";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 240, 240);

            // Header
            Panel headerPanel = new Panel
            {
                Size = new Size(1000, 80),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(33, 150, 243)
            };
            this.Controls.Add(headerPanel);

            Label titleLabel = new Label
            {
                Text = $"👑 Welcome, {admin.FullName}!",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                Size = new Size(900, 40),
                Location = new Point(20, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };
            headerPanel.Controls.Add(titleLabel);

            // Quick action buttons
            int btnX = 50;
            int btnY = 120;

            CreateDashboardButton("👥 Manage Users", btnX, btnY, (s, e) =>
            {
                MessageBox.Show("User Management - Coming soon!");
            });

            CreateDashboardButton("📦 Manage Products", btnX + 220, btnY, (s, e) =>
            {
                MessageBox.Show("Product Management - Coming soon!");
            });

            CreateDashboardButton("💰 POS System", btnX + 440, btnY, (s, e) =>
            {
                // Open POS
                POSForm pos = new POSForm(admin);
                pos.ShowDialog();
            });

            CreateDashboardButton("📊 Reports", btnX, btnY + 150, (s, e) =>
            {
                MessageBox.Show("Reports - Coming soon!");
            });

            CreateDashboardButton("🏭 Suppliers", btnX + 220, btnY + 150, (s, e) =>
            {
                MessageBox.Show("Supplier Management - Coming soon!");
            });

            CreateDashboardButton("⚙️ Settings", btnX + 440, btnY + 150, (s, e) =>
            {
                MessageBox.Show("Settings - Coming soon!");
            });

            // Logout button
            Button logoutBtn = new Button
            {
                Text = "Logout",
                Size = new Size(120, 40),
                Location = new Point(850, 620),
                BackColor = Color.FromArgb(244, 67, 54),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            logoutBtn.FlatAppearance.BorderSize = 0;
            logoutBtn.Click += (s, e) => this.Close();
            this.Controls.Add(logoutBtn);
        }

        private void CreateDashboardButton(string text, int x, int y, EventHandler onClick)
        {
            Button btn = new Button
            {
                Text = text,
                Size = new Size(200, 120),
                Location = new Point(x, y),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(33, 33, 33),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            btn.FlatAppearance.BorderColor = Color.LightGray;
            btn.Click += onClick;
            this.Controls.Add(btn);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // AdminDashboard
            // 
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "AdminDashboard";
            this.Load += new System.EventHandler(this.AdminDashboard_Load);
            this.ResumeLayout(false);

        }

        private void AdminDashboard_Load(object sender, EventArgs e)
        {

        }
    }

    public class CashierDashboard : Form
    {
        private Cashier cashier;

        public CashierDashboard(Cashier cashier)
        {
            this.cashier = cashier;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = $"Cashier Dashboard - {cashier.FullName}";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 240, 240);

            // Header
            Panel headerPanel = new Panel
            {
                Size = new Size(1000, 80),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(76, 175, 80)
            };
            this.Controls.Add(headerPanel);

            Label titleLabel = new Label
            {
                Text = $"💰 Welcome, {cashier.FullName}!",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                Size = new Size(900, 40),
                Location = new Point(20, 20)
            };
            headerPanel.Controls.Add(titleLabel);

            // Big POS button
            Button posBtn = new Button
            {
                Text = "🛒 START NEW SALE",
                Size = new Size(400, 200),
                Location = new Point(300, 200),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            posBtn.FlatAppearance.BorderSize = 0;
            posBtn.Click += (s, e) =>
            {
                POSForm pos = new POSForm(cashier);
                pos.ShowDialog();
            };
            this.Controls.Add(posBtn);

            // Sales summary
            Label salesLabel = new Label
            {
                Text = $"Today's Sales: ₱{cashier.TodaysSales:N2}\nTransactions: {cashier.TransactionCount}",
                Font = new Font("Segoe UI", 14),
                Size = new Size(400, 80),
                Location = new Point(300, 430),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(salesLabel);

            // Logout
            Button logoutBtn = new Button
            {
                Text = "Logout",
                Size = new Size(120, 40),
                Location = new Point(850, 620),
                BackColor = Color.FromArgb(244, 67, 54),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            logoutBtn.FlatAppearance.BorderSize = 0;
            logoutBtn.Click += (s, e) => this.Close();
            this.Controls.Add(logoutBtn);
        }
    }

    public class InventoryDashboard : Form
    {
        private InventoryClerk clerk;

        public InventoryDashboard(InventoryClerk clerk)
        {
            this.clerk = clerk;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = $"Inventory Dashboard - {clerk.FullName}";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            Label label = new Label
            {
                Text = $"📦 Inventory Clerk Dashboard\n\nWelcome, {clerk.FullName}!",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Size = new Size(600, 100),
                Location = new Point(200, 200),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(label);
        }
    }

    public class POSForm : Form
    {
        private User currentUser;

        public POSForm(User user)
        {
            this.currentUser = user;
            this.Text = "Point of Sale";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;

            Label label = new Label
            {
                Text = "🛒 POS System\n\n(Full POS interface will be implemented here)",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                Size = new Size(800, 200),
                Location = new Point(200, 250),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(label);
        }
    }
}


