using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.EMMA;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using WarehouseManagementSystem.Data;
using WarehouseManagementSystem.Models;

namespace WarehouseManagementSystem.Controllers
{
    
 
    public class HomeController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly ApplicationDbContext context;
        
        public HomeController(ApplicationDbContext context, INotificationService notificationService)
        {
            this.context = context;
            _notificationService = notificationService;
        }

        [HttpGet("Orders/Export")]
        public async Task<IActionResult> ExportOrdersToExcel(int? statusID, DateTime? startDate, DateTime? endDate)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var filteredOrders = context.Orders.AsQueryable();

            if (statusID.HasValue)
            {
                filteredOrders = filteredOrders.Where(o => o.StatusID == statusID.Value);
            }
            if (startDate.HasValue)
            {
                filteredOrders = filteredOrders.Where(o => o.OrderDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                filteredOrders = filteredOrders.Where(o => o.OrderDate <= endDate.Value);
            }

            var ordersToExport = await filteredOrders
                .Include(o => o.Status)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .ToListAsync();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Orders");

               
                worksheet.Cells[1, 1].Value = "Sipariþ Tarihi";
                worksheet.Cells[1, 2].Value = "Müþteri ID";
                worksheet.Cells[1, 3].Value = "Durum";
                worksheet.Cells[1, 4].Value = "Ürün Adý";
                worksheet.Cells[1, 5].Value = "Miktar";

              
                int row = 2;
                foreach (var order in ordersToExport)
                {
                    foreach (var detail in order.OrderDetails)
                    {
                        worksheet.Cells[row, 1].Value = order.OrderDate.ToString("yyyy-MM-dd");
                        worksheet.Cells[row, 2].Value = order.CustomerID;
                        worksheet.Cells[row, 3].Value = order.Status.StatusName;
                        worksheet.Cells[row, 4].Value = detail.Product.ProductName;
                        worksheet.Cells[row, 5].Value = detail.Quantity;
                        row++;
                    }
                }

                worksheet.Cells.AutoFitColumns();

                var excelData = package.GetAsByteArray();
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var fileName = $"FilteredOrders_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.xlsx";

                return File(excelData, contentType, fileName);
            }
        }


        
        public IActionResult Index()
        {
          
            var products = context.Products.ToList();
            var lowStockProducts = CheckStockLevels();
            ViewBag.LowStockProducts = lowStockProducts;

            var totalStock = products.Sum(p => p.QuantityInStock);
            var productCount = products.Count();

          
            ViewBag.TotalStock = totalStock;
            ViewBag.ProductCount = productCount;

          
            var userId = HttpContext.Session.GetInt32("UserID");

            if (userId != null)
            {
              
                var user = context.Users.FirstOrDefault(u => u.UserID == userId.Value);

               
                ViewBag.UserName = user?.UserName;
            }
            else
            {
            
                ViewBag.UserName = "Ziyaretçi";
            }

            
            return View(products);
        }



        [HttpGet("Categories")]
     public async Task<IActionResult> Categories()
        {
            var categories = await context.Categories.ToListAsync();
            return View(categories);
        }

        [HttpGet]
        public IActionResult CategoryCreate() {
            return View();
        }

        [HttpPost("Category/Create")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> CategoryCreate([Bind("CategoryName")] Category category)
        {
            var roleID = HttpContext.Session.GetInt32("RoleID");
            if (roleID == 1)
            {
                context.Add(category);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Categories));

              
            }
            else if(roleID == 2)
            {
                return Forbid();
            }
            else
            {
                return NotFound();
            }
            

        }
        [HttpGet]
        public async Task<IActionResult> CategoryEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var category = await context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> CategoryEdit(int id, [Bind("CategoryID, CategoryName")] Category category)
        {
            var roleID = HttpContext.Session.GetInt32("RoleID");
            if (roleID == 1)
            {
                if (id != category.CategoryID)
                {
                    return NotFound();
                }

                context.Update(category);
                await context.SaveChangesAsync();

                return RedirectToAction(nameof(Categories));

            }
            else if(roleID == 2)
            {
                return Forbid();
            }
            else
            {
                return NotFound();
            }
            
        }
        
       
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult>CategoryDelete(int id)
        {
            var roleID = HttpContext.Session.GetInt32("RoleID");
            if (roleID == 1)
            {
                var category = await context.Categories.FindAsync(id);
                context.Categories.Remove(category);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Categories));
            }
            else if (roleID == 2)
            {
                return Forbid();
            }
            else
            {
                return NotFound();
            }

        }
       

        public async Task<IActionResult> Suppliers()
        {
            var suppliers = await context.Suppliers.ToListAsync();
            return View(suppliers);
        }
        [HttpGet]
        public  IActionResult SupplierCreate()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
     
        public async Task<IActionResult> SupplierCreate([Bind("SupplierName","ContactInfo")] Supplier supplier)
        {
            var roleID = HttpContext.Session.GetInt32("RoleID");

            if (roleID == 1)
            {
                context.Add(supplier);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Suppliers));
            }
            else if (roleID == 2)
            {
                return Forbid();
            }
            else
            {
                return NotFound();
            }
           
        }
        [HttpGet] 
        public async Task<IActionResult> SupplierEdit(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }
            var suppliers = await context.Suppliers.FirstOrDefaultAsync(s => s.SupplierID == id);
            if (suppliers == null)
            {
                return NotFound();
            }
            return View(suppliers);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> SupplierEdit(int id, [Bind("SupplierID, SupplierName")] Supplier supplier)
        {
            var roleID = HttpContext.Session.GetInt32("RoleID");
            if (roleID == 1)
            {
                if (id != supplier.SupplierID)
                {
                    return NotFound();
                }


                context.Update(supplier);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Suppliers));

               
            }
            else if (roleID == 2)
            {
                return Forbid();
            }
            else
            {
                return NotFound();
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
      
        public async Task<IActionResult> SupplierDelete(int? id)
        {
            var roleID = HttpContext.Session.GetInt32("RoleID");
            if (roleID == 1)
            {
                var supplier = await context.Suppliers.FindAsync(id);
                context.Suppliers.Remove(supplier);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Supplier));
            }
            else if (roleID == 2)
            {
                return Forbid();
            }
            else
            {
                return NotFound();
            }
           
        }

        public async Task<IActionResult> Users()
        {

            var users = await context.Users.Include(u => u.Role).ToListAsync();
            return View(users);
        }
        [HttpGet]
        public IActionResult UserCreate()
        {
            ViewBag.Roles = context.Roles.ToList(); 
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
 
        public async Task<IActionResult> UserCreate([Bind("UserName,Password,RoleID")]Users users)
        {
            var roleID = HttpContext.Session.GetInt32("RoleID");
            if (roleID == 1)
            {
                context.Add(users);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Users));

                ViewBag.Roles = context.Roles.ToListAsync();
                return View(users);

            }
            else if (roleID == 2)
            {
                return Forbid();
            }
            else
            {
                return NotFound();
            }

            
        }
        [HttpGet]
        public async Task<IActionResult> UserEdit(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }
            var user = await context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var roles = await context.Roles.ToListAsync();
            ViewBag.Roles = roles;

            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public async Task<IActionResult> UserEdit(int id, [Bind("UserID,UserName,Password,RoleID")] Users users)
        {
            var roleID = HttpContext.Session.GetInt32("RoleID");
            if (roleID == 1)
            {
                if (id != users.UserID)
                {
                    return NotFound();
                }

                context.Update(users);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Users));

                ViewBag.Roles = context.Roles.ToList();
                return View(users);
            }
            else if (roleID == 2)
            {
                return Forbid();
            }
            else
            {
                return NotFound();
            }
            
        }
        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
       
        public async Task<IActionResult> UserDelete(int id)
        {
            var roleID = HttpContext.Session.GetInt32("RoleID");
            if (roleID == 1)
            {
                var user = await context.Users.FindAsync(id);
                context.Users.Remove(user);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Users));
            }
            else if (roleID == 2)
            {
                 return Forbid();
            }
            else
            {
                return NotFound();
            }

        }
      

        public async Task<IActionResult> Roles()
        {
            var roles = await context.Roles.ToListAsync();
            return View(roles);
        }
        [HttpGet]
        public IActionResult RoleCreate()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> RoleCreate([Bind("RoleName")] Role role)
        {
            var roleID = HttpContext.Session.GetInt32("RoleID");
            if (roleID == 1)
            {
                context.Add(role);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Roles));

                return View(role);
            }
            else if (roleID == 2)
            {
                return Forbid();
            }
            else
            {
                return NotFound();
            }
            
        }
        [HttpGet]
        public async Task<IActionResult> RoleEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
     
        public async Task<IActionResult> RoleEdit(int id, [Bind("RoleID,RoleName")] Role role)
        {
            var roleID = HttpContext.Session.GetInt32("RoleID");
            if (roleID == 1)
            {
                if (id != role.RoleID)
                {
                    return NotFound();
                }




                context.Update(role);
                await context.SaveChangesAsync();


                return RedirectToAction(nameof(Roles));

                
            }
            else if (roleID == 2)
            {
                return Forbid();
            }
            else
            {
                return NotFound();
            }
          
        }
        [HttpPost, ActionName("RoleDelete")]
        [ValidateAntiForgeryToken]
  
        public async Task<IActionResult> RoleDelete(int id)
        {
            var roleID = HttpContext.Session.GetInt32("RoleID");
            if (roleID == 1)
            {
                var role = await context.Roles.FindAsync(id);
                if (role != null)
                {
                    context.Roles.Remove(role);
                    await context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Roles));
            }
            else if (roleID == 2)
            {
                return Forbid();
            }
            else
            {
                return NotFound();
            }
        
        }
       
       
        [HttpGet("Orders")]
        public async Task<IActionResult> Orders(int? customerID, int? statusID, DateTime? startDate, DateTime? endDate)
        {
            var orders = await context.Orders
    .Include(o => o.Shipment)
    .ThenInclude(s => s.ShippingCompany)
    .ToListAsync();
            var shippingRates = context.ShippingRates.ToList();
            Console.WriteLine($"customerID: {customerID}");
            Console.WriteLine($"statusID: {statusID}");
            Console.WriteLine($"startDate: {startDate}");
            Console.WriteLine($"endDate: {endDate}");
           
            var orderProducts = await context.OrderDetails
                .Include(od => od.Order)
                .Include(od => od.Product)

                .Select(od => new OrderProductSummaryViewModel
                {
                    ProductName = od.Product.ProductName,
                    Quantity = od.Quantity,
                    OrderDate = od.Order.OrderDate,
                }).ToListAsync();

            
            var ordersQuery = context.Orders.AsQueryable();
            int? currentRoleID = HttpContext.Session.GetInt32("RoleID");
            ViewBag.CurrentRoleID = currentRoleID;

            if (customerID.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.CustomerID == customerID.Value);
            }
            if (statusID.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.StatusID == statusID.Value);
            }
            if (startDate.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.OrderDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.OrderDate <= endDate.Value);
            }

            var filteredOrders = await ordersQuery
                .Include(o => o.Status)
                .Include(o => o.OrderDetails)

                .ThenInclude(od => od.Product)
                .ToListAsync();

            var viewModel = new OrderFilterViewModel
            {
                Orders = filteredOrders,
                Status = await context.Statuses.ToListAsync(),
                OrderProductSummary = orderProducts, // Bu ekleme gerekebilir
                ShippingRates = shippingRates,
               
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderProductSummaryData()
        {
            var orderProducts = await context.OrderDetails
           .Include(od => od.Order)
           .Include(od => od.Product)
           .Select(od => new OrderProductSummaryViewModel
           {
               ProductName = od.Product.ProductName,
               Quantity = od.Quantity,
               OrderDate = od.Order.OrderDate
           })
           .ToListAsync();

            return Json(orderProducts);
        }

        [HttpGet]
        public IActionResult OrderCreate()
        {
            ViewBag.ShippingCompanies = context.ShippingCompanies.ToList();

            var viewModel = new OrderCreateViewModel
            {
                Order = new Models.Order(),
                AvailableProducts = context.Products.ToList(),
                ShippingCompanies = context.ShippingCompanies.ToList()
            };

            return View(viewModel);
        }

        [HttpPost("OrderCreate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OrderCreate(OrderCreateViewModel viewModel, string selectedQuantities, string selectedProductIDs)
        {
            ViewBag.ShippingCompanies = context.ShippingCompanies.ToList();

            var roleID = HttpContext.Session.GetInt32("RoleID");

            if (roleID == 1)
            {
                if (string.IsNullOrEmpty(selectedProductIDs) || string.IsNullOrEmpty(selectedQuantities))
                {
                    ModelState.AddModelError("", "Ürünler ve miktarlarý eksik veya hatalý.");
                    viewModel.AvailableProducts = context.Products.ToList();
                    return View(viewModel);
                }

                var productIDs = selectedProductIDs.Split(',').Select(int.Parse).ToList();
                var quantities = selectedQuantities.Split(',').Select(int.Parse).ToList();

                if (productIDs.Count == 0 || quantities.Count == 0 || productIDs.Count != quantities.Count)
                {
                    ModelState.AddModelError("", "Ürünler ve miktarlarý eksik veya hatalý.");
                    viewModel.AvailableProducts = context.Products.ToList();
                    return View(viewModel);
                }

                // Sipariþi ekle ve kaydet
                viewModel.Order.StatusID = 1; // Baþlangýçta durum 1 olarak ayarlanýr
                context.Orders.Add(viewModel.Order);
                await context.SaveChangesAsync();

                // Sipariþ numarasýný oluþtur
                var orderNumber = new OrderNumber
                {
                    OrderID = viewModel.Order.OrderID,
                    GeneratedOrderNumber = GenerateOrderNumber(),
                    CreatedAt = DateTime.Now,
                    OrderNumberValue = GenerateOrderNumber()
                };

                // Sipariþ numarasýný ekle ve güncelle
                context.OrderNumbers.Add(orderNumber);
                await context.SaveChangesAsync();

                viewModel.Order.OrderNumberID = orderNumber.OrderNumberID;
                viewModel.Order.OrderNumberValue = orderNumber.OrderNumberValue;
                context.Orders.Update(viewModel.Order);
                await context.SaveChangesAsync(); // Güncellemeyi kaydedin

                // Sipariþ detaylarýný ekle
                foreach (var productID in productIDs)
                {
                    var quantity = quantities[productIDs.IndexOf(productID)];
                    var product = await context.Products.FindAsync(productID);

                    if (product != null && product.QuantityInStock >= quantity)
                    {
                        var orderDetails = new OrderDetails
                        {
                            OrderID = viewModel.Order.OrderID,
                            ProductID = productID,
                            StatusID = 1,  // Sipariþ oluþturulurken baþlangýç durumu
                            Quantity = quantity,
                            Price = product.Price
                        };

                        product.QuantityInStock -= quantity;
                        context.OrderDetails.Add(orderDetails);
                    }
                    else
                    {
                        ModelState.AddModelError("", $"Ürün stoðu yetersiz: {product?.ProductName}");
                        viewModel.AvailableProducts = context.Products.ToList();
                        return View(viewModel);
                    }
                }

                // ShippingCost Hesaplama
                var shippingRate = await context.ShippingRates.FirstOrDefaultAsync(sr => sr.ShippingCompanyID == viewModel.Order.ShippingCompanyID);
                if (shippingRate != null)
                {
                    int productCount = productIDs.Count;
                    decimal shippingCost = (decimal)shippingRate.Rate * productCount;
                    viewModel.Order.TotalCost = shippingCost;
                    context.Orders.Update(viewModel.Order); // Update iþlemi kullanýn
                    await context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Orders));
            }
            else if (roleID == 2)
            {
                return Forbid();
            }
            else
            {
                return NotFound();
            }
        }







        private string GenerateShipmentNumber()
        {
            return "SN" + DateTime.Now.Ticks;
        }

        private string GenerateOrderNumber(int length)
        {
            // Farklý parametreler için bir metod tanýmý
            return "ORD" + new Random().Next(1000, 9999).ToString();
        }














        [HttpGet]
        public async Task<IActionResult> OrderEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewBag.Statuses = context.Statuses.ToList();
            var order = await context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }
        [HttpPost, ActionName("OrderEdit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OrderEdit(int id, [Bind("OrderID,OrderDate,CustomerID,StatusID")] Models.Order order)
         {
            var roleID = HttpContext.Session.GetInt32("RoleID");
            if (roleID == 1)
            {
                if (id != order.OrderID)
                {
                    return NotFound();
                }



                context.Update(order);
                await context.SaveChangesAsync();


                return RedirectToAction(nameof(Orders));

                return View(order);
            }
            else if (roleID == 2)
            {
                return Forbid();
            }
            else
            {
                return NotFound();
            }
            
        }
        [HttpPost, ActionName("OrderDelete")]
        [ValidateAntiForgeryToken]
     
        public async Task<IActionResult> OrderDelete(int id)
        {
            var roleID = HttpContext.Session.GetInt32("RoleID");
            if (roleID==1)
            {
                var order = await context.Orders.FindAsync(id);
                if (order != null)
                {
                    context.Orders.Remove(order);
                    await context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Orders));
            }
            else if (roleID == 2)
            {
                return Forbid();
            }
            else
            {
                return NotFound();
            }
           
        }
        public async Task<IActionResult> OrderDetails()
        {
            var orderDetails = await context.OrderDetails
                .Include(od => od.Order)
                .Include(od => od.Product)
                .ToListAsync();
            return View(orderDetails);
        }
        [HttpGet]
        public async Task<IActionResult> OrderDetailsDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var orderDetails = await context.OrderDetails
                .Include(od => od.Order)
                .Include(od => od.Product)
                .FirstOrDefaultAsync(m => m.OrderDetailsID == id);

            if (orderDetails == null)
            {
                return NotFound();
            }

            return View(orderDetails);
        }
        public async Task<IActionResult> Products()
        {
            var products = await context.Products
                .Include(p => p.Category) 
                .Include(p => p.Supplier) 
                .ToListAsync();
            return View(products);
        }
        [HttpGet]
        public IActionResult ProductCreate()
        {
            ViewBag.Categories = context.Categories.ToList();
            ViewBag.Suppliers = context.Suppliers.ToList();
            
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
     
        public async Task<IActionResult> ProductCreate([Bind("ProductName,Description,QuantityInStock,Price,CategoryID,SupplierID")] Product product)
        {
            var roleID = HttpContext.Session.GetInt32("RoleID");

            if (roleID == 1)
            {
                context.Add(product);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Products));

               
                return View(product);
            }
            else if (roleID == 2)
            {
                return Forbid();
            }
            else
            {
                return NotFound();
            }
        }
           
                
        [HttpGet]
        public async Task<IActionResult> ProductEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = await context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.ProductID == id);

            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Categories = await context.Categories.ToListAsync();
            ViewBag.Suppliers = await context.Suppliers.ToListAsync();
            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductEdit(int id, [Bind("ProductID,ProductName,Description,QuantityInStock,Price,CategoryID,SupplierID")] Product product)
        {
            var roleID = HttpContext.Session.GetInt32("RoleID");
            if (roleID == 1)
            {
                if (id != product.ProductID)
                {
                    return NotFound();
                }




                context.Update(product);
                await context.SaveChangesAsync();

                return RedirectToAction(nameof(Products));

               
                
               
            }
            else if (roleID == 2)
            {
                return Forbid();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost, ActionName("ProductDelete")]
        [ValidateAntiForgeryToken]
       
        public async Task<IActionResult> ProductDelete(int id)
        {
            var roleID = HttpContext.Session.GetInt32("RoleID");
            if (roleID == 1)
            {
                var product = await context.Products
                                .Include(p => p.Category)
                                .Include(p => p.Supplier)
                                .FirstOrDefaultAsync(m => m.ProductID == id);

                if (product != null)
                {
                    context.Products.Remove(product);
                    await context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Products));
            }
            else if (roleID == 2)
            {
                return Forbid();
            }
            else 
            {
                return NotFound(); 
            }
        }

        [HttpGet, HttpPost]
        public IActionResult ProcessOrder(int id)
        {
         
            var order = context.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }

          
            if (order.StatusID < 5)
            {
                order.StatusID++;
                context.SaveChanges();
            }

           
            return RedirectToAction("Orders");
        }
    
        public List<Product> CheckStockLevels()
        {
            int lowStock = 10;
            var lowStockProducts = context.Products.Where(p => p.QuantityInStock < lowStock).ToList();

            foreach (var product in lowStockProducts)
            {
                NotifyLowStock(product);
            }
            return lowStockProducts;
        }
        public void NotifyLowStock(Product product)
        {
            
            string message = $"{product.ProductName} ürünü için stok seviyesi düþük. Mevcut Stok: {product.QuantityInStock}";
            _notificationService.SendNotification("Düþük Stok Uyarýsý", message);
        }
       
        public string GenerateOrderNumber()
        {
            var timestamp = DateTime.Now.ToString("yyMMddHHmmss");
            var random = new Random().Next(1000, 9999);
            return $"{timestamp}{random}";
        }
        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(string orderNumberValue, int orderID, int newStatusID)
        {
            var orderid = context.Orders.FirstOrDefault(o => o.OrderNumber.OrderNumberValue == orderNumberValue).OrderID;
            orderID = orderid;
            var order = context.Orders
                .Include(o => o.Shipment)
                .Include(o => o.OrderNumber)
                .FirstOrDefault(o => o.OrderNumber.OrderNumberValue == orderNumberValue && o.OrderID == orderID);

            if (order == null)
            {
                return NotFound();
            }

            // Determine the new status
            if (order.StatusID == 1)
            {
                order.StatusID = 2;
            }
            else if ( order.StatusID == 2)
            {
                order.StatusID = 3;

                // Create a new shipment if the status is being updated to 3
               
            }
            else if (order.StatusID == 3)
            {
                order.StatusID = 4;
            }
           
            else
            {
                // Invalid status transition
                ModelState.AddModelError("", "Geçersiz durum geçiþi.");
                return View(); // Redirect to an error page or view
            }
            if (order.StatusID == 3)
            {
                newStatusID = order.StatusID;
                var shipment = new Shipment
                {
                    OrderID = order.OrderID,
                    ShipmentNumber = GenerateShipmentNumber(),
                    ShipmentDate = DateTime.UtcNow,
                    ShippingCompanyID = order.ShippingCompanyID,
                    StatusID = newStatusID
                };
                context.Shipments.Add(shipment);
            }
       
           
            

            // Save changes to the database
            await context.SaveChangesAsync();

            return RedirectToAction("Orders");
        }

        public IActionResult GetShippingCost(int shippingCompanyID, int productCount)
        {
            var rate = context.ShippingRates
                .Where(r => r.ShippingCompanyID == shippingCompanyID)
                .FirstOrDefault();

            if (rate == null)
            {
                return Json(new { shippingCost = 0 });
            }

            // Assuming rate.Rate is the base cost and it increases per product
            decimal shippingCost = (decimal)rate.Rate * (decimal)productCount;

            return Json(new { shippingCost = shippingCost });
        }




        public async Task<IActionResult> ShipmentDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shipment = await context.Shipments
                .Include(s => s.Order)
                .Include(s => s.ShippingCompany)
                .FirstOrDefaultAsync(m => m.ShipmentID == id);
            if (shipment == null)
            {
                return NotFound();
            }

            return View(shipment);
        }

        // GET: Shipment/Create
        public IActionResult ShipmentCreate()
        {
            ViewBag.Orders = context.Orders.ToList();
            ViewBag.ShippingCompanies = context.ShippingCompanies.ToList();
            return View();
        }

        // POST: Shipment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderID,ShipmentNumber,ShipmentDate,ShippingCompanyID")] Shipment shipment)
        {
            if (ModelState.IsValid)
            {
                context.Add(shipment);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Orders = context.Orders.ToList();
            ViewBag.ShippingCompanies = context.ShippingCompanies.ToList();
            return View(shipment);
        }

        // GET: Shipment/Edit/5
        public async Task<IActionResult> ShipmentEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shipment = await context.Shipments.FindAsync(id);
            if (shipment == null)
            {
                return NotFound();
            }

            ViewBag.Orders = context.Orders.ToList();
            ViewBag.ShippingCompanies = context.ShippingCompanies.ToList();
            return View(shipment);
        }

        // POST: Shipment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShipmentEdit(int id, [Bind("ShipmentId,OrderID,ShipmentNumber,ShipmentDate,ShippingCompanyID")] Shipment shipment)
        {
            if (id != shipment.ShipmentID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    context.Update(shipment);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShipmentExists(shipment.ShipmentID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Orders = context.Orders.ToList();
            ViewBag.ShippingCompanies = context.ShippingCompanies.ToList();
            return View(shipment);
        }

        // GET: Shipment/Delete/5
        public async Task<IActionResult> ShipmentDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shipment = await context.Shipments
                .Include(s => s.Order)
                .Include(s => s.ShippingCompany)
                .FirstOrDefaultAsync(m => m.ShipmentID == id);
            if (shipment == null)
            {
                return NotFound();
            }

            return View(shipment);
        }

        // POST: Shipment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShipmentDelete(int id)
        {
            var shipment = await context.Shipments.FindAsync(id);
           context.Shipments.Remove(shipment);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShipmentExists(int id)
        {
            return context.Shipments.Any(e => e.ShipmentID == id);
        }

    }
}
















