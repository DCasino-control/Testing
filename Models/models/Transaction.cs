using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing.Models.models
{
    public class Transaction 
    {
        public int TransactionId { get; set; }
        public DateTime TransactionDate { get; set; }
        public int CashierId { get; set; }
        public Cashier Cashier { get; set; }
        public List<TransactionItem> Items { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal ChangeAmount { get; set; }
        public string Status { get; set; }
        public string VoidReason { get; set; }
        public int? VoidedBy { get; set; }
        public DateTime? VoidedDate { get; set; }

        public Transaction()
        {
            Items = new List<TransactionItem>();
            TransactionDate = DateTime.Now;
            Status = "Pending";
        }

        public bool AddItem(Product product, int quantity)
        {
            if (product == null || quantity <= 0) return false;

            if (!product.RemoveStock(quantity)) return false;

            var item = new TransactionItem(product, quantity);
            Items.Add(item);
            CalculateTotal();
            return true;
        }

        public void RemoveItem(int itemIndex)
        {
            if (itemIndex >= 0 && itemIndex < Items.Count)
            {
                var item = Items[itemIndex];
                item.Product.AddStock(item.Quantity);
                Items.RemoveAt(itemIndex);
                CalculateTotal();
            }
        }

        public void CalculateTotal()
        {
            TotalAmount = Items.Sum(item => item.Subtotal);
        }

        public bool CompleteSale(decimal amountPaid)
        {
            AmountPaid = amountPaid;
            ChangeAmount = AmountPaid - TotalAmount;

            if (ChangeAmount < 0) return false;

            Status = "Completed";
            return true;
        }

        public void Print()
        {
            // Receipt printing logic
        }

        public bool VoidTransaction(int managerId, string reason)
        {
            if (Status != "Completed") return false;

            foreach (var item in Items)
            {
                item.Product.AddStock(item.Quantity);
            }

            Status = "Voided";
            VoidedBy = managerId;
            VoidedDate = DateTime.Now;
            VoidReason = reason;
            return true;
        }
    }
}

