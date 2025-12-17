using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing.Models.models
{
    public class PurchaseOrder 
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public int CreatedBy { get; set; }
        public List<PurchaseOrderItem> Items { get; set; }

        public PurchaseOrder()
        {
            Items = new List<PurchaseOrderItem>();
            OrderDate = DateTime.Now;
            Status = "Pending";
            OrderNumber = $"PO-{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}";
        }

        public void AddItem(Product product, int quantity, decimal unitCost)
        {
            var item = new PurchaseOrderItem
            {
                Product = product,
                ProductId = product.ProductId,
                OrderQuantity = quantity,
                UnitCost = unitCost
            };
            item.CalculateSubtotal();
            Items.Add(item);
            CalculateTotal();
        }

        public void CalculateTotal()
        {
            TotalAmount = Items.Sum(i => i.Subtotal);
        }

        public void Print()
        {
            // Print logic
        }
    }

    public class PurchaseOrderItem
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int OrderQuantity { get; set; }
        public int ReceivedQuantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal Subtotal { get; set; }

        public void CalculateSubtotal()
        {
            Subtotal = OrderQuantity * UnitCost;
        }
    }
}

