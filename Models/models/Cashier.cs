using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing.Models.models
{
    public class Cashier : User
    {
        public decimal TodaysSales { get; set; }
        public int TransactionCount { get; set; }

        public Cashier()
        {
            Role = "Cashier";
            TodaysSales = 0;
            TransactionCount = 0;
        }

        public override List<string> GetPermissions()
        {
            return new List<string>
            {
                "Process Sales",
                "Print Receipts",
                "View Inventory",
                "Request Refunds"
            };
        }

        public override void ShowDashboard()
        {
            // Dashboard logic
        }

        public void UpdateSalesMetrics(decimal amount)
        {
            TodaysSales += amount;
            TransactionCount++;
        }
    }
}
