using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcLaptop.Authorization;
using MvcLaptop.Data;
using MvcLaptop.Models;
using MvcLaptop.Utils.Constants;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var order = await _context.Orders!.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = status;
            await _context.SaveChangesAsync();

            return RedirectToAction("");
        }

        public IActionResult RevenueChart(int? selectedYear)
        {
            var years = _context.Orders!
                .Select(o => o.OrderDate.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToList();

            int yearToView = selectedYear ?? years.FirstOrDefault(); // mặc định: năm mới nhất

            var revenueData = _context.Orders!
                .Where(o => o.OrderDate.Year == yearToView)
                .GroupBy(o => o.OrderDate.Month)
                .Select(g => new RevenueByMonth
                {
                    Month = g.Key,
                    Year = yearToView,
                    TotalRevenue = g.Sum(o => o.TotalPrice)
                })
                .OrderBy(r => r.Month)
                .ToList();

            ViewBag.Years = new SelectList(years);
            ViewBag.SelectedYear = yearToView;

            return View(revenueData);
        }
        public IActionResult OrderStatsChart()
        {
            var data = _context.Orders!
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .Select(g => new OrderStatsByMonth
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    OrderCount = g.Count()
                })
                .OrderBy(r => r.Year).ThenBy(r => r.Month)
                .ToList();

            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var order = await _context.Orders!.FindAsync(id);
            if (order == null) return NotFound();

            // Cập nhật trạng thái hoặc xử lý logic hủy
            order.Status = "Đã bị hủy";
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
