using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing.Models.models
{
    public class TransactionItem
    {
        public int ItemId { get; set; }
        public int TransactionId { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }

        public TransactionItem() { }

        public TransactionItem(Product product, int quantity)
        {
            Product = product;
            ProductId = product.ProductId;
            Quantity = quantity;
            UnitPrice = product.UnitPrice;
            CalculateSubtotal();
        }

        public void CalculateSubtotal()
        {
            Subtotal = UnitPrice * Quantity;
        }
    }
}

