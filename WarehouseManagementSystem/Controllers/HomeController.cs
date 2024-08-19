using ClosedXML.Excel;
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

        [HttpGet]
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
                .Include(o => o.Statuses)
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
                        worksheet.Cells[row, 3].Value = order.Statuses.StatusName;
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



        [HttpGet]
     public async Task<IActionResult> Categories()
        {
            var categories = await context.Categories.ToListAsync();
            return View(categories);
        }

        [HttpGet]
        public IActionResult CategoryCreate() {
            return View();
        }

        [HttpPost]
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

                return View(category);
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
            var suppliers = context.Suppliers.ToListAsync();
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
                return RedirectToAction(nameof(Supplier));

                return View(supplier);
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
            ViewBag.Roles = context.Roles.ToListAsync();
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
        public async Task<IActionResult> Orders(int? customerID, int? statusID, DateTime? startDate, DateTime? endDate)
        {

            var orderProducts = await context.OrderDetails
                .Include(od => od.Order)
                .Include(od=> od.Product)
                .Select(od=> new OrderProductSummaryViewModel
            {
                ProductName = od.Product.ProductName,
                Quantity= od.Quantity,
                OrderDate= od.Order.OrderDate,
            }).ToListAsync();
            // Sipariþler üzerinde filtreleme iþlemi yapýlacak
       
            var ordersQuery = context.Orders.AsQueryable();
            int? currentRoleID = HttpContext.Session.GetInt32("RoleID");
            ViewData["CurrentRoleID"] = currentRoleID;
            // Filtreleme iþlemleri
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

            // Filtrelenmiþ sipariþlerin yüklenmesi
            var filteredOrders = await ordersQuery
                .Include(o => o.Statuses)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .ToListAsync();

            // ViewModel oluþturulmasý
            var viewModel = new OrderFilterViewModel
            {
               
                Orders = filteredOrders,
                Statuses = await context.Statuses.ToListAsync(),
                // Diðer gerekli veriler buraya eklenebilir
            };

            // Sayfanýn gönderilmesi
            return View(viewModel); // `View(viewModel, "_OrderList")` yerine `View(viewModel)` kullanýmý, `_OrderList` isimli bir partial view kullanýmý varsa ona uygun deðiþiklikler yapýlabilir
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
            var viewModel = new OrderCreateViewModel
            {
                Order = new Order(),
                AvailableProducts = context.Products.ToList()
            };
            var products = context.Products.ToList();
            //var statuses = context.Statuses.ToList();
            //ViewBag.Statuses = statuses;
            ViewBag.Products = products;
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]


        public async Task<IActionResult> OrderCreate(OrderCreateViewModel viewModel, string selectedQuantities, string selectedProductIDs)
        {
            Console.WriteLine($"SelectedProductIDs: {selectedProductIDs}");
            Console.WriteLine($"SelectedQuantities: {selectedQuantities}");
          
            viewModel.Order.StatusID = 1;
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

                // Yeni sipariþi ekle
                context.Orders.Add(viewModel.Order);
                await context.SaveChangesAsync(); // Sipariþin ID'sini almak için

                // Sipariþ numarasý oluþtur
                var orderNumber = new OrderNumber
                {
                    OrderID = viewModel.Order.OrderID,
                    GeneratedOrderNumber = GenerateOrderNumber(),  // Parametresiz çaðrým
                    CreatedAt = DateTime.Now,
                    OrderNumberValue = GenerateOrderNumber()  // Parametresiz çaðrým
                };
               

                // Sipariþ numarasýný ekle
                context.OrderNumbers.Add(orderNumber);
                
                
                await context.SaveChangesAsync(); // Sipariþ numarasýnýn ID'sini almak için

                // Sipariþ numarasýný sipariþe iliþkilendir
                viewModel.Order.OrderNumberID = orderNumber.OrderNumberID;
                context.Orders.Update(viewModel.Order);
                await context.SaveChangesAsync();
                var order = context.Orders.Find(viewModel.Order.OrderID);
                order.OrderNumberID = orderNumber.OrderNumberID;
                order.OrderNumberValue = orderNumber.OrderNumberValue;
                context.Orders.Update(order);
                await context.SaveChangesAsync();

                // Sipariþ detaylarýný ekle
                for (int i = 0; i < productIDs.Count; i++)
                {
                    var productID = productIDs[i];
                    var quantity = quantities[i];

                    var orderDetails = new OrderDetails
                    {
                        OrderID = viewModel.Order.OrderID,
                        ProductID = productID,
                        Quantity = quantity,
                        Price = (await context.Products.FindAsync(productID))?.Price ?? 0
                    };

                    var product = await context.Products.FindAsync(productID);
                    if (product != null && product.QuantityInStock >= quantity)
                    {
                        product.QuantityInStock -= quantity;
                        context.OrderDetails.Add(orderDetails);
                        await context.SaveChangesAsync();
                    }
                    else
                    {
                        ModelState.AddModelError("", $"Ürün stoðu yetersiz: {product?.ProductName}");
                        viewModel.AvailableProducts = context.Products.ToList();
                        return View(viewModel);
                    }
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
        public async Task<IActionResult> OrderEdit(int id, [Bind("OrderID,OrderDate,CustomerID,StatusID")] Order order)
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

            ViewBag.Categories = context.Categories.ToListAsync();
            ViewBag.Suppliers = context.Suppliers.ToListAsync();
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
        public IActionResult UpdateOrderStatus(string orderNumberValue)
        {
           
            var order = context.Orders
                .Include(o => o.OrderNumber)
                .FirstOrDefault(o => o.OrderNumber.OrderNumberValue == orderNumberValue);

            if (order == null)
            {
               
                return NotFound();
            }

            
            if (order.StatusID == 1)
            {
                order.StatusID = 2; 
            }
            else if (order.StatusID == 2)
            {
                order.StatusID = 3; 
            }
            else if (order.StatusID == 3)
            {
                order.StatusID = 4; 
            }
            else if (order.StatusID == 4)
            {
                order.StatusID = 5; 
            }
            

           
            context.SaveChanges();

            
            return RedirectToAction("Orders");
        }



    }
}
















