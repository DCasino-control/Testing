using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing.Models.models
{
    public class InventoryClerk : User
    {
        public InventoryClerk()
        {
            Role = "Inventory Clerk";
        }

        public override List<string> GetPermissions()
        {
            return new List<string>
            {
                "Manage Stock",
                "Record Stock In/Out",
                "Update Products",
                "Generate Inventory Reports"
            };
        }

        public override void ShowDashboard()
        {
            // Dashboard logic
        }
    }
}

