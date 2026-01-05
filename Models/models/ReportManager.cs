using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing.Models.models
{
    public class ReportManager
    {
        public class SalesReport : Report
        {
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public decimal TotalSales { get; set; }
            public int TransactionCount { get; set; }
            public List<Transaction> Transactions { get; set; }

            public SalesReport() : base("Sales Report")
            {
                Transactions = new List<Transaction>();
            }

            public override void Generate()
            {
                TotalSales = Transactions.Where(t => t.Status == "Completed").Sum(t => t.TotalAmount);
                TransactionCount = Transactions.Count(t => t.Status == "Completed");
            }

            public override void Display()
            {
                // Kani kay mo display sa report
            }
        }

        public class InventoryReport : Report
        {
            public List<Product> Products { get; set; }
            public decimal TotalValue { get; set; }
            public int LowStockCount { get; set; }

            public InventoryReport() : base("Inventory Report")
            {
                Products = new List<Product>();
            }

            public override void Generate()
            {
                TotalValue = Products.Sum(p => p.StockQuantity * p.CostPrice);
                LowStockCount = Products.Count(p => p.IsLowStock());
            }

            public override void Display()
            {
                // Kani kay mo display sa report
            }
        }

        public SalesReport GenerateSalesReport(DateTime startDate, DateTime endDate, List<Transaction> transactions)
        {
            var report = new SalesReport
            {
                StartDate = startDate,
                EndDate = endDate,
                Transactions = transactions.Where(t =>
                    t.TransactionDate >= startDate &&
                    t.TransactionDate <= endDate &&
                    t.Status == "Completed"
                ).ToList()
            };
            report.Generate();
            return report;
        }

        public InventoryReport GenerateInventoryReport(List<Product> products)
        {
            var report = new InventoryReport
            {
                Products = products
            };
            report.Generate();
            return report;
        }
    }
}