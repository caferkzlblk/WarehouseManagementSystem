namespace WarehouseManagementSystem.Models
{
    public class OrderFilterViewModel
    {
        public IEnumerable<Order> Orders { get; set; } // Siparişler listesi
        public int OrderID { get; set; }
        public IEnumerable<Statuses> Status { get; set; } // Durumlar listesi
        public int? CustomerID { get; set; } // Müşteri ID'si
        public int? StatusID { get; set; } // Durum ID'si
        public DateTime? StartDate { get; set; } // Başlangıç tarihi
        public DateTime? EndDate { get; set; } // Bitiş tarihi
        public string OrderNumberValue { get; set; } // Sipariş numarası
        public List<ProductViewModel> Products { get; set; } // Ürünler listesi
        public IEnumerable<OrderProductSummaryViewModel> OrderProductSummary { get; set; } // Sipariş ürün özeti

        public IEnumerable<Shipment> Shipments { get; set; }
        public List<ShippingRates> ShippingRates { get; set; }
    }

    public class ProductViewModel
    {
        public string ProductName { get; set; } // Ürün adı
        public int Quantity { get; set; } // Miktar
    }
}
