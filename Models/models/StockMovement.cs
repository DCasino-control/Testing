using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing.Models.models
{
    public class StockMovement
    {
        public int MovementId { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public string MovementType { get; set; } // In, Out, Adjustment
        public int Quantity { get; set; }
        public DateTime MovementDate { get; set; }
        public int UserId { get; set; }
        public string Reference { get; set; }
        public string Notes { get; set; }

        public StockMovement()
        {
            MovementDate = DateTime.Now;
        }
    }
}

