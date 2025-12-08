using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing.Models.models
{
    public class Admin : User
    {
        public Admin()
        {
            Role = "Admin";
        }

        // E-Implement ang abstract na methods
        public override List<string> GetPermissions()
        {
            return new List<string>
            {
                "Manage Users",
                "Adjust Prices",
                "View All Reports",
                "Approve Refunds",
                "System Configuration"
            };
        }

        public override void ShowDashboard()
        {
            // mao ni mo show ug Dashboard process para sa Admin
        }
    }
}

