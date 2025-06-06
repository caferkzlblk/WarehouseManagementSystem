namespace NewFolder1;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WarehouseManagementSystem.Data; // DbContext namespace'in
using Microsoft.AspNetCore.Http;
public class UserNameActionFilter : IActionFilter
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserNameActionFilter(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var userId = _httpContextAccessor.HttpContext.Session.GetInt32("UserID");
        string userName = "Ziyaretçi";

        if (userId.HasValue)
        {
            var user = _context.Users.Find(userId.Value);
            if (user != null)
                userName = user.UserName;
        }

        var controller = context.Controller as Controller;
        if (controller != null)
        {
            controller.ViewBag.UserName = userName;
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Burada bir işlem yapmana gerek yok
    }
}
