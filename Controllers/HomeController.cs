using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MvcLaptop.Models;
using MvcLaptop.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
namespace MvcLaptop.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger; //Dependency Injection

    private readonly MvcLaptopContext _context; //Dependency Injection

    public HomeController(MvcLaptopContext context, ILogger<HomeController> logger)
    {
        _context = context;
        _logger = logger;
    }
    public async Task<IActionResult> Index(string genre, string searchString)
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        ViewData["Genres"] = _context.Laptop
                .Select(l => l.Genre)
                .Distinct()
                .OrderBy(g => g)
                .ToList();
        var laptops = string.IsNullOrEmpty(genre)
            ? _context.Laptop.AsQueryable() // Nếu không có genre, trả về tất cả sản phẩm
            : _context.Laptop.Where(l => l.Genre == genre); // Lọc theo genre

        if (!string.IsNullOrEmpty(searchString))
        {
            laptops = laptops.Where(l => l.Title!.ToUpper().Contains(searchString.ToUpper()));
        }
        ViewData["CurrentGenre"] = genre;
        return View(laptops);
    }
    public async Task<IActionResult> Product(string genre, string searchString)
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        ViewData["Genres"] = _context.Laptop
            .Select(l => l.Genre)
            .Distinct()
            .OrderBy(g => g)
            .ToList();
        // Lọc sản phẩm theo Genre
        var laptops = string.IsNullOrEmpty(genre)
            ? _context.Laptop.AsQueryable() // Nếu không có genre, trả về tất cả sản phẩm
            : _context.Laptop.Where(l => l.Genre == genre); // Lọc theo genre

        if (!string.IsNullOrEmpty(searchString))
        {
            laptops = laptops.Where(l => l.Title!.ToUpper().Contains(searchString.ToUpper()));
        }
        ViewData["CurrentGenre"] = genre;
        ViewData["SearchString"] = searchString;
        return View(await laptops.ToListAsync());
    }

    public IActionResult Privacy()
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        return View();
    }
    public IActionResult Details(int id)
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        var laptop = _context.Laptop.FirstOrDefault(l => l.Id == id);
        if (laptop == null)
        {
            return NotFound();
        }
        // Truyền danh sách Genres vào ViewData
        ViewData["Genres"] = _context.Laptop
            .Select(l => l.Genre)
            .Distinct()
            .OrderBy(g => g)
            .ToList();

        return View(laptop);
    }
    // Action trả về danh sách Genre
    public IActionResult PartialGenres()
    {
        var genres = _context.Laptop
            .Select(l => l.Genre)
            .Distinct()
            .OrderBy(g => g)
            .ToList();

        return PartialView("_GenreMenu", genres);
    }
    public IActionResult Login()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string userName, string password)
    {
        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
        {
            ModelState.AddModelError(string.Empty, "Username and password are required.");
            return View();
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName && u.Password == password);

        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return View();
        }

        // Lưu thông tin người dùng vào session
        HttpContext.Session.SetString("UserName", user.UserName);

        return RedirectToAction(nameof(Index));
    }
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction(nameof(Login));
    }
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register([Bind("UserName,Email,Password")] User user)
    {
        if (ModelState.IsValid)
        {
            // Kiểm tra xem tên người dùng đã tồn tại trong cơ sở dữ liệu chưa
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == user.UserName);
            if (existingUser != null)
            {
                // Nếu tên người dùng đã tồn tại, thêm lỗi vào ModelState
                ModelState.AddModelError("UserName", "Tên tài khoản này đã tồn tại. Vui lòng chọn tên khác.");
                return View(user);  // Trả về view đăng ký với thông báo lỗi
            }
            // Nếu tên người dùng chưa tồn tại, thêm người dùng mới vào cơ sở dữ liệu
            _context.Add(user);
            await _context.SaveChangesAsync();
            // Đăng ký thành công, chuyển hướng tới trang đăng nhập
            return RedirectToAction(nameof(Login));
        }
        return View(user);
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
