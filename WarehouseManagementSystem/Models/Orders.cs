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


        public decimal TotalCost { get; set; }

        public int StatusID { get; set; }
        [ForeignKey("StatusID")]
        public Statuses Status { get; set; } // Navigation property

        public string? OrderNumberValue { get; set; }

        public int ShippingCompanyID { get; set; }
        [ForeignKey("ShippingCompanyID")]
        public ShippingCompany ShippingCompany { get; set; } // Navigation property

        public int? OrderNumberID { get; set; } // Nullable because it may be assigned later
        [ForeignKey("OrderNumberID")]
        public OrderNumber? OrderNumber { get; set; } // Navigation property

        public ICollection<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();
        public Shipment? Shipment { get; set; }
    }
}
