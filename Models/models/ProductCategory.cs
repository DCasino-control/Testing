using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing.Models.models
{
public   class ProductCategory
    {
        public int CategoryID { get; set; }
        public String CategoryName { get; set; }
        public String Description { get; set; }
        public bool isActive { get; set; }

        public List<string> GetProduct()
        {
            return new List<string>
            {
                "Product1",
                "Product2",
                "Product3"
            };
        }
        public ProductCategory() { }

        public ProductCategory(int id, string name, string description = "")
        {
            CategoryID = id;
            CategoryName = name;
            Description = description;
        }

    }
}
