using System.ComponentModel.DataAnnotations;

namespace WarehouseManagementSystem.Models
{
    public class Role
    {
        [Key]
        public int RoleID { get; set; }
        public string RoleName { get; set; }

        // Navigation property
        public ICollection<Users> Users { get; set; }
    }

}
