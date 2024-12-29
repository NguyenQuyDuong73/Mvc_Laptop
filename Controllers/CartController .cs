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
        private readonly MvcLaptopContext _context; //Dependency Injection

        // Constructor để inject DbContext
        public CartController(MvcLaptopContext context)
        {
            _context = context;
        }
        // Giỏ hàng tạm thời (có thể thay bằng lưu trữ cơ sở dữ liệu hoặc session)
        private static Dictionary<int, int> CartItems = new Dictionary<int, int>();

        // Thêm sản phẩm vào giỏ hàng
        public IActionResult AddToCart(int id, int quantity = 1)
        {
           if (CartItems.ContainsKey(id)) // Sử dụng ContainsKey để kiểm tra khóa
        {
            // Nếu sản phẩm đã tồn tại, tăng số lượng
            CartItems[id] += quantity;
        }
        else
        {
            // Nếu sản phẩm chưa tồn tại, thêm mới
            CartItems.Add(id, quantity); // Sử dụng Add để thêm một cặp khóa-giá trị
        }

        TempData["Message"] = "Sản phẩm đã được thêm vào giỏ hàng!";
        return RedirectToAction("Details", "Home", new { id = id });
        }

        // Mua sản phẩm ngay lập tức
        public IActionResult BuyNow(int id, int quantity = 1)
        {
            if (CartItems.ContainsKey(id)) // Sử dụng ContainsKey
            {
                // Nếu sản phẩm đã tồn tại, tăng số lượng
                CartItems[id] += quantity;
            }
            else
            {
                // Nếu sản phẩm chưa tồn tại, thêm mới
                CartItems.Add(id, quantity);
            }

            return RedirectToAction("Index");
        }

        // Hiển thị giỏ hàng (tuỳ chọn)
        public IActionResult Index()
        {
            var userName = HttpContext.Session.GetString("UserName");
            ViewData["UserName"] = userName;
            // Lấy danh sách sản phẩm và số lượng từ giỏ hàng
            var cartProducts  = CartItems.Select(ci => new
            {
                Product = _context.Laptop.FirstOrDefault(l => l.Id == ci.Key),
                Quantity = ci.Value
            }).ToList();

            // Tính tổng số lượng sản phẩm trong giỏ hàng
            var totalQuantity = CartItems.Values.Sum(); // Cộng tất cả số lượng sản phẩm trong giỏ hàng

            // Lưu tổng số lượng vào ViewData
            ViewData["CartItemCount"] = totalQuantity;
            // Truyền danh sách sản phẩm và số lượng vào View
            return View(cartProducts);
        }
        public IActionResult UpdateQuantity(int id, int quantity)
        {
            // Cập nhật số lượng sản phẩm trong giỏ hàng
            if (CartItems.ContainsKey(id))
            {
                CartItems[id] = quantity;  // Cập nhật lại số lượng cho sản phẩm
            }
            // Trả về trạng thái thành công hoặc dữ liệu giỏ hàng mới (nếu cần)
            return Json(new { success = true });
        }
        public IActionResult GetCartItemCount()
        {
            // Tính tổng số lượng sản phẩm trong giỏ hàng
            var totalQuantity = CartItems.Values.Sum();

            // Trả về số lượng sản phẩm dưới dạng JSON
            return Json(new { totalQuantity });
        }
        public IActionResult RemoveFromCart(int id)
        {
            if (CartItems.ContainsKey(id)) // Use ContainsKey instead of Contains
            {
                CartItems.Remove(id);
                TempData["Message"] = "Sản phẩm đã được xóa khỏi giỏ hàng!";
            }
            else
            {
                TempData["Message"] = "Sản phẩm không tồn tại trong giỏ hàng!";
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        [HttpPost]
        public IActionResult Checkout(Dictionary<int, int> quantities)
        {
            // Kiểm tra trạng thái đăng nhập
            if (!User.Identity.IsAuthenticated)
            {
                // Nếu chưa đăng nhập, trả về thông báo yêu cầu đăng nhập
                TempData["Message"] = "Bạn cần đăng nhập để tiếp tục thanh toán.";
                TempData["RedirectToLogin"] = true; // Cờ báo để hiển thị thông báo
                return RedirectToAction("Index", "Cart");
            }else{
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
            }
            // // Lưu thay đổi vào cơ sở dữ liệu
            // _context.SaveChanges();

            // // Hiển thị thông báo thanh toán thành công
            // TempData["Message"] = "Thanh toán thành công!";
            return RedirectToAction("Index", "Checkout");

        }

        // Trang xác nhận thanh toán
        public IActionResult Confirmation()
        {
            return View();
        }
    }
}
