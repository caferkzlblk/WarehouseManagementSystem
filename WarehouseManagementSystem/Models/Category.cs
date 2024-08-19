using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WarehouseManagementSystem.Models;

namespace WarehouseManagementSystem.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } 
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
