using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WarehouseManagementSystem.Models
{
    public class ShippingCompany
    {
        [Key]
        
        public int ShippingCompanyID { get; set; }

        [Required]
        [StringLength(255)]
        
        public string CompanyName { get; set; }

        [Required]
        [StringLength(255)]
        public string Phone { get; set; }

        [Required]
        [StringLength(255)]
        public string Address { get; set; }
    }
}
