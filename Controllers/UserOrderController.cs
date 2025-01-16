using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcLaptop.Data;
using MvcLaptop.Models;

namespace MvcLaptop.Controllers
{
    [Authorize]
    public class UserOrderController : Controller
    {
        private readonly MvcLaptopContext _context;

        public UserOrderController(MvcLaptopContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách đơn hàng của người dùng
        public async Task<IActionResult> Index()
        {
            // Lấy UserId của người dùng đang đăng nhập
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                TempData["Error"] = "Bạn cần đăng nhập để xem danh sách đơn hàng.";
                return RedirectToAction("Login", "Account");
            }

            // Truy vấn danh sách đơn hàng của người dùng
            var orders = await _context.Orders!
                .Include(o => o.orderDetails)
                .ThenInclude(od => od.Product) // Bao gồm sản phẩm
                .Where(o => o.UserId == userId) // Lọc theo UserId
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        // Chi tiết đơn hàng
        public async Task<IActionResult> Details(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                TempData["Error"] = "Bạn cần đăng nhập để xem chi tiết đơn hàng.";
                return RedirectToAction("Login", "Account");
            }

            // Lấy chi tiết đơn hàng của người dùng
            var order = await _context.Orders!
                .Include(o => o.orderDetails)
                .ThenInclude(od => od.Product) // Bao gồm sản phẩm
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId); // Lọc theo UserId và OrderId

            if (order == null)
            {
                TempData["Error"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction("Index");
            }

            return View(order);
        }
    }
}
