using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Testing.Database;

namespace Testing.Forms
{
    public partial class UserManagementForm : Form
    {
        private DatabaseConnection db = DatabaseConnection.Instance;

        // UI Controls
        private DataGridView dgvUsers;
        private Button btnAdd, btnDelete;

        public UserManagementForm()
        {
            InitializeComponent();
            SetupModernUI();
            LoadUsers();
        }

        private void SetupModernUI()
        {
            // 1. Form Settings
            this.Size = new Size(1200, 700);
            this.Text = "Manage Users";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;

            // 2. Header Panel
            Panel panelHeader = new Panel();
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Height = 60;
            panelHeader.BackColor = Color.FromArgb(41, 128, 185);

            Label lblTitle = new Label();
            lblTitle.Text = "USER MANAGEMENT";
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
            btnDelete.Text = "DELETE USER";
            btnDelete.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnDelete.BackColor = Color.FromArgb(231, 76, 60); // Red
            btnDelete.ForeColor = Color.White;
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Size = new Size(150, 45);
            // Position: Bottom Left
            btnDelete.Location = new Point(20, 15);
            btnDelete.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnDelete.Cursor = Cursors.Hand;
            btnDelete.Click += BtnDelete_Click;
            panelBottom.Controls.Add(btnDelete);

            // --- ADD BUTTON (BOTTOM RIGHT) ---
            btnAdd = new Button();
            btnAdd.Text = "+ ADD USER";
            btnAdd.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnAdd.BackColor = Color.FromArgb(46, 204, 113); // Green
            btnAdd.ForeColor = Color.White;
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Size = new Size(180, 45);
            // Position: Bottom Right
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

            dgvUsers = new DataGridView();
            dgvUsers.Dock = DockStyle.Fill;
            dgvUsers.BackgroundColor = Color.White;
            dgvUsers.BorderStyle = BorderStyle.None;
            dgvUsers.ReadOnly = true;
            dgvUsers.RowHeadersVisible = false;
            dgvUsers.AllowUserToAddRows = false;
            dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvUsers.ColumnHeadersHeight = 45;
            dgvUsers.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvUsers.EnableHeadersVisualStyles = false;
            dgvUsers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvUsers.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvUsers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            dgvUsers.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvUsers.DefaultCellStyle.SelectionBackColor = Color.FromArgb(41, 128, 185);
            dgvUsers.DefaultCellStyle.Padding = new Padding(5);
            dgvUsers.RowTemplate.Height = 35;

            // --- COLUMNS ---
            dgvUsers.AutoGenerateColumns = false;
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "UserId", DataPropertyName = "UserId", HeaderText = "ID", Visible = false });
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "Username", DataPropertyName = "Username", HeaderText = "Username", FillWeight = 100 });
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "FullName", DataPropertyName = "FullName", HeaderText = "Full Name", FillWeight = 150 });
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "Role", DataPropertyName = "Role", HeaderText = "Role", FillWeight = 80 });

            panelGrid.Controls.Add(dgvUsers);
        }

        private void LoadUsers()
        {
            try
            {
                dgvUsers.DataSource = db.ExecuteQuery("SELECT UserId, Username, FullName, Role FROM Users");
            }
            catch (Exception ex) { MessageBox.Show("Error loading users: " + ex.Message); }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            // --- POPUP DIALOG FOR ADDING USER ---
            Form addForm = new Form();
            addForm.Text = "Add New User";
            addForm.Size = new Size(400, 450);
            addForm.StartPosition = FormStartPosition.CenterParent;
            addForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            addForm.MaximizeBox = false;
            addForm.MinimizeBox = false;
            addForm.BackColor = Color.White;

            int y = 20;
            TextBox txtUsername = new TextBox();
            TextBox txtPassword = new TextBox();
            TextBox txtFullName = new TextBox();
            ComboBox cmbRole = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            cmbRole.Items.AddRange(new string[] { "Admin", "Cashier", "InventoryClerk" });
            cmbRole.SelectedIndex = 0;

            AddDialogInput(addForm, "Username:", txtUsername, ref y);

            // Password Field
            Label lblPass = new Label { Text = "Password:", Location = new Point(40, y), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            addForm.Controls.Add(lblPass);
            txtPassword.Location = new Point(40, y + 20);
            txtPassword.Size = new Size(300, 30);
            txtPassword.Font = new Font("Segoe UI", 10);
            txtPassword.PasswordChar = '*'; // Mask password
            addForm.Controls.Add(txtPassword);
            y += 60;

            AddDialogInput(addForm, "Full Name:", txtFullName, ref y);

            // Role Field
            Label lblRole = new Label { Text = "Role:", Location = new Point(40, y), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            addForm.Controls.Add(lblRole);
            cmbRole.Location = new Point(40, y + 20);
            cmbRole.Size = new Size(300, 30);
            cmbRole.Font = new Font("Segoe UI", 10);
            addForm.Controls.Add(cmbRole);
            y += 60;

            // Save Button
            y += 10;
            Button btnSave = new Button();
            btnSave.Text = "CREATE USER";
            btnSave.Location = new Point(40, y);
            btnSave.Size = new Size(300, 40);
            btnSave.BackColor = Color.FromArgb(46, 204, 113);
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.DialogResult = DialogResult.OK;
            addForm.Controls.Add(btnSave);

            if (addForm.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    MessageBox.Show("Username and Password are required!");
                    return;
                }

                try
                {
                    string query = "INSERT INTO Users (Username, Password, FullName, Role) VALUES (@u, @p, @n, @r)";
                    MySqlParameter[] p = {
                        new MySqlParameter("@u", txtUsername.Text),
                        new MySqlParameter("@p", txtPassword.Text),
                        new MySqlParameter("@n", txtFullName.Text),
                        new MySqlParameter("@r", cmbRole.Text)
                    };

                    if (db.ExecuteNonQuery(query, p) > 0)
                    {
                        MessageBox.Show("User Created!");
                        LoadUsers();
                    }
                }
                catch (Exception ex) { MessageBox.Show("Error saving user: " + ex.Message); }
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
            if (dgvUsers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a user to delete.", "Delete User", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Get Data
            string username = dgvUsers.SelectedRows[0].Cells["Username"].Value.ToString();
            string userId = dgvUsers.SelectedRows[0].Cells["UserId"].Value.ToString();

            // 3. Confirmation
            DialogResult result = MessageBox.Show(
                $"Are you sure you want to DELETE user '{username}'?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Stop);

            // 4. Action
            if (result == DialogResult.Yes)
            {
                try
                {
                    string query = "DELETE FROM Users WHERE UserId = @id";
                    MySqlParameter[] p = { new MySqlParameter("@id", userId) };
                    db.ExecuteNonQuery(query, p);

                    MessageBox.Show("User Deleted Successfully!");
                    LoadUsers();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting user: " + ex.Message);
                }
            }
        }
    }
}