using Microsoft.AspNetCore.Mvc;
using WarehouseManagementSystem.Data;

namespace WarehouseManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public ProductController(ApplicationDbContext context)
        {
           this.context = context;
        }

        [HttpGet("ProductStockData")]
        public IActionResult GetProductStockData()
        {
            var products = context.Products.ToList();
            var producData = products.Select(p => new
            {
                p.ProductName,
                p.QuantityInStock
            }).ToList();
            

            return Ok(producData);
        }

    }
}

