using System;
using System.Drawing;
using System.Windows.Forms; // This is required
using Testing.Database;

namespace Testing.Forms
{
    // FIX IS HERE: We added ": Form" to tell the app this is a Window
    public partial class InventoryClerkDashboard : Form
    {
        private int currentUserId;
        private string currentUserName;

        public InventoryClerkDashboard(int userId, string userName)
        {
            currentUserId = userId;
            currentUserName = userName;

            // This builds the window
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // InventoryClerkDashboard
            // 
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "InventoryClerkDashboard";
            this.Load += new System.EventHandler(this.InventoryClerkDashboard_Load);
            this.ResumeLayout(false);

        }

        private void InventoryClerkDashboard_Load(object sender, EventArgs e)
        {

        }
    }
}