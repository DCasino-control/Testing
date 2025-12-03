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
    public partial class USER_LOGIN : Form
    {
        public USER_LOGIN()
        {
            InitializeComponent();
        }
        private void button4_Click_1(object sender, EventArgs e)
        {
            portal childForm = new portal();
            childForm.ShowDialog();
            this.Close();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            CASHIER childForm = new CASHIER();

            childForm.ShowDialog();
            this.Close();
        }
    }
}
