using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing.Models.models
{
    public class SupplierManager
    {
        private List<Supplier> suppliers = new List<Supplier>();
        private int nextId = 1;

        public void AddSupplier(Supplier supplier)
        {
            supplier.SupplierId = nextId++;
            suppliers.Add(supplier);
        }

        public Supplier GetSupplierById(int id)
        {
            return suppliers.FirstOrDefault(s => s.SupplierId == id && s.IsActive);
        }

        public List<Supplier> GetAllSuppliers()
        {
            return suppliers.Where(s => s.IsActive).ToList();
        }
    }
}
