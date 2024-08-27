using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehouseManagementSystem.Models
{
    public class OrderDetails
    {
        [Key]
        public int OrderDetailsID { get; set; }

        // Foreign keys
        public int OrderID { get; set; }
        [ForeignKey("OrderID")]
        public Order Order { get; set; }

        public int ProductID { get; set; }
        [ForeignKey("ProductID")]
        public Product Product { get; set; }

        public int StatusID { get; set; }
        [ForeignKey("StatusID")]
        public Statuses Status { get; set; } // Navigation property

        public int Quantity { get; set; }
        public double Price { get; set; }
    }
}
