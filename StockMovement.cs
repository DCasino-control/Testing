using System;
using System.Drawing;
using System.Windows.Forms;
using Testing.Database;
using MySql.Data.MySqlClient;

namespace Testing.Forms
{
    public partial class StockMovementForm : Form
    {
        private int productId;
        private string productName;
        private string movementType; // "In" or "Out"
        private int userId;
        private DatabaseConnection db;

        private Label lblTitle;
        private Label lblProduct;
        private Label lblQuantity;
        private NumericUpDown numQuantity;
        private Label lblReference;
        private TextBox txtReference;
        private Label lblNotes;
        private TextBox txtNotes;
        private Button btnSave;
        private Button btnCancel;
        private Panel panelTop;

        public StockMovementForm(int prodId, string prodName, string type, int uId)
        {
            productId = prodId;
            productName = prodName;
            movementType = type;
            userId = uId;
            db = DatabaseConnection.Instance;

            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = $"Stock {movementType} - {productName}";
            this.Size = new Size(500, 450);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            Color headerColor = movementType == "In" ?
                Color.FromArgb(39, 174, 96) : Color.FromArgb(230, 126, 34);

            // Top Panel
            panelTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = headerColor
            };
            this.Controls.Add(panelTop);

            lblTitle = new Label
            {
                Text = $"STOCK {movementType.ToUpper()}",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            panelTop.Controls.Add(lblTitle);

            // Product Label
            lblProduct = new Label
            {
                Text = $"Product: {productName}",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(30, 90),
                AutoSize = true
            };
            this.Controls.Add(lblProduct);

            // Quantity
            lblQuantity = new Label
            {
                Text = "Quantity:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(30, 130),
                AutoSize = true
            };
            this.Controls.Add(lblQuantity);

            numQuantity = new NumericUpDown
            {
                Font = new Font("Segoe UI", 12),
                Location = new Point(30, 155),
                Size = new Size(420, 30),
                Minimum = 1,
                Maximum = 10000,
                Value = 1
            };
            this.Controls.Add(numQuantity);

            // Reference
            lblReference = new Label
            {
                Text = "Reference Number:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(30, 200),
                AutoSize = true
            };
            this.Controls.Add(lblReference);

            txtReference = new TextBox
            {
                Font = new Font("Segoe UI", 12),
                Location = new Point(30, 225),
                Size = new Size(420, 30),
   
            };
            this.Controls.Add(txtReference);

            // Notes
            lblNotes = new Label
            {
                Text = "Notes:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(30, 270),
                AutoSize = true
            };
            this.Controls.Add(lblNotes);

            txtNotes = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(30, 295),
                Size = new Size(420, 60),
                Multiline = true,
              
            };
            this.Controls.Add(txtNotes);

            // Buttons
            btnCancel = new Button
            {
                Text = "CANCEL",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Size = new Size(200, 45),
                Location = new Point(30, 370),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += BtnCancel_Click;
            this.Controls.Add(btnCancel);

            btnSave = new Button
            {
                Text = $"SAVE STOCK {movementType.ToUpper()}",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Size = new Size(220, 45),
                Location = new Point(240, 370),
                BackColor = headerColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Validate inputs
            if (numQuantity.Value <= 0)
            {
                MessageBox.Show("Please enter a valid quantity.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtReference.Text))
            {
                MessageBox.Show("Please enter a reference number.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtReference.Focus();
                return;
            }

            // Confirm action
            string message = $"Confirm stock {movementType.ToLower()} of {numQuantity.Value} units?\n\n" +
                           $"Product: {productName}\n" +
                           $"Quantity: {numQuantity.Value}\n" +
                           $"Reference: {txtReference.Text}";

            if (MessageBox.Show(message, "Confirm Stock Movement",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (SaveStockMovement())
                {
                    MessageBox.Show($"Stock {movementType.ToLower()} recorded successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }

        private bool SaveStockMovement()
        {
            MySqlConnection conn = db.GetConnection();
            MySqlTransaction transaction = null;

            try
            {
                transaction = conn.BeginTransaction();

                // Check current stock for "Out" movement
                if (movementType == "Out")
                {
                    string checkQuery = "SELECT StockQuantity FROM Products WHERE ProductId = @productId";
                    MySqlParameter[] checkParams = {
                        new MySqlParameter("@productId", productId)
                    };

                    MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn, transaction);
                    checkCmd.Parameters.AddRange(checkParams);
                    int currentStock = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (currentStock < numQuantity.Value)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Insufficient stock! Current stock: {currentStock}",
                            "Stock Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }

                // Insert stock movement record
                string movementQuery = @"INSERT INTO StockMovements 
                    (ProductId, MovementType, Quantity, UserId, Reference, Notes)
                    VALUES (@productId, @type, @qty, @userId, @ref, @notes)";

                MySqlParameter[] movementParams = {
                    new MySqlParameter("@productId", productId),
                    new MySqlParameter("@type", movementType),
                    new MySqlParameter("@qty", numQuantity.Value),
                    new MySqlParameter("@userId", userId),
                    new MySqlParameter("@ref", txtReference.Text.Trim()),
                    new MySqlParameter("@notes", txtNotes.Text.Trim())
                };

                MySqlCommand movementCmd = new MySqlCommand(movementQuery, conn, transaction);
                movementCmd.Parameters.AddRange(movementParams);
                movementCmd.ExecuteNonQuery();

                // Update product stock
                string updateQuery = movementType == "In" ?
                    "UPDATE Products SET StockQuantity = StockQuantity + @qty WHERE ProductId = @productId" :
                    "UPDATE Products SET StockQuantity = StockQuantity - @qty WHERE ProductId = @productId";

                MySqlParameter[] updateParams = {
                    new MySqlParameter("@qty", numQuantity.Value),
                    new MySqlParameter("@productId", productId)
                };

                MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn, transaction);
                updateCmd.Parameters.AddRange(updateParams);
                updateCmd.ExecuteNonQuery();

                // Log activity
                string logQuery = @"INSERT INTO ActivityLogs (UserId, Action, Description)
                    VALUES (@userId, @action, @description)";

                MySqlParameter[] logParams = {
                    new MySqlParameter("@userId", userId),
                    new MySqlParameter("@action", $"Stock {movementType}"),
                    new MySqlParameter("@description",
                        $"Stock {movementType}: {productName}, Qty: {numQuantity.Value}, Ref: {txtReference.Text}")
                };

                MySqlCommand logCmd = new MySqlCommand(logQuery, conn, transaction);
                logCmd.Parameters.AddRange(logParams);
                logCmd.ExecuteNonQuery();

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                MessageBox.Show($"Error saving stock movement: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Cancel this stock movement?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // StockMovementForm
            // 
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "StockMovementForm";
            this.Load += new System.EventHandler(this.StockMovementForm_Load);
            this.ResumeLayout(false);

        }

        private void StockMovementForm_Load(object sender, EventArgs e)
        {

        }
    }
}