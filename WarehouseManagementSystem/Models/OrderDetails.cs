using System.ComponentModel.DataAnnotations;

namespace WarehouseManagementSystem.Models
{
    public class OrderDetails
    {
        [Key]
        public int OrderDetailsID { get; set; }

        // Foreign keys
        public int OrderID { get; set; }
        public Order Order { get; set; }

        public int ProductID { get; set; }
        public Product Product { get; set; }

    
        public int Quantity { get; set; }
        public double Price { get; set; }
    }

}
