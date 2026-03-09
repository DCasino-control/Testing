using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Testing.Database;

namespace Testing.Forms
{
    public partial class ProductManagementForm : Form
    {
        private DatabaseConnection db = DatabaseConnection.Instance;

        // UI Controls
        private DataGridView dgvProducts;
        private Button btnAdd, btnDelete;

        public ProductManagementForm()
        {
            InitializeComponent();
            SetupModernUI();
            LoadProducts();
        }

        private void SetupModernUI()
        {
            // 1. Form Settings
            this.Size = new Size(1200, 700);
            this.Text = "Manage Products";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;

            // 2. Header Panel
            Panel panelHeader = new Panel();
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Height = 60;
            panelHeader.BackColor = Color.FromArgb(41, 128, 185);

            Label lblTitle = new Label();
            lblTitle.Text = "PRODUCT MANAGEMENT";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(20, 15);
            lblTitle.AutoSize = true;
            panelHeader.Controls.Add(lblTitle);
            this.Controls.Add(panelHeader);

            // 3. Bottom Panel (Footer)
            Panel panelBottom = new Panel();
            panelBottom.Dock = DockStyle.Bottom;
            panelBottom.Height = 80;
            panelBottom.BackColor = Color.White;
            this.Controls.Add(panelBottom);

            // --- DELETE BUTTON (BOTTOM LEFT - Encircled Space) ---
            btnDelete = new Button();
            btnDelete.Text = "DELETE PRODUCT";
            btnDelete.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnDelete.BackColor = Color.FromArgb(231, 76, 60); // Red
            btnDelete.ForeColor = Color.White;
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Size = new Size(160, 45);
            // Position: Left side of footer
            btnDelete.Location = new Point(20, 15);
            btnDelete.Cursor = Cursors.Hand;
            btnDelete.Click += BtnDelete_Click;
            panelBottom.Controls.Add(btnDelete); // Added to Bottom Panel

            // --- ADD BUTTON (BOTTOM RIGHT) ---
            btnAdd = new Button();
            btnAdd.Text = "+ ADD PRODUCT";
            btnAdd.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnAdd.BackColor = Color.FromArgb(46, 204, 113); // Green
            btnAdd.ForeColor = Color.White;
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Size = new Size(180, 45);
            // Position: Right side of footer
            btnAdd.Location = new Point(panelBottom.Width - 210, 15);
            btnAdd.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAdd.Cursor = Cursors.Hand;
            btnAdd.Click += BtnAdd_Click;
            panelBottom.Controls.Add(btnAdd);

            // 4. Grid Setup
            Panel panelGrid = new Panel();
            panelGrid.Dock = DockStyle.Fill;
            panelGrid.Padding = new Padding(20);
            panelGrid.BackColor = Color.WhiteSmoke;
            this.Controls.Add(panelGrid);
            panelHeader.SendToBack();
            panelBottom.SendToBack();

            dgvProducts = new DataGridView();
            dgvProducts.Dock = DockStyle.Fill;
            dgvProducts.BackgroundColor = Color.White;
            dgvProducts.BorderStyle = BorderStyle.None;
            dgvProducts.ReadOnly = true;
            dgvProducts.RowHeadersVisible = false;
            dgvProducts.AllowUserToAddRows = false;
            dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvProducts.ColumnHeadersHeight = 45;
            dgvProducts.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvProducts.EnableHeadersVisualStyles = false;
            dgvProducts.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvProducts.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvProducts.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            dgvProducts.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvProducts.DefaultCellStyle.SelectionBackColor = Color.FromArgb(41, 128, 185);
            dgvProducts.DefaultCellStyle.Padding = new Padding(5);
            dgvProducts.RowTemplate.Height = 35;

            // Define Columns (Added 'Name' to fix errors)
            dgvProducts.AutoGenerateColumns = false;
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { Name = "ProductId", DataPropertyName = "ProductId", HeaderText = "ID", Visible = false });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { Name = "ProductCode", DataPropertyName = "ProductCode", HeaderText = "Code", FillWeight = 80 });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { Name = "ProductName", DataPropertyName = "ProductName", HeaderText = "Product Name", FillWeight = 150 });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { Name = "Description", DataPropertyName = "Description", HeaderText = "Description", FillWeight = 200 });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { Name = "CategoryName", DataPropertyName = "CategoryName", HeaderText = "Category", FillWeight = 100 });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { Name = "UnitPrice", DataPropertyName = "UnitPrice", HeaderText = "Price", FillWeight = 70 });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { Name = "StockQuantity", DataPropertyName = "StockQuantity", HeaderText = "Stock", FillWeight = 60 });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { Name = "IsActive", DataPropertyName = "IsActive", HeaderText = "Active", Visible = false });

            panelGrid.Controls.Add(dgvProducts);
        }

        private void LoadProducts()
        {
            try
            {
                string query = @"SELECT p.ProductId, p.ProductCode, p.ProductName, p.Description, 
                               c.CategoryName, p.UnitPrice, p.StockQuantity, p.IsActive 
                               FROM Products p
                               LEFT JOIN ProductCategories c ON p.CategoryId = c.CategoryId
                               WHERE p.IsActive = 1";

                dgvProducts.DataSource = db.ExecuteQuery(query);
            }
            catch (Exception ex) { MessageBox.Show("Error loading products: " + ex.Message); }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            Form addForm = new Form();
            addForm.Text = "Add New Product";
            addForm.Size = new Size(400, 550);
            addForm.StartPosition = FormStartPosition.CenterParent;
            addForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            addForm.MaximizeBox = false;
            addForm.MinimizeBox = false;
            addForm.BackColor = Color.White;

            int y = 20;
            TextBox txtCode = new TextBox();
            TextBox txtName = new TextBox();
            TextBox txtDesc = new TextBox();
            TextBox txtPrice = new TextBox();
            TextBox txtStock = new TextBox();
            ComboBox cboCat = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };

            try
            {
                DataTable dtCat = db.ExecuteQuery("SELECT CategoryId, CategoryName FROM ProductCategories");
                cboCat.DataSource = dtCat;
                cboCat.DisplayMember = "CategoryName";
                cboCat.ValueMember = "CategoryId";
            }
            catch { }

            AddDialogInput(addForm, "Product Code:", txtCode, ref y);
            AddDialogInput(addForm, "Product Name:", txtName, ref y);

            Label lblDesc = new Label { Text = "Description:", Location = new Point(40, y), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            addForm.Controls.Add(lblDesc);
            txtDesc.Location = new Point(40, y + 20);
            txtDesc.Size = new Size(300, 30);
            txtDesc.Font = new Font("Segoe UI", 10);
            addForm.Controls.Add(txtDesc);
            y += 60;

            Label lblCat = new Label { Text = "Category:", Location = new Point(40, y), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            addForm.Controls.Add(lblCat);
            cboCat.Location = new Point(40, y + 20);
            cboCat.Size = new Size(300, 30);
            cboCat.Font = new Font("Segoe UI", 10);
            addForm.Controls.Add(cboCat);
            y += 60;

            AddDialogInput(addForm, "Unit Price:", txtPrice, ref y);
            AddDialogInput(addForm, "Initial Stock:", txtStock, ref y);

            y += 10;
            Button btnSave = new Button();
            btnSave.Text = "SAVE PRODUCT";
            btnSave.Location = new Point(40, y);
            btnSave.Size = new Size(300, 40);
            btnSave.BackColor = Color.FromArgb(46, 204, 113);
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.DialogResult = DialogResult.OK;
            addForm.Controls.Add(btnSave);

            if (addForm.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrWhiteSpace(txtCode.Text) || string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Code and Name are required!");
                    return;
                }

                try
                {
                    string query = @"INSERT INTO Products (ProductCode, ProductName, Description, CategoryId, UnitPrice, StockQuantity, IsActive, CreatedDate) 
                                   VALUES (@code, @name, @desc, @cat, @price, @stock, 1, NOW())";

                    MySqlParameter[] p = {
                        new MySqlParameter("@code", txtCode.Text),
                        new MySqlParameter("@name", txtName.Text),
                        new MySqlParameter("@desc", txtDesc.Text),
                        new MySqlParameter("@cat", cboCat.SelectedValue ?? DBNull.Value),
                        new MySqlParameter("@price", txtPrice.Text),
                        new MySqlParameter("@stock", txtStock.Text)
                    };

                    db.ExecuteNonQuery(query, p);
                    MessageBox.Show("Product Added Successfully!");
                    LoadProducts();
                }
                catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
            }
        }

        private void AddDialogInput(Form f, string label, TextBox txt, ref int y)
        {
            Label lbl = new Label { Text = label, Location = new Point(40, y), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            f.Controls.Add(lbl);
            txt.Location = new Point(40, y + 20);
            txt.Size = new Size(300, 30);
            txt.Font = new Font("Segoe UI", 10);
            f.Controls.Add(txt);
            y += 60;
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            // 1. Validation
            if (dgvProducts.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a product to delete.", "Delete Product", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Get Data
            string prodName = dgvProducts.SelectedRows[0].Cells["ProductName"].Value.ToString();
            string prodId = dgvProducts.SelectedRows[0].Cells["ProductId"].Value.ToString();

            // 3. Confirmation Dialog (Yes/No)
            DialogResult result = MessageBox.Show(
                $"Are you sure you want to DELETE '{prodName}'?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Stop);

            // 4. Action
            if (result == DialogResult.Yes)
            {
                try
                {
                    string query = "UPDATE Products SET IsActive = 0 WHERE ProductId = @id";
                    MySqlParameter[] p = { new MySqlParameter("@id", prodId) };
                    db.ExecuteNonQuery(query, p);

                    MessageBox.Show("Product Deleted Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadProducts();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting product: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}