using System.ComponentModel.DataAnnotations;

namespace WarehouseManagementSystem.Models
{
    public class Statuses
    {
        [Key]
        
        public int StatusID { get; set; }
        public string StatusName { get; set; }

        public ICollection<OrderDetails> Orders { get; set; }
        public ICollection<OrderDetails> OrderDetails { get; set; }

    }
}
