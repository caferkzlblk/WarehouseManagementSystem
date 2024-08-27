using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehouseManagementSystem.Models
{
    public class Shipment
    {
        [Key]
        public int ShipmentID { get; set; }

        [Required]
        public int OrderID { get; set; }

        [Required]
        [StringLength(255)]
        public string ShipmentNumber { get; set; }

        [Required]
        public int ShippingCompanyID { get; set; }

        [Required]
        public DateTime ShipmentDate { get; set; }

        [Required]
     
        public int StatusID { get; set; }

        // Navigation Properties
        public virtual Order Order { get; set; }
        public virtual ShippingCompany ShippingCompany { get; set; }
        public Statuses Status { get; set; }
    }
}
