using System.ComponentModel.DataAnnotations;

namespace WarehouseManagementSystem.Models
{
    public class Statuses
    {
        [Key]
        public int StatusID { get; set; }
        public string StatusName { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Shipment> Shipments { get; set; }

    }
}
