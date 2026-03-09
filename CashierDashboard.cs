using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Testing.Database;
using Testing.Models.models;

namespace Testing.Forms
{
    public partial class CashierDashboard : Form
    {
        private int currentUserId;
        private string currentUserName;
        private DatabaseConnection db;
        private List<CartItem> cartItems;
        private decimal cartTotal;

        // UI Components (The Tools)
        private Panel panelTop, panelLeft, panelRight, panelTotals;
        private Label lblTotalAmount;
        private TextBox txtSearch;
        private FlowLayoutPanel flowProducts;
        private DataGridView dgvCart;
        private ComboBox cboCategory;
        private Button btnCheckout;

        public CashierDashboard(int userId, string userName)
        {
            currentUserId = userId;
            currentUserName = userName;
            db = DatabaseConnection.Instance;
            cartItems = new List<CartItem>();
            cartTotal = 0;

            // 1. This builds the window and creates all the "Tools" (Buttons, Grids, etc.)
            SetupCashierUI();

            // 2. These configure the tools (Now safe to run because tools exist!)
            SetupCartGrid();
            LoadCategories();
            LoadProducts();
        }

        // --- THIS METHOD CREATES ALL THE UI ELEMENTS MANUALLY ---
        private void SetupCashierUI()
        {
            // Form Settings
            this.Text = "Cashier Dashboard";
            this.Size = new Size(1280, 720);
            this.WindowState = FormWindowState.Maximized;

            // --- 1. TOP PANEL ---
            panelTop = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(52, 73, 94) };
            Label lblTitle = new Label { Text = "UMVC POS - " + currentUserName, ForeColor = Color.White, Font = new Font("Segoe UI", 14, FontStyle.Bold), Location = new Point(20, 15), AutoSize = true };
            Button btnLogout = new Button { Text = "Logout", BackColor = Color.IndianRed, ForeColor = Color.White, Size = new Size(80, 30), Location = new Point(1150, 15) };
            btnLogout.Click += (s, e) => { if (MessageBox.Show("Logout?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes) this.Close(); };

            panelTop.Controls.Add(lblTitle);
            panelTop.Controls.Add(btnLogout);
            this.Controls.Add(panelTop);

            // --- 2. RIGHT PANEL (CART) ---
            panelRight = new Panel { Dock = DockStyle.Right, Width = 400, BackColor = Color.WhiteSmoke, Padding = new Padding(10) };

            // Create the DataGridView (The Box for the Cart)
            dgvCart = new DataGridView();
            dgvCart.Dock = DockStyle.Top;
            dgvCart.Height = 400;
            dgvCart.BackgroundColor = Color.White;
            dgvCart.AllowUserToAddRows = false;
            dgvCart.ReadOnly = true;
            dgvCart.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCart.RowHeadersVisible = false;

            // Totals Section
            panelTotals = new Panel { Dock = DockStyle.Top, Height = 100, BackColor = Color.White };
            lblTotalAmount = new Label { Text = "₱0.00", Font = new Font("Segoe UI", 24, FontStyle.Bold), ForeColor = Color.Green, Location = new Point(10, 30), AutoSize = true };
            panelTotals.Controls.Add(new Label { Text = "Total:", Location = new Point(10, 10) });
            panelTotals.Controls.Add(lblTotalAmount);

            // Buttons
            Button btnClear = new Button { Text = "Clear Cart", BackColor = Color.Orange, Dock = DockStyle.Top, Height = 40 };
            btnClear.Click += (s, e) => { cartItems.Clear(); RefreshCart(); };

            btnCheckout = new Button { Text = "CHECKOUT", BackColor = Color.Green, ForeColor = Color.White, Dock = DockStyle.Bottom, Height = 60, Font = new Font("Segoe UI", 14, FontStyle.Bold) };
            btnCheckout.Click += BtnCheckout_Click;

            // Add everything to the Right Panel
            panelRight.Controls.Add(btnCheckout);
            panelRight.Controls.Add(btnClear);
            panelRight.Controls.Add(panelTotals);
            panelRight.Controls.Add(dgvCart);
            this.Controls.Add(panelRight);

            // --- 3. LEFT PANEL (PRODUCTS) ---
            panelLeft = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            Panel searchPanel = new Panel { Dock = DockStyle.Top, Height = 50 };

            // Create Search Box and Category Combo
            txtSearch = new TextBox { Width = 300, Font = new Font("Segoe UI", 12), Location = new Point(0, 10) };
            txtSearch.TextChanged += (s, e) => LoadProducts(txtSearch.Text);

            cboCategory = new ComboBox { Width = 200, Location = new Point(310, 10), DropDownStyle = ComboBoxStyle.DropDownList };
            cboCategory.SelectedIndexChanged += (s, e) => LoadProducts(txtSearch.Text);

            searchPanel.Controls.Add(txtSearch);
            searchPanel.Controls.Add(cboCategory);

            // Create the Product Grid
            flowProducts = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, BackColor = Color.White };

            panelLeft.Controls.Add(flowProducts);
            panelLeft.Controls.Add(searchPanel);
            this.Controls.Add(panelLeft);
        }

        // --- SETUP GRID COLUMNS ---
        private void SetupCartGrid()
        {
            dgvCart.Columns.Clear();
            dgvCart.Columns.Add("Name", "Item");
            dgvCart.Columns.Add("Qty", "Qty");
            dgvCart.Columns.Add("Price", "Price");
            dgvCart.Columns.Add("Sub", "Total");
            dgvCart.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        // --- LOAD CATEGORIES ---
        private void LoadCategories()
        {
            try
            {
                DataTable dt = db.ExecuteQuery("SELECT CategoryId, CategoryName FROM ProductCategories WHERE IsActive = 1");
                cboCategory.Items.Clear();
                cboCategory.Items.Add(new CategoryItem { CategoryId = 0, CategoryName = "All" });
                foreach (DataRow r in dt.Rows)
                    cboCategory.Items.Add(new CategoryItem { CategoryId = (int)r["CategoryId"], CategoryName = r["CategoryName"].ToString() });

                cboCategory.DisplayMember = "CategoryName";
                cboCategory.SelectedIndex = 0;
            }
            catch { }
        }

        // --- LOAD PRODUCTS ---
        private void LoadProducts(string search = "")
        {
            try
            {
                flowProducts.Controls.Clear();
                string query = "SELECT * FROM Products WHERE IsActive = 1 AND StockQuantity > 0";
                if (!string.IsNullOrEmpty(search)) query += $" AND ProductName LIKE '%{search}%'";

                DataTable dt = db.ExecuteQuery(query);

                foreach (DataRow row in dt.Rows)
                {
                    Button prodBtn = new Button();
                    prodBtn.Size = new Size(150, 100);
                    prodBtn.Text = $"{row["ProductName"]}\n₱{row["UnitPrice"]}\n(Stock: {row["StockQuantity"]})";
                    prodBtn.BackColor = Color.LightBlue;
                    prodBtn.Margin = new Padding(5);
                    prodBtn.Tag = row;
                    prodBtn.Click += (s, e) => AddToCart(row);
                    flowProducts.Controls.Add(prodBtn);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // CashierDashboard
            // 
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "CashierDashboard";
            this.Load += new System.EventHandler(this.CashierDashboard_Load);
            this.ResumeLayout(false);

        }

        private void CashierDashboard_Load(object sender, EventArgs e)
        {

        }

        // --- CART LOGIC ---
        private void AddToCart(DataRow row)
        {
            int id = (int)row["ProductId"];
            var existing = cartItems.FirstOrDefault(i => i.ProductId == id);

            // Check stock logic
            int currentStock = Convert.ToInt32(row["StockQuantity"]);
            int currentInCart = existing != null ? existing.Quantity : 0;

            if (currentInCart + 1 > currentStock)
            {
                MessageBox.Show("Not enough stock!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (existing != null) existing.Quantity++;
            else cartItems.Add(new CartItem
            {
                ProductId = id,
                ProductName = row["ProductName"].ToString(),
                Quantity = 1,
                UnitPrice = Convert.ToDecimal(row["UnitPrice"]),
                Subtotal = Convert.ToDecimal(row["UnitPrice"])
            });
            RefreshCart();
        }

        private void RefreshCart()
        {
            dgvCart.Rows.Clear();
            cartTotal = 0;
            foreach (var item in cartItems)
            {
                item.Subtotal = item.Quantity * item.UnitPrice;
                dgvCart.Rows.Add(item.ProductName, item.Quantity, item.UnitPrice, item.Subtotal);
                cartTotal += item.Subtotal;
            }
            lblTotalAmount.Text = "₱" + cartTotal.ToString("N2");
        }

        // --- CHECKOUT LOGIC ---
        private void BtnCheckout_Click(object sender, EventArgs e)
        {
            if (cartItems.Count == 0)
            {
                MessageBox.Show("Cart is empty!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var checkoutForm = new CheckoutForm(cartItems, cartTotal, currentUserId))
            {
                if (checkoutForm.ShowDialog() == DialogResult.OK)
                {
                    cartItems.Clear();
                    RefreshCart();
                    LoadProducts(txtSearch.Text);
                }
            }
        }
    }
}