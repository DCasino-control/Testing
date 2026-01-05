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
    public partial class LoginForm : Form
    {
        private UserManager userManager;

        // UI Controls
        private Panel mainPanel;
        private Label titleLabel;
        private Label subtitleLabel;
        private Label usernameLabel;
        private TextBox usernameTextBox;
        private Label passwordLabel;
        private TextBox passwordTextBox;
        private Button loginButton;
        private Label messageLabel;
        private Panel headerPanel;

        public LoginForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
            userManager = new UserManager();
            LoadDefaultUsers();
        }

       

        private void InitializeCustomComponents()
        {
            // ═══ HEADER PANEL ═══
            headerPanel = new Panel
            {
                Size = new Size(450, 100),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(33, 150, 243)
            };
            this.Controls.Add(headerPanel);

            // ═══ TITLE ═══
            titleLabel = new Label
            {
                Text = "🏪 UMVC CANTEEN",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.White,
                Size = new Size(450, 40),
                Location = new Point(0, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };
            headerPanel.Controls.Add(titleLabel);

            // ═══ SUBTITLE ═══
            subtitleLabel = new Label
            {
                Text = "Inventory & POS System",
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.White,
                Size = new Size(450, 30),
                Location = new Point(0, 60),
                TextAlign = ContentAlignment.MiddleCenter
            };
            headerPanel.Controls.Add(subtitleLabel);

            // ═══ MAIN PANEL ═══
            mainPanel = new Panel
            {
                Size = new Size(350, 320),
                Location = new Point(50, 130),
                BackColor = Color.White
            };
            // Add shadow effect
            mainPanel.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, mainPanel.ClientRectangle,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid);
            };
            this.Controls.Add(mainPanel);

            // ═══ USERNAME LABEL ═══
            usernameLabel = new Label
            {
                Text = "Username",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Size = new Size(300, 25),
                Location = new Point(25, 30)
            };
            mainPanel.Controls.Add(usernameLabel);

            // ═══ USERNAME TEXTBOX ═══
            usernameTextBox = new TextBox
            {
                Font = new Font("Segoe UI", 12),
                Size = new Size(300, 35),
                Location = new Point(25, 60),
                BorderStyle = BorderStyle.FixedSingle
            };
            mainPanel.Controls.Add(usernameTextBox);

            // ═══ PASSWORD LABEL ═══
            passwordLabel = new Label
            {
                Text = "Password",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Size = new Size(300, 25),
                Location = new Point(25, 110)
            };
            mainPanel.Controls.Add(passwordLabel);

            // ═══ PASSWORD TEXTBOX ═══
            passwordTextBox = new TextBox
            {
                Font = new Font("Segoe UI", 12),
                Size = new Size(300, 35),
                Location = new Point(25, 140),
                BorderStyle = BorderStyle.FixedSingle,
                PasswordChar = '●'
            };
            mainPanel.Controls.Add(passwordTextBox);

            // ═══ LOGIN BUTTON ═══
            loginButton = new Button
            {
                Text = "LOGIN",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(300, 45),
                Location = new Point(25, 200),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            loginButton.FlatAppearance.BorderSize = 0;
            loginButton.Click += LoginButton_Click;
            mainPanel.Controls.Add(loginButton);

            // ═══ MESSAGE LABEL ═══
            messageLabel = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.Red,
                Size = new Size(300, 40),
                Location = new Point(25, 260),
                TextAlign = ContentAlignment.MiddleCenter
            };
            mainPanel.Controls.Add(messageLabel);

            // ═══ DEFAULT CREDENTIALS INFO ═══
            Label infoLabel = new Label
            {
                Text = "Default Login:\nAdmin: admin / admin123\nCashier: cashier1 / cash123",
                Font = new Font("Segoe UI", 8, FontStyle.Italic),
                ForeColor = Color.Gray,
                Size = new Size(450, 60),
                Location = new Point(0, 460),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(infoLabel);

            // ═══ ENTER KEY SUPPORT ═══
            this.AcceptButton = loginButton;
            usernameTextBox.KeyPress += (s, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    passwordTextBox.Focus();
                    e.Handled = true;
                }
            };
        }

        private void LoadDefaultUsers()
        {
            // Create default users
            userManager.AddUser(new Admin
            {
                UserId = 1,
                Username = "admin",
                Password = "admin123",
                FullName = "Juan Dela Cruz",
                Email = "admin@umvc.edu"
            });

            userManager.AddUser(new Cashier
            {
                UserId = 2,
                Username = "cashier1",
                Password = "cash123",
                FullName = "Maria Santos",
                Email = "maria@umvc.edu"
            });

            userManager.AddUser(new InventoryClerk
            {
                UserId = 3,
                Username = "clerk1",
                Password = "clerk123",
                FullName = "Pedro Garcia",
                Email = "pedro@umvc.edu"
            });
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            // Clear previous message
            messageLabel.Text = "";
            messageLabel.ForeColor = Color.Red;

            // Validate input
            if (string.IsNullOrWhiteSpace(usernameTextBox.Text))
            {
                messageLabel.Text = "Please enter username";
                usernameTextBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(passwordTextBox.Text))
            {
                messageLabel.Text = "Please enter password";
                passwordTextBox.Focus();
                return;
            }

            // Disable button during login
            loginButton.Enabled = false;
            loginButton.Text = "Logging in...";

            // Attempt login
            User user = userManager.Authenticate(usernameTextBox.Text, passwordTextBox.Text);

            if (user != null)
            {
                // Success!
                messageLabel.ForeColor = Color.Green;
                messageLabel.Text = $"✓ Welcome {user.FullName}!";

                // Show message for 1 second then open dashboard
                Timer timer = new Timer();
                timer.Interval = 1000;
                timer.Tick += (s, ev) =>
                {
                    timer.Stop();
                    OpenDashboard(user);
                };
                timer.Start();
            }
            else
            {
                // Failed
                messageLabel.Text = "✗ Invalid username or password";
                passwordTextBox.Clear();
                passwordTextBox.Focus();
                loginButton.Enabled = true;
                loginButton.Text = "LOGIN";

                // Shake animation
                ShakeForm();
            }
        }

        private void ShakeForm()
        {
            // Simple shake effect
            Point originalLocation = this.Location;
            Timer shakeTimer = new Timer();
            shakeTimer.Interval = 50;
            int shakeCount = 0;

            shakeTimer.Tick += (s, e) =>
            {
                shakeCount++;
                if (shakeCount % 2 == 0)
                    this.Location = new Point(originalLocation.X + 10, originalLocation.Y);
                else
                    this.Location = originalLocation;

                if (shakeCount >= 6)
                {
                    shakeTimer.Stop();
                    this.Location = originalLocation;
                }
            };
            shakeTimer.Start();
        }

        private void OpenDashboard(User user)
        {
            // Hide login form
            this.Hide();

            // Open dashboard based on user role
            Form dashboard = null;

            if (user is Admin)
            {
                dashboard = new AdminDashboard(user as Admin);
            }
            else if (user is Cashier)
            {
                dashboard = new CashierDashboard(user as Cashier);
            }
            else if (user is InventoryClerk)
            {
                dashboard = new InventoryDashboard(user as InventoryClerk);
            }

            if (dashboard != null)
            {
                dashboard.FormClosed += (s, e) =>
                {
                    // Show login again when dashboard closes
                    this.Show();
                    usernameTextBox.Clear();
                    passwordTextBox.Clear();
                    usernameTextBox.Focus();
                    loginButton.Enabled = true;
                    loginButton.Text = "LOGIN";
                    messageLabel.Text = "";
                };
                dashboard.Show();
            }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }
    }
}
