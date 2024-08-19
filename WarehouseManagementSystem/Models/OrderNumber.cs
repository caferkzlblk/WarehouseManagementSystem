using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehouseManagementSystem.Models
{
    public class OrderNumber
    {
        [Key]
        public int OrderNumberID { get; set; }

        [Required]
        public string? GeneratedOrderNumber { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public string? OrderNumberValue { get; set; }

        // Foreign key for Order
        [ForeignKey("OrderID")]
        public int OrderID { get; set; }

        // Navigation property
        public Order Order { get; set; }
    }
}
