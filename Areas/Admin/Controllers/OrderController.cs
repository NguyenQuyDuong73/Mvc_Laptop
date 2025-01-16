using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcLaptop.Authorization;
using MvcLaptop.Data;
using MvcLaptop.Models;
using MvcLaptop.Utils.Constants;

namespace MvcLaptop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {

        private readonly MvcLaptopContext _context;

        public OrderController(MvcLaptopContext context)
        {
            _context = context;
        }
        
        // Hiển thị danh sách đơn hàng
        [ClaimRequirement(FunctionCode.SYSTEM_USER, CommandCode.VIEW)]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Lấy UserId từ Identity
            var orders = await _context.Orders!
                .Include(o => o.User) // Bao gồm thông tin người dùng
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }
        // Chi tiết đơn hàng
        [ClaimRequirement(FunctionCode.SYSTEM_USER, CommandCode.VIEW)]
        public async Task<IActionResult> Details(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Lấy UserId từ Identity
            var order = await _context.Orders!
                .Include(o => o.User) // Bao gồm thông tin người dùng
                .Include(o => o.orderDetails)
                .ThenInclude(od => od.Product) // Bao gồm sản phẩm
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
    }
}
