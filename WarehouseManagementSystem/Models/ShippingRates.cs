using System.ComponentModel.DataAnnotations;

namespace WarehouseManagementSystem.Models
{
    public class ShippingRates
    {
        [Key]
        public int ShippingRateID { get; set; }
        public int ShippingCompanyID { get; set; }
        public int MinProductCount { get; set; } 
        public int MaxProductCount { get; set; } 
        public double Rate { get; set; } 

        public ShippingCompany ShippingCompany { get; set; }
    }
}
