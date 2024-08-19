namespace WarehouseManagementSystem.Models
{
    public class OrderFilterViewModel
    {
        public IEnumerable<Order> Orders { get; set; }
   
        public IEnumerable<Statuses> Statuses { get; set; }
        public int? CustomerID { get; set; }
        public int? StatusID { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string OrderNumberValue { get; set; }
        public List<ProductViewModel> Products { get; set; }

    }
    public class ProductViewModel
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
    }
}
