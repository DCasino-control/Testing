using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing.Models.models;

namespace Testing.Models
{
   
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public ProductCategory Category { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal CostPrice { get; set; }
        public int StockQuantity { get; set; }
        public int CriticalStockLevel { get; set; }
        public string ImagePath { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public Product()
        {
            IsActive = true;
            CreatedDate = DateTime.Now;
            ModifiedDate = DateTime.Now;
        }

        public bool IsLowStock()
        {
            return StockQuantity <= CriticalStockLevel;
        }

        public decimal GetProfitMargin()
        {
            return UnitPrice - CostPrice;
        }

        public bool AddStock(int quantity)
        {
            if (quantity > 0)
            {
                StockQuantity += quantity;
                ModifiedDate = DateTime.Now;
                return true;
            }
            return false;
        }

        public bool RemoveStock(int quantity)
        {
            if (quantity > 0 && StockQuantity >= quantity)
            {
                StockQuantity -= quantity;
                ModifiedDate = DateTime.Now;
                return true;
            }
            return false;
        }
    }
}

