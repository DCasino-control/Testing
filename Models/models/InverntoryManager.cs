using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing.Models.models
{
    public class InventoryManager
    {
        private List<StockMovement> movements = new List<StockMovement>();
        private int nextId = 1;

        public bool RecordStockIn(Product product, int quantity, string reference, int userId)
        {
            if (product.AddStock(quantity))
            {
                var movement = new StockMovement
                {
                    MovementId = nextId++,
                    Product = product,
                    ProductId = product.ProductId,
                    MovementType = "In",
                    Quantity = quantity,
                    UserId = userId,
                    Reference = reference
                };
                movements.Add(movement);
                return true;
            }
            return false;
        }

        public bool RecordStockOut(Product product, int quantity, string reference, int userId)
        {
            if (product.RemoveStock(quantity))
            {
                var movement = new StockMovement
                {
                    MovementId = nextId++,
                    Product = product,
                    ProductId = product.ProductId,
                    MovementType = "Out",
                    Quantity = quantity,
                    UserId = userId,
                    Reference = reference
                };
                movements.Add(movement);
                return true;
            }
            return false;
        }

        public List<Product> CheckLowStockAlerts(List<Product> products)
        {
            return products.Where(p => p.IsLowStock()).ToList();
        }

        public List<StockMovement> GetMovementHistory()
        {
            return movements;
        }
    }
}

