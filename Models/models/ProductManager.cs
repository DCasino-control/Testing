using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing.Models.models
{
    public class ProductManager
    {
        private List<Product> products = new List<Product>();
        private int nextId = 1;

        public void AddProduct(Product product)
        {
            if (!products.Any(p => p.ProductCode == product.ProductCode))
            {
                product.ProductId = nextId++;
                products.Add(product);
            }
        }

        public void UpdateProduct(Product product)
        {
            var existing = products.FirstOrDefault(p => p.ProductId == product.ProductId);
            if (existing != null)
            {
                existing.ProductName = product.ProductName;
                existing.UnitPrice = product.UnitPrice;
                existing.CostPrice = product.CostPrice;
                existing.ModifiedDate = System.DateTime.Now;
            }
        }

        public void DeleteProduct(int productId)
        {
            var product = products.FirstOrDefault(p => p.ProductId == productId);
            if (product != null)
            {
                product.IsActive = false;
            }
        }

        public Product GetProductById(int id)
        {
            return products.FirstOrDefault(p => p.ProductId == id && p.IsActive);
        }

        public Product GetProductByCode(string code)
        {
            return products.FirstOrDefault(p => p.ProductCode == code && p.IsActive);
        }

        public List<Product> SearchProducts(string keyword)
        {
            return products.Where(p =>
                p.IsActive &&
                (p.ProductName.ToLower().Contains(keyword.ToLower()) ||
                 p.ProductCode.ToLower().Contains(keyword.ToLower()))
            ).ToList();
        }

        public List<Product> GetLowStockProducts()
        {
            return products.Where(p => p.IsActive && p.IsLowStock()).ToList();
        }

        public List<Product> GetAllProducts()
        {
            return products.Where(p => p.IsActive).ToList();
        }
    }
}

