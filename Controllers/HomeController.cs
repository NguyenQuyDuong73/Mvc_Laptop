using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MvcLaptop.Models;
using MvcLaptop.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
namespace MvcLaptop.Controllers;
using Microsoft.AspNetCore.Identity;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger; //Dependency Injection

    private readonly MvcLaptopContext _context; //Dependency Injection
    private readonly UserManager<User> _userManager;
    public HomeController(MvcLaptopContext context, ILogger<HomeController> logger, UserManager<User> userManager)
    {
        _context = context;
        _logger = logger;
        _userManager = userManager;
    }
    public async Task<IActionResult> Index(int? categoryId, string searchString)
    {     
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            Console.WriteLine("Người dùng chưa đăng nhập. Claims sau khi xóa:");
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"Claim tồn tại: {claim.Type} - {claim.Value}");
            }
        }
        else
        {
            Console.WriteLine("Người dùng đã đăng nhập. Claims hợp lệ:");
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"Claim: {claim.Type} - {claim.Value}");
            }
        }
        await SetUserRolesViewDataAsync();
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        ViewData["Categories"] = _context.Category!
            .OrderBy(c => c.Name_Category)
            .ToList();
        var products = _context.Product
         .Include(p => p.ProductImages)
         .Include(p => p.Category).AsQueryable();
        if (categoryId.HasValue)
        {
            products = products.Where(p => p.CategoryId == categoryId.Value);
        }

        if (!string.IsNullOrEmpty(searchString))
        {
            products = products.Where(p => p.Title!.ToUpper().Contains(searchString.ToUpper()));
        }

        ViewData["CurrentCategory"] = categoryId;
        ViewData["SearchString"] = searchString;
        return View(await products.ToListAsync());
    }
    public async Task<IActionResult> Product(int? categoryId, string searchString)
    {
        await SetUserRolesViewDataAsync();
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        ViewData["Categories"] = _context.Category!
            .OrderBy(c => c.Name_Category)
            .ToList();
        var products = _context.Product
        .Include(p => p.ProductImages)
        .Include(p => p.Category)
        .AsQueryable();

        if (categoryId.HasValue)
        {
            products = products.Where(p => p.CategoryId == categoryId.Value);
        }

        if (!string.IsNullOrEmpty(searchString))
        {
            products = products.Where(p => p.Title!.ToUpper().Contains(searchString.ToUpper()));
        }

        ViewData["CurrentCategory"] = categoryId;
        ViewData["SearchString"] = searchString;

        return View(await products.ToListAsync());
    }

    public IActionResult Privacy()
    {
        _ = SetUserRolesViewDataAsync();
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        return View();
    }
    public IActionResult Details(int id)
    {
        _ = SetUserRolesViewDataAsync();
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        var laptop = _context.Product
            .Include(l => l.ProductImages) // Nạp danh sách ảnh
            .Include(l => l.Category) // Nạp danh mục
            .FirstOrDefault(l => l.Id == id);
        if (laptop == null)
        {
            return NotFound();
        }
        // Truyền danh sách Genres vào ViewData
        ViewData["Categories"] = _context.Category!
            .OrderBy(c => c.Name_Category)
            .ToList();
        return View(laptop);
    }
    public IActionResult About()
    {
        _ = SetUserRolesViewDataAsync();
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        return View();
    }
    // Action trả về danh sách Genre
    public IActionResult PartialGenres()
    {
        var categories = _context.Category!
            .OrderBy(c => c.Name_Category)
            .ToList();

        return PartialView("_CategoryMenu", categories);
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    private async Task SetUserRolesViewDataAsync()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                ViewData["UserRoles"] = roles;
            }
        }
        else
        {
            ViewData["UserRoles"] = new List<string>();
        }
    }

}
