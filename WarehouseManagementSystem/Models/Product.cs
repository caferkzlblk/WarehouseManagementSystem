using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehouseManagementSystem.Models
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public int QuantityInStock { get; set; }
        public int Price { get; set; }

        // Foreign keys
        public int CategoryID { get; set; }
        public Category Category { get; set; }

        public int SupplierID { get; set; }
        public Supplier Supplier { get; set; }

        // Navigation property
        public ICollection<OrderDetails> OrderDetails { get; set; }
    }

}
