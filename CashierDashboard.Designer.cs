using System;
using System.Windows.Forms;

namespace Testing.Forms
{
    partial class CashierDashboard
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        // We REMOVED InitializeComponent() from here because 
        // it is already inside your CashierDashboard.cs file.
        // Keeping it here would cause a "Duplicate Method" error.
    }
}