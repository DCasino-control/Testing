using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Testing.Database;
using Testing.Models.models;

namespace Testing.Forms
{
    public partial class CheckoutForm : Form
    {
        // ==========================================
        // 🎒 CLASS VARIABLES (The Backpack)
        // ==========================================
        private List<CartItem> _cartItems;
        private decimal _totalAmount;
        private int _cashierId;
        private DatabaseConnection db;

        // UI Controls
        private TextBox txtAmountPaid; // The Real Input Box
        private Label lblChangeValue;
        private Button btnComplete;
        private DataGridView dgvItems;

        public CheckoutForm(List<CartItem> items, decimal total, int userId)
        {
            _cartItems = items;
            _totalAmount = total;
            _cashierId = userId;
            db = DatabaseConnection.Instance;

            // 1. Build the UI
            SetupCheckoutUI();

            // 2. Load the cart items
            LoadItems();

            // 3. Focus on Payment Box safely
            this.Load += (s, e) => {
                if (txtAmountPaid != null)
                {
                    this.ActiveControl = txtAmountPaid;
                }
            };
        }

        private void SetupCheckoutUI()
        {
            // --- UI SETUP CODE (Same as before) ---
            this.Controls.Clear();
            this.Text = "Process Payment";
            this.Size = new Size(950, 750);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            TableLayoutPanel mainLayout = new TableLayoutPanel();
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.RowCount = 3;
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 45F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 55F));
            this.Controls.Add(mainLayout);

            // Header
            Panel panelHeader = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(41, 128, 185) };
            Label lblTitle = new Label { Text = "CHECKOUT SUMMARY", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.White, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
            panelHeader.Controls.Add(lblTitle);
            mainLayout.Controls.Add(panelHeader, 0, 0);

            // Grid
            dgvItems = new DataGridView();
            dgvItems.Dock = DockStyle.Fill;
            dgvItems.BackgroundColor = Color.WhiteSmoke;
            dgvItems.BorderStyle = BorderStyle.None;
            dgvItems.ReadOnly = true;
            dgvItems.RowHeadersVisible = false;
            dgvItems.AllowUserToAddRows = false;
            dgvItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvItems.ColumnHeadersHeight = 40;
            dgvItems.Columns.Add("Prod", "Product");
            dgvItems.Columns.Add("Qty", "Qty");
            dgvItems.Columns.Add("Price", "Price");
            dgvItems.Columns.Add("Sub", "Subtotal");
            mainLayout.Controls.Add(dgvItems, 0, 1);

            // Payment Panel
            Panel panelPayment = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(20) };
            mainLayout.Controls.Add(panelPayment, 0, 2);

            // Buttons
            Panel panelActions = new Panel { Dock = DockStyle.Bottom, Height = 80, Padding = new Padding(0, 10, 0, 0) };
            Button btnCancel = new Button { Text = "CANCEL", Dock = DockStyle.Left, Width = 150, BackColor = Color.IndianRed, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold), Cursor = Cursors.Hand };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            btnComplete = new Button { Text = "COMPLETE PAYMENT", Dock = DockStyle.Right, Width = 350, BackColor = Color.FromArgb(39, 174, 96), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 16, FontStyle.Bold), Cursor = Cursors.Hand };
            btnComplete.FlatAppearance.BorderSize = 0;
            btnComplete.Click += BtnComplete_Click; // <--- TRIGGERS DATABASE SAVE

            panelActions.Controls.Add(btnCancel);
            panelActions.Controls.Add(btnComplete);
            panelPayment.Controls.Add(panelActions);

            // Quick Cash
            FlowLayoutPanel panelQuick = new FlowLayoutPanel { Dock = DockStyle.Right, Width = 320, FlowDirection = FlowDirection.LeftToRight };
            int[] bills = { 20, 50, 100, 200, 500, 1000 };
            foreach (int bill in bills)
            {
                Button btnBill = new Button { Text = $"₱{bill}", Width = 95, Height = 50, BackColor = Color.WhiteSmoke, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11, FontStyle.Bold), Margin = new Padding(3) };
                btnBill.Click += (s, e) => { if (txtAmountPaid != null) { txtAmountPaid.Text = bill.ToString(); txtAmountPaid.Focus(); } };
                panelQuick.Controls.Add(btnBill);
            }
            Button btnExact = new Button { Text = "EXACT AMOUNT", Width = 297, Height = 50, BackColor = Color.Goldenrod, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11, FontStyle.Bold), Margin = new Padding(3, 10, 3, 3) };
            btnExact.Click += (s, e) => { if (txtAmountPaid != null) { txtAmountPaid.Text = _totalAmount.ToString(); txtAmountPaid.Focus(); } };
            panelQuick.Controls.Add(btnExact);
            panelPayment.Controls.Add(panelQuick);

            // Inputs
            FlowLayoutPanel panelInputs = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, AutoScroll = true };
            Label lblTotalTitle = new Label { Text = "TOTAL DUE", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.Gray, AutoSize = true, Margin = new Padding(0, 0, 0, 5) };
            Label lblTotalDisp = new Label { Text = $"₱{_totalAmount:N2}", Font = new Font("Segoe UI", 32, FontStyle.Bold), ForeColor = Color.FromArgb(41, 128, 185), AutoSize = true, Margin = new Padding(0, 0, 0, 20) };
            Label lblPayTitle = new Label { Text = "CASH TENDERED", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.Gray, AutoSize = true, Margin = new Padding(0, 0, 0, 5) };

            // The Real Textbox
            txtAmountPaid = new TextBox { Font = new Font("Segoe UI", 24), Width = 280, Margin = new Padding(0, 0, 0, 20) };
            txtAmountPaid.TextChanged += CalculateChange;

            Label lblChangeTitle = new Label { Text = "CHANGE", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.Gray, AutoSize = true, Margin = new Padding(0, 0, 0, 5) };
            lblChangeValue = new Label { Text = "₱0.00", Font = new Font("Segoe UI", 36, FontStyle.Bold), ForeColor = Color.DodgerBlue, AutoSize = true };

            panelInputs.Controls.Add(lblTotalTitle);
            panelInputs.Controls.Add(lblTotalDisp);
            panelInputs.Controls.Add(lblPayTitle);
            panelInputs.Controls.Add(txtAmountPaid);
            panelInputs.Controls.Add(lblChangeTitle);
            panelInputs.Controls.Add(lblChangeValue);
            panelPayment.Controls.Add(panelInputs);
        }

        private void LoadItems()
        {
            if (dgvItems == null) return;
            foreach (var item in _cartItems)
                dgvItems.Rows.Add(item.ProductName, item.Quantity, item.UnitPrice, item.Subtotal);
        }

        private void CalculateChange(object sender, EventArgs e)
        {
            if (lblChangeValue == null) return;
            if (decimal.TryParse(txtAmountPaid.Text, out decimal paid))
            {
                decimal change = paid - _totalAmount;
                lblChangeValue.Text = $"₱{(change < 0 ? 0 : change):N2}";
                lblChangeValue.ForeColor = change >= 0 ? Color.DodgerBlue : Color.IndianRed;
            }
            else
            {
                lblChangeValue.Text = "₱0.00";
            }
        }

        // ==========================================
        //  DATABASE IMPLEMENTATION (Based on your Screenshot)
        // ==========================================
        private void BtnComplete_Click(object sender, EventArgs e)
        {
            // 1. Validate Input
            if (!decimal.TryParse(txtAmountPaid.Text, out decimal amountPaid) || amountPaid < _totalAmount)
            {
                MessageBox.Show("Insufficient Payment! Please check the amount.", "Payment Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal change = amountPaid - _totalAmount;

            // 2. Open Connection
            using (MySqlConnection conn = db.GetConnection())
            {
                try
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();

                    // 3. Start Transaction (Ensures all tables update, or none do)
                    MySqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        // --- A. INSERT INTO TRANSACTIONS ---
                        // Attributes: TransactionDate, CashierId, TotalAmount, AmountPaid, ChangeAmount, Status
                        string queryTrans = @"INSERT INTO Transactions 
                                            (TransactionDate, CashierId, TotalAmount, AmountPaid, ChangeAmount, Status) 
                                            VALUES (NOW(), @cid, @total, @paid, @change, 'Completed');
                                            SELECT LAST_INSERT_ID();";

                        MySqlCommand cmd = new MySqlCommand(queryTrans, conn, transaction);
                        cmd.Parameters.AddWithValue("@cid", _cashierId);
                        cmd.Parameters.AddWithValue("@total", _totalAmount);
                        cmd.Parameters.AddWithValue("@paid", amountPaid);
                        cmd.Parameters.AddWithValue("@change", change);

                        // Get the ID of the new transaction
                        int transId = Convert.ToInt32(cmd.ExecuteScalar());

                        // --- B. LOOP THROUGH CART ITEMS ---
                        foreach (var item in _cartItems)
                        {
                            // 1. Save Item Details
                            string queryItem = @"INSERT INTO TransactionItems 
                                               (TransactionId, ProductId, Quantity, UnitPrice, Subtotal) 
                                               VALUES (@tid, @pid, @qty, @price, @sub)";

                            cmd = new MySqlCommand(queryItem, conn, transaction);
                            cmd.Parameters.AddWithValue("@tid", transId);
                            cmd.Parameters.AddWithValue("@pid", item.ProductId);
                            cmd.Parameters.AddWithValue("@qty", item.Quantity);
                            cmd.Parameters.AddWithValue("@price", item.UnitPrice);
                            cmd.Parameters.AddWithValue("@sub", item.Subtotal);
                            cmd.ExecuteNonQuery();

                            // 2. Update Product Stock (Subtract Quantity)
                            string queryStock = "UPDATE Products SET StockQuantity = StockQuantity - @qty WHERE ProductId = @pid";
                            cmd = new MySqlCommand(queryStock, conn, transaction);
                            cmd.Parameters.AddWithValue("@qty", item.Quantity);
                            cmd.Parameters.AddWithValue("@pid", item.ProductId);
                            cmd.ExecuteNonQuery();
                        }

                        // --- C. COMMIT (SAVE) ---
                        transaction.Commit();

                        MessageBox.Show("Transaction Completed Successfully!\nChange: " + lblChangeValue.Text, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Close form and return "OK" so the Dashboard knows to clear the cart
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        // If anything fails, undo changes
                        transaction.Rollback();
                        MessageBox.Show("Transaction Failed: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Connection Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}