using Microsoft.AspNetCore.Mvc;
using WarehouseManagementSystem.Data;
using Microsoft.EntityFrameworkCore;
using WarehouseManagementSystem.Models;
using System.Security.Claims;
namespace WarehouseManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext context;

        public AccountController(ApplicationDbContext context)
        {
            this.context = context;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string UserName, string Password)
        {
           
            var user = await context.Users
                 .Include(u => u.Role)
                 .FirstOrDefaultAsync(u => u.UserName == UserName && u.Password == Password);
            if (user != null)
            {
               
                HttpContext.Session.SetInt32("UserID", user.UserID);
                HttpContext.Session.SetInt32("RoleID", user.RoleID);
                //Oturum Açıldı
                Console.WriteLine("User found: " + UserName);
                //var roleID = HttpContext.Session.GetInt32("RoleID");
                return RedirectToAction("Index", "Home");
            }
            
            
                Console.WriteLine("Login failed for user: " + UserName);
                ViewBag.Message = "Invalid login attempt.";
            
           
            return View();
        }


        //[HttpGet("GetUserDetails")]
        //public IActionResult GetUserDetails()
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    if (userId == null)
        //    {
        //        return Unauthorized();
        //    }
        //    var user = context.Users.FirstOrDefault(u => u.UserID.ToString() == userId);
        //    if (user == null)
        //    {
        //        return NotFound("Kullanıcı bulunamadı.");
        //    }
        //    return Ok(user);
        //}






        public IActionResult AccessDenied()
        {
            {
               string userRole = User.IsInRole("Yönetici") ? "Yönetici" : "Çalışan";
               ViewData["Message"] = userRole == "Yönetici"
                    ? "Yönetici olarak bu sayfaya erişim izniniz var, ancak yetkiniz olmayan bir işlem gerçekleştirmeye çalıştınız."
                    : "Bu sayfayı görüntülemek için yeterli izniniz yok.";
                return View();
            }
        }

    }
}
