using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcLaptop.Data;
using MvcLaptop.Models;
namespace MvcLaptop.Controllers
{
    public class CartController : Controller
    {
        private readonly MvcLaptopContext _context;

        // Constructor để inject DbContext
        public CartController(MvcLaptopContext context)
        {
            _context = context;
        }
        // Giỏ hàng tạm thời (có thể thay bằng lưu trữ cơ sở dữ liệu hoặc session)

        // Thêm sản phẩm vào giỏ hàng
        public IActionResult AddToCart(int id, int quantity = 1)
        {
            var cartItems = HttpContext.Session.GetObject<Dictionary<int, int>>("CartItems") ?? new Dictionary<int, int>();
            if (cartItems.ContainsKey(id)) // Sử dụng ContainsKey để kiểm tra khóa
            {
                // Nếu sản phẩm đã tồn tại, tăng số lượng
                cartItems[id] += quantity;
            }
            else
            {
                // Nếu sản phẩm chưa tồn tại, thêm mới
                cartItems.Add(id, quantity); // Sử dụng Add để thêm một cặp khóa-giá trị
            }

            HttpContext.Session.SetObject("CartItems", cartItems);

            TempData["Message"] = "Sản phẩm đã được thêm vào giỏ hàng!";
            return RedirectToAction("Details", "Home", new { id = id });
        }

        // Mua sản phẩm ngay lập tức
        public IActionResult BuyNow(int id, int quantity = 1)
        {
            // Lấy giỏ hàng từ session
            var cartItems = HttpContext.Session.GetObject<Dictionary<int, int>>("CartItems") ?? new Dictionary<int, int>();
            if (cartItems.ContainsKey(id)) // Sử dụng ContainsKey
            {
                // Nếu sản phẩm đã tồn tại, tăng số lượng
                cartItems[id] += quantity;
            }
            else
            {
                // Nếu sản phẩm chưa tồn tại, thêm mới
                cartItems.Add(id, quantity);
            }
            HttpContext.Session.SetObject("CartItems", cartItems);
            return RedirectToAction("Index");
        }

        // Hiển thị giỏ hàng (tuỳ chọn)
        public IActionResult Index()
        {
            // Lấy giỏ hàng từ session
            var cartItems = HttpContext.Session.GetObject<Dictionary<int, int>>("CartItems") ?? new Dictionary<int, int>();
            // Lấy danh sách sản phẩm và số lượng từ giỏ hàng
            var cartProducts  = cartItems.Select(ci => new
            {
                Product = _context.Laptop.FirstOrDefault(l => l.Id == ci.Key),
                Quantity = ci.Value
            }).ToList();

            // Tính tổng số lượng sản phẩm trong giỏ hàng
            var totalQuantity = cartItems.Values.Sum(); // Cộng tất cả số lượng sản phẩm trong giỏ hàng

            // Lưu tổng số lượng vào ViewData
            ViewData["CartItemCount"] = totalQuantity;
            // Truyền danh sách sản phẩm và số lượng vào View
            return View(cartProducts);
        }
        public IActionResult UpdateQuantity(int id, int quantity)
        {
            // Lấy giỏ hàng từ session
            var cartItems = HttpContext.Session.GetObject<Dictionary<int, int>>("CartItems") ?? new Dictionary<int, int>();

            // Cập nhật số lượng sản phẩm trong giỏ hàng
            if (cartItems.ContainsKey(id))
            {
                cartItems[id] = quantity;  // Cập nhật lại số lượng cho sản phẩm
            }

            // Lưu giỏ hàng lại vào session
            HttpContext.Session.SetObject("CartItems", cartItems);

            // Trả về trạng thái thành công hoặc dữ liệu giỏ hàng mới (nếu cần)
            return Json(new { success = true });
        }

        public IActionResult RemoveFromCart(int id)
        {
            var cartItems = HttpContext.Session.GetObject<Dictionary<int, int>>("CartItems") ?? new Dictionary<int, int>();
            if (cartItems.ContainsKey(id)) // Use ContainsKey instead of Contains
            {
                cartItems.Remove(id);
                TempData["Message"] = "Sản phẩm đã được xóa khỏi giỏ hàng!";
            }
            else
            {
                TempData["Message"] = "Sản phẩm không tồn tại trong giỏ hàng!";
            }

            HttpContext.Session.SetObject("CartItems", cartItems);

            return RedirectToAction("Index");
        }
        [HttpPost]
        [HttpPost]
        public IActionResult Checkout(Dictionary<int, int> quantities)
        {
            foreach (var item in quantities)
            {
                int productId = item.Key;
                int quantityInCart = item.Value;

                // Lấy sản phẩm từ cơ sở dữ liệu
                var product = _context.Laptop.FirstOrDefault(l => l.Id == productId);

                if (product != null)
                {
                    // Kiểm tra số lượng tồn kho
                    if (product.Quantity >= quantityInCart)
                    {
                        product.Quantity -= quantityInCart; // Trừ số lượng trong kho
                    }
                    else
                    {
                        TempData["Error"] = $"Sản phẩm {product.Title} không đủ số lượng trong kho!";
                        return RedirectToAction("Index");
                    }
                }
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            _context.SaveChanges();

            // Hiển thị thông báo thanh toán thành công
            TempData["Message"] = "Thanh toán thành công!";
            return RedirectToAction("Confirmation");
        }

        // Trang xác nhận thanh toán
        public IActionResult Confirmation()
        {
            return View();
        }
    }
}
