using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Testing.Database;

namespace Testing.Forms
{
    public partial class SupplierManagementForm : Form
    {
        private DatabaseConnection db = DatabaseConnection.Instance;
        private DataGridView dgvSuppliers;
        private Button btnAdd;

        public SupplierManagementForm()
        {
            InitializeComponent();
            SetupModernUI();
            LoadSuppliers();
        }

        private void SetupModernUI()
        {
            // 1. Form Settings
            this.Size = new Size(1200, 700);
            this.Text = "Supplier Management";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;

            // 2. Header Panel
            Panel panelHeader = new Panel();
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Height = 60;
            panelHeader.BackColor = Color.FromArgb(41, 128, 185);

            Label lblTitle = new Label();
            lblTitle.Text = "SUPPLIER MANAGEMENT";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(20, 15);
            lblTitle.AutoSize = true;
            panelHeader.Controls.Add(lblTitle);
            this.Controls.Add(panelHeader);

            // 3. Bottom Panel (For the Button)
            Panel panelBottom = new Panel();
            panelBottom.Dock = DockStyle.Bottom;
            panelBottom.Height = 80;
            panelBottom.BackColor = Color.White;
            this.Controls.Add(panelBottom);

            // ADD SUPPLIER BUTTON (Bottom Right)
            btnAdd = new Button();
            btnAdd.Text = "+ ADD SUPPLIER";
            btnAdd.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnAdd.BackColor = Color.FromArgb(46, 204, 113); // Green
            btnAdd.ForeColor = Color.White;
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Size = new Size(180, 45);
            // Anchor to Bottom Right
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

            dgvSuppliers = new DataGridView();
            dgvSuppliers.Dock = DockStyle.Fill;
            dgvSuppliers.BackgroundColor = Color.White;
            dgvSuppliers.BorderStyle = BorderStyle.None;
            dgvSuppliers.ReadOnly = true;
            dgvSuppliers.RowHeadersVisible = false;
            dgvSuppliers.AllowUserToAddRows = false;
            dgvSuppliers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSuppliers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Grid Styling
            dgvSuppliers.ColumnHeadersHeight = 45;
            dgvSuppliers.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvSuppliers.EnableHeadersVisualStyles = false;
            dgvSuppliers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvSuppliers.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvSuppliers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            dgvSuppliers.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvSuppliers.DefaultCellStyle.SelectionBackColor = Color.FromArgb(41, 128, 185);
            dgvSuppliers.DefaultCellStyle.Padding = new Padding(5);
            dgvSuppliers.RowTemplate.Height = 35;

            // Explicitly Indicate Columns
            dgvSuppliers.Columns.Add("SupplierId", "ID");
            dgvSuppliers.Columns.Add("SupplierName", "Supplier Name");
            dgvSuppliers.Columns.Add("ContactPerson", "Contact Person");
            dgvSuppliers.Columns.Add("Phone", "Phone Number");
            dgvSuppliers.Columns.Add("Email", "Email Address");
            dgvSuppliers.Columns.Add("Address", "Address");

            // Adjust ID width
            dgvSuppliers.Columns["SupplierId"].FillWeight = 30;

            panelGrid.Controls.Add(dgvSuppliers);
        }

        private void LoadSuppliers()
        {
            try
            {
                dgvSuppliers.Rows.Clear();
                string query = "SELECT SupplierId, SupplierName, ContactPerson, Phone, Email, Address FROM Suppliers WHERE IsActive = 1";
                DataTable dt = db.ExecuteQuery(query);

                foreach (DataRow row in dt.Rows)
                {
                    dgvSuppliers.Rows.Add(
                        row["SupplierId"],
                        row["SupplierName"],
                        row["ContactPerson"],
                        row["Phone"],
                        row["Email"],
                        row["Address"]
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading suppliers: " + ex.Message);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            // Create a Popup Dialog Form
            Form addForm = new Form();
            addForm.Text = "Add New Supplier";
            addForm.Size = new Size(400, 450);
            addForm.StartPosition = FormStartPosition.CenterParent;
            addForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            addForm.MaximizeBox = false;
            addForm.MinimizeBox = false;
            addForm.BackColor = Color.White;

            int y = 20;
            TextBox txtName = new TextBox();
            TextBox txtContact = new TextBox();
            TextBox txtPhone = new TextBox();
            TextBox txtEmail = new TextBox();
            TextBox txtAddress = new TextBox();

            AddDialogInput(addForm, "Supplier Name:", txtName, ref y);
            AddDialogInput(addForm, "Contact Person:", txtContact, ref y);
            AddDialogInput(addForm, "Phone Number:", txtPhone, ref y);
            AddDialogInput(addForm, "Email Address:", txtEmail, ref y);
            AddDialogInput(addForm, "Address:", txtAddress, ref y);

            y += 20;
            Button btnSave = new Button();
            btnSave.Text = "SAVE SUPPLIER";
            btnSave.Location = new Point(40, y);
            btnSave.Size = new Size(300, 40);
            btnSave.BackColor = Color.FromArgb(46, 204, 113);
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.DialogResult = DialogResult.OK; // Important for closing
            addForm.Controls.Add(btnSave);

            // Show the Dialog
            if (addForm.ShowDialog() == DialogResult.OK)
            {
                // Validation
                if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtPhone.Text))
                {
                    MessageBox.Show("Supplier Name and Phone are required!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Database Insert
                try
                {
                    string query = @"INSERT INTO Suppliers (SupplierName, ContactPerson, Phone, Email, Address, IsActive, CreatedDate) 
                                   VALUES (@name, @contact, @phone, @email, @addr, 1, NOW())";

                    MySqlParameter[] parameters = {
                        new MySqlParameter("@name", txtName.Text.Trim()),
                        new MySqlParameter("@contact", txtContact.Text.Trim()),
                        new MySqlParameter("@phone", txtPhone.Text.Trim()),
                        new MySqlParameter("@email", txtEmail.Text.Trim()),
                        new MySqlParameter("@addr", txtAddress.Text.Trim())
                    };

                    db.ExecuteNonQuery(query, parameters);
                    MessageBox.Show("Supplier Added Successfully!");
                    LoadSuppliers(); // Refresh Main Grid
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
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
    }
}