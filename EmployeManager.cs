using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Testing
{
    public partial class EmployeManager : Form
    {
        public EmployeManager()
        {
            InitializeComponent();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            DASHBOARD childForm = new DASHBOARD();
            childForm.TopLevel = false;
            childForm.TopMost = true;
            childForm.FormBorderStyle = FormBorderStyle.None;
            panel1.Controls.Add(childForm);
            childForm.Show();
        }
    }
}
