using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.Windows.Forms;
using Testing.Database;

namespace Testing.Forms
{
    public partial class ReportsForm : Form
    {
        // 1. KEEPING YOUR EXISTING VARIABLES
        DatabaseConnection db = DatabaseConnection.Instance;
        DataGridView dgvReports;

        public ReportsForm()
        {
            InitializeComponent();

            // 2. KEEPING YOUR CORE LOGIC (Just separated into steps)
            // We create the grid first
            dgvReports = new DataGridView();

            // We load the data using YOUR query
            dgvReports.DataSource = db.ExecuteQuery("SELECT * FROM Transactions");

            // 3. UI IMPROVEMENT: We call this new method to style everything
            ApplyModernStyle();
        }

        private void ApplyModernStyle()
        {
            // --- A. FORM SETTINGS ---
            this.Text = "Sales Report";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;

            // --- B. ADD A HEADER (New!) ---
            Panel panelHeader = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(41, 128, 185) };
            Label lblTitle = new Label { Text = "TRANSACTION HISTORY", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = Color.White, Location = new Point(20, 15), AutoSize = true };
            panelHeader.Controls.Add(lblTitle);
            this.Controls.Add(panelHeader);

            // --- C. STYLE THE GRID (Making your dgvReports look good) ---
            dgvReports.Dock = DockStyle.Fill;
            dgvReports.BackgroundColor = Color.White;
            dgvReports.BorderStyle = BorderStyle.None;
            dgvReports.ReadOnly = true;
            dgvReports.RowHeadersVisible = false;
            dgvReports.AllowUserToAddRows = false;
            dgvReports.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvReports.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Header Style
            dgvReports.EnableHeadersVisualStyles = false;
            dgvReports.ColumnHeadersHeight = 45;
            dgvReports.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94); // Dark Blue Header
            dgvReports.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvReports.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvReports.ColumnHeadersDefaultCellStyle.Padding = new Padding(10, 0, 0, 0);

            // Row Style
            dgvReports.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvReports.DefaultCellStyle.SelectionBackColor = Color.FromArgb(41, 128, 185); // Light Blue Selection
            dgvReports.DefaultCellStyle.Padding = new Padding(10, 0, 0, 0);
            dgvReports.RowTemplate.Height = 35;
            dgvReports.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240); // Zebra striping

            // --- D. ADD GRID TO FORM ---
            // We add a container panel to ensure the grid sits BELOW the header
            Panel panelContent = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
            panelContent.Controls.Add(dgvReports);
            this.Controls.Add(panelContent);

            // Send header to back so it docks correctly at top
            panelHeader.SendToBack();
        }
    }
}