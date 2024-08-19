using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehouseManagementSystem.Models
{
    public class Order
    {
        [Key]
        public int OrderID { get; set; }

        public DateTime OrderDate { get; set; }

        public int CustomerID { get; set; }

        public int StatusID { get; set; }

        public Statuses? Statuses { get; set; }

        public string? OrderNumberValue { get; set; }

        public int? OrderNumberID { get; set; } // Nullable because it may be assigned later

        [ForeignKey("OrderNumberID")]
        public OrderNumber? OrderNumber { get; set; } // Navigation property

        // Navigation property
        public ICollection<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();
    }
}
