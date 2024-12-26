using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MvcLaptop.Models;
using MvcLaptop.Data;

namespace MvcLaptop.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly MvcLaptopContext _context;
    public HomeController(MvcLaptopContext context,ILogger<HomeController> logger)
    {
        _context = context;
        _logger = logger;
    }
    public IActionResult Index()
    {
        var laptops = _context.Laptop.ToList();
        return View(laptops);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
