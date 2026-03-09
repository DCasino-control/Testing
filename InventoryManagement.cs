using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Testing.Database;

namespace Testing.Forms
{
    public partial class InventoryManagementForm : Form
    {
        private int currentUserId;
        private DatabaseConnection db;

        // UI Components
        private DataGridView dgvProducts;
        private TextBox txtSearch;
        private ComboBox cboFilter;
        private Button btnStockIn, btnStockOut, btnRefresh;
        private Panel panelControls;

        public InventoryManagementForm(int userId)
        {
            currentUserId = userId;
            db = DatabaseConnection.Instance;

            SetupModernUI();
            LoadProducts();
        }

        // --- HELPER FUNCTION FOR FILTERING ---
        // This function builds the SQL condition based on the dropdown selection
        private string GetStockFilterCondition()
        {
            // Check if cboFilter is initialized to avoid null errors during startup
            if (cboFilter == null || cboFilter.SelectedItem == null) return "";

            int selectedIndex = cboFilter.SelectedIndex;

            // Index 1 = Low Stock
            if (selectedIndex == 1)
            {
                // Shows items below critical level OR items with 0 stock
                return " AND (p.StockQuantity <= p.CriticalStockLevel OR p.StockQuantity = 0)";
            }
            // Index 2 = Out of Stock
            else if (selectedIndex == 2)
            {
                return " AND p.StockQuantity = 0";
            }
            // Index 3 = Active Only
            else if (selectedIndex == 3)
            {
                return " AND p.IsActive = 1";
            }

            // Default (Index 0) = All Products
            return "";
        }

        private void LoadProducts()
        {
            try
            {
                // Ensure grid is initialized
                if (dgvProducts == null) return;

                dgvProducts.Rows.Clear();

                // 1. Base Query
                string query = @"SELECT p.ProductId, p.ProductCode, p.ProductName, p.Description,
                               p.CategoryId, c.CategoryName, p.UnitPrice, p.CostPrice, 
                               p.StockQuantity, p.CriticalStockLevel, p.ImagePath, p.IsActive
                               FROM Products p
                               LEFT JOIN ProductCategories c ON p.CategoryId = c.CategoryId
                               WHERE 1=1";

                List<MySqlParameter> parameters = new List<MySqlParameter>();

                // 2. Search Logic
                if (txtSearch != null && !string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    query += " AND (p.ProductName LIKE @search OR p.ProductCode LIKE @search)";
                    parameters.Add(new MySqlParameter("@search", $"%{txtSearch.Text}%"));
                }

                // 3. Stock Filter Logic (Calls the helper function)
                query += GetStockFilterCondition();

                query += " ORDER BY p.ProductName";

                // 4. Execute
                DataTable dt = db.ExecuteQuery(query, parameters.ToArray());

                // 5. Populate Grid
                foreach (DataRow row in dt.Rows)
                {
                    int stock = row["StockQuantity"] != DBNull.Value ? Convert.ToInt32(row["StockQuantity"]) : 0;
                    int critical = row["CriticalStockLevel"] != DBNull.Value ? Convert.ToInt32(row["CriticalStockLevel"]) : 0;

                    string status = "";
                    if (stock == 0) status = "OUT OF STOCK";
                    else if (stock <= critical) status = "LOW STOCK";
                    else status = "IN STOCK";

                    if (row["IsActive"] != DBNull.Value && Convert.ToInt32(row["IsActive"]) == 0)
                        status = "INACTIVE";

                    string catName = row["CategoryName"] == DBNull.Value ? "Uncategorized" : row["CategoryName"].ToString();
                    string desc = row["Description"] == DBNull.Value ? "" : row["Description"].ToString();

                    dgvProducts.Rows.Add(
                        row["ProductId"],
                        row["ProductCode"],
                        row["ProductName"],
                        desc,
                        row["CategoryId"],
                        catName,
                        row["UnitPrice"],
                        row["CostPrice"],
                        stock,
                        critical,
                        row["ImagePath"],
                        row["IsActive"],
                        status
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }

        private void SetupModernUI()
        {
            this.Controls.Clear();
            this.Text = "Inventory Management";
            this.Size = new Size(1300, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;

            // --- HEADER PANEL ---
            Panel panelTop = new Panel { Dock = DockStyle.Top, Height = 70, BackColor = Color.FromArgb(41, 128, 185) };
            Label lblTitle = new Label { Text = "INVENTORY MANAGEMENT", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.White, Location = new Point(20, 18), AutoSize = true };
            panelTop.Controls.Add(lblTitle);
            this.Controls.Add(panelTop);

            // --- CONTROLS PANEL ---
            panelControls = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = Color.White };

            // Search
            Label lblSearch = new Label { Text = "Search Product Name:", Location = new Point(20, 15), Font = new Font("Segoe UI", 9, FontStyle.Bold), AutoSize = true };
            txtSearch = new TextBox { Location = new Point(20, 40), Size = new Size(250, 30), Font = new Font("Segoe UI", 11) };
            txtSearch.TextChanged += (s, e) => LoadProducts();

            // Filter
            Label lblFilter = new Label { Text = "Filter Status:", Location = new Point(300, 15), Font = new Font("Segoe UI", 9, FontStyle.Bold), AutoSize = true };
            cboFilter = new ComboBox { Location = new Point(300, 40), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10) };
            cboFilter.Items.AddRange(new object[] { "All Products", "Low Stock", "Out of Stock", "Active Only" });
            cboFilter.SelectedIndex = 0;
            cboFilter.SelectedIndexChanged += (s, e) => LoadProducts();

            // Buttons
            btnStockIn = CreateModernButton("STOCK IN (+)", 500, 35, Color.FromArgb(46, 204, 113));
            btnStockIn.Click += BtnStockIn_Click;

            btnStockOut = CreateModernButton("STOCK OUT (-)", 630, 35, Color.FromArgb(243, 156, 18));
            btnStockOut.Click += BtnStockOut_Click;

            btnRefresh = CreateModernButton("REFRESH", 760, 35, Color.FromArgb(52, 152, 219));
            btnRefresh.Click += (s, e) => LoadProducts();

            panelControls.Controls.Add(lblSearch); panelControls.Controls.Add(txtSearch);
            panelControls.Controls.Add(lblFilter); panelControls.Controls.Add(cboFilter);
            this.Controls.Add(panelControls);

            // --- GRID ---
            dgvProducts = new DataGridView();
            dgvProducts.Dock = DockStyle.Fill;
            dgvProducts.BackgroundColor = Color.White;
            dgvProducts.BorderStyle = BorderStyle.None;
            dgvProducts.ReadOnly = true;
            dgvProducts.RowHeadersVisible = false;
            dgvProducts.AllowUserToAddRows = false;
            dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Grid Styling
            dgvProducts.ColumnHeadersHeight = 45;
            dgvProducts.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvProducts.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvProducts.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvProducts.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvProducts.DefaultCellStyle.SelectionBackColor = Color.FromArgb(41, 128, 185);
            dgvProducts.RowTemplate.Height = 35;

            // Add Columns
            dgvProducts.Columns.Add("ProductId", "ID");
            dgvProducts.Columns.Add("ProductCode", "Code");
            dgvProducts.Columns.Add("ProductName", "Product Name");
            dgvProducts.Columns.Add("Description", "Description");
            dgvProducts.Columns.Add("CategoryId", "Cat ID");
            dgvProducts.Columns.Add("CategoryName", "Category");
            dgvProducts.Columns.Add("UnitPrice", "Unit Price");
            dgvProducts.Columns.Add("CostPrice", "Cost Price");
            dgvProducts.Columns.Add("StockQuantity", "Stock");
            dgvProducts.Columns.Add("CriticalStockLevel", "Critical Lvl");
            dgvProducts.Columns.Add("ImagePath", "Image Path");
            dgvProducts.Columns.Add("IsActive", "Active");
            dgvProducts.Columns.Add("Status", "Status");

            dgvProducts.CellFormatting += DgvProducts_CellFormatting;

            this.Controls.Add(dgvProducts);
        }

        private Button CreateModernButton(string text, int x, int y, Color bg)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Location = new Point(x, y);
            btn.Size = new Size(120, 35);
            btn.BackColor = bg;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
            panelControls.Controls.Add(btn);

            return btn;
        }

        private void DgvProducts_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvProducts.Columns[e.ColumnIndex].Name == "Status" && e.Value != null)
            {
                string status = e.Value.ToString();
                if (status == "OUT OF STOCK") { e.CellStyle.ForeColor = Color.Red; e.CellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold); }
                else if (status == "LOW STOCK") { e.CellStyle.ForeColor = Color.OrangeRed; e.CellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold); }
                else if (status == "INACTIVE") { e.CellStyle.ForeColor = Color.Gray; e.CellStyle.Font = new Font("Segoe UI", 10, FontStyle.Italic); }
                else { e.CellStyle.ForeColor = Color.Green; }
            }
        }

        private void BtnStockIn_Click(object sender, EventArgs e)
        {
            UpdateStock("In");
        }

        private void BtnStockOut_Click(object sender, EventArgs e)
        {
            UpdateStock("Out");
        }

        private void UpdateStock(string type)
        {
            if (dgvProducts.SelectedRows.Count == 0)
            {
                MessageBox.Show($"Please select a product from the list to Stock {type}.", "Select Product", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string prodId = dgvProducts.SelectedRows[0].Cells["ProductId"].Value.ToString();
            string prodName = dgvProducts.SelectedRows[0].Cells["ProductName"].Value.ToString();
            string currentStock = dgvProducts.SelectedRows[0].Cells["StockQuantity"].Value.ToString();

            Form infoForm = new Form()
            {
                Size = new Size(350, 250),
                Text = $"Stock {type} - {prodName}",
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            int y = 20;
            Label lblInfo = new Label { Text = $"Current Stock: {currentStock}", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.DimGray };
            infoForm.Controls.Add(lblInfo);
            y += 40;

            string actionText = type == "In" ? "Add" : "Remove";
            Label lblQty = new Label { Left = 20, Top = y, Text = $"Enter Quantity to {actionText}:", AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };

            TextBox txtQty = new TextBox() { Left = 20, Top = y + 25, Width = 280, Font = new Font("Segoe UI", 11) };

            y += 70;
            Button btnConfirm = new Button() { Text = "CONFIRM", Left = 180, Width = 120, Top = y, DialogResult = DialogResult.OK, BackColor = type == "In" ? Color.Green : Color.Orange, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Height = 35 };
            Button btnCancel = new Button() { Text = "Cancel", Left = 40, Width = 120, Top = y, DialogResult = DialogResult.Cancel, Height = 35 };

            infoForm.Controls.Add(lblQty); infoForm.Controls.Add(txtQty);
            infoForm.Controls.Add(btnConfirm); infoForm.Controls.Add(btnCancel);
            infoForm.AcceptButton = btnConfirm;

            if (infoForm.ShowDialog() == DialogResult.OK)
            {
                if (int.TryParse(txtQty.Text, out int qty) && qty > 0)
                {
                    string sql = "";
                    if (type == "In")
                        sql = "UPDATE Products SET StockQuantity = StockQuantity + @qty WHERE ProductId = @id";
                    else
                        sql = "UPDATE Products SET StockQuantity = StockQuantity - @qty WHERE ProductId = @id";

                    MySqlParameter[] p = { new MySqlParameter("@qty", qty), new MySqlParameter("@id", prodId) };
                    db.ExecuteNonQuery(sql, p);

                    MessageBox.Show($"Successfully updated stock for {prodName}!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadProducts();
                }
                else
                {
                    MessageBox.Show("Invalid quantity entered.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}