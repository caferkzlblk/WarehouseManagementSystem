namespace WarehouseManagementSystem.Models
{
    public class OrderCreateViewModel
    {
        public OrderCreateViewModel()
        {
            SelectedProductIDs = new List<int>();
            Quantities = new List<int>();
        }
        public Order Order { get; set; }
        public IEnumerable<Product> AvailableProducts { get; set; }
       
        public List<int> SelectedProductIDs { get; set; } = new List<int>();
        public List<int> Quantities { get; set; } = new List<int>();
        public IEnumerable<ShippingCompany> ShippingCompanies { get; set; }
        public ShippingCompany ShippingCompanyID { get; set; }

        public List<ShippingRates> ShippingRates { get; set; }
        public double CalculatedShippingCost { get; set; }
    }

}
