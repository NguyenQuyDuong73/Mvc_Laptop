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
        // Lấy giỏ hàng từ Session
        private Dictionary<int, int> GetCartFromSession()
        {
            var cartItemsJson = HttpContext.Session.GetString("CartItems");
            if (string.IsNullOrEmpty(cartItemsJson))
            {
                return new Dictionary<int, int>();
            }
#pragma warning disable CS8603 // Possible null reference return.
            return System.Text.Json.JsonSerializer.Deserialize<Dictionary<int, int>>(cartItemsJson);
#pragma warning restore CS8603 // Possible null reference return.
        }

        // Lưu giỏ hàng vào Session
        private void SaveCartToSession(Dictionary<int, int> cartItems)
        {
            var cartItemsJson = System.Text.Json.JsonSerializer.Serialize(cartItems);
            HttpContext.Session.SetString("CartItems", cartItemsJson);
        }

        // Thêm sản phẩm vào giỏ hàng
        public IActionResult AddToCart(int id, int quantity = 1)
        {
            // Lấy giỏ hàng từ Session
            var cartItems = GetCartFromSession();
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
            // Lưu giỏ hàng vào Session
            SaveCartToSession(cartItems);
            TempData["Message"] = "Sản phẩm đã được thêm vào giỏ hàng!";
            return RedirectToAction("Details", "Home", new { id = id });
        }

        // Mua sản phẩm ngay lập tức
        public IActionResult BuyNow(int id, int quantity = 1)
        {
            var cartItems = GetCartFromSession();
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
            SaveCartToSession(cartItems);
            return RedirectToAction("Index");
        }

        // Hiển thị giỏ hàng (tuỳ chọn)
        public IActionResult Index()
        {
            var userName = HttpContext.Session.GetString("UserName");
            ViewData["UserName"] = userName;
            // Lấy danh sách sản phẩm và số lượng từ Session
            var cartItems = GetCartFromSession();
            // Lấy danh sách sản phẩm và số lượng từ giỏ hàng
            var cartProducts = cartItems.Select(ci => new
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
            var cartItems = GetCartFromSession();
            // Cập nhật số lượng sản phẩm trong giỏ hàng
            if (cartItems.ContainsKey(id))
            {
                cartItems[id] = quantity;  // Cập nhật lại số lượng cho sản phẩm
            }
            // Trả về trạng thái thành công hoặc dữ liệu giỏ hàng mới (nếu cần)
            SaveCartToSession(cartItems);
            return Json(new { success = true });
        }
        public IActionResult GetCartItemCount()
        {
            var cartItems = GetCartFromSession();
            // Tính tổng số lượng sản phẩm trong giỏ hàng
            var totalQuantity = cartItems.Values.Sum();

            // Trả về số lượng sản phẩm dưới dạng JSON
            return Json(new { totalQuantity });
        }
        public IActionResult RemoveFromCart(int id)
        {
            var cartItems = GetCartFromSession();
            if (cartItems.ContainsKey(id)) // Use ContainsKey instead of Contains
            {
                cartItems.Remove(id);
                TempData["Message"] = "Sản phẩm đã được xóa khỏi giỏ hàng!";
            }
            else
            {
                TempData["Message"] = "Sản phẩm không tồn tại trong giỏ hàng!";
            }
            SaveCartToSession(cartItems);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Checkout(Dictionary<int, int> quantities)
        {
            var cartItems = GetCartFromSession();
            // Kiểm tra trạng thái đăng nhập từ Session
            var userName = HttpContext.Session.GetString("UserName");
            ViewData["UserName"] = userName;
            // Kiểm tra nếu giỏ hàng trống
            if (string.IsNullOrEmpty(userName))
            {
                // Nếu chưa đăng nhập, trả về thông báo và hiển thị modal
                TempData["Message"] = "Bạn cần đăng nhập để tiếp tục đặt hàng.";
                TempData["ShowLoginModal"] = true; // Hiển thị modal yêu cầu đăng nhập
                return RedirectToAction("Index");
            }
            foreach (var item in quantities)
            {
                int productId = item.Key;
                int quantityInCart = item.Value;

                // Lấy sản phẩm từ cơ sở dữ liệu
                var product = _context.Laptop.FirstOrDefault(l => l.Id == productId);

                if (product != null)
                {
                    // Kiểm tra số lượng tồn kho
                    if (product.Quantity < quantityInCart)
                    {
                        TempData["Error"] = $"Sản phẩm {product.Title} không đủ số lượng trong kho!";
                        return RedirectToAction("Index");
                    }
                }
            }
            var cartProducts = cartItems.Select(ci => new
            {
                Product = _context.Laptop.FirstOrDefault(l => l.Id == ci.Key),
                Quantity = ci.Value
            }).ToList();
            // Tính tổng tiền với kiểu dữ liệu chính xác
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var totalPrice = cartProducts.Sum(item => 
                Convert.ToDecimal(item.Product.Price) * Convert.ToDecimal(item.Quantity)
            );
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            ViewData["CartItems"] = cartProducts;
            ViewData["TotalPrice"] = totalPrice;
            return View(new Order());
        }
        [HttpPost]
        public IActionResult ProcessCheckout(Order order)
        {
            // Lấy giỏ hàng từ Session
            var cartItems = GetCartFromSession();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Giỏ hàng của bạn trống.";
                return RedirectToAction("Index");
            }

            var totalPrice = cartItems.Sum(ci => 
            Convert.ToDecimal(ci.Value) * (_context.Laptop.FirstOrDefault(l => l.Id == ci.Key)?.Price ?? 0m));


            // Lấy thông tin người dùng từ Session
            var userName = HttpContext.Session.GetString("UserName");
            var email = HttpContext.Session.GetString("Email");

            // Kiểm tra nếu người dùng chưa đăng nhập
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(email))
            {
                TempData["Error"] = "Bạn cần đăng nhập để tiếp tục đặt hàng.";
                return RedirectToAction("Index");
            }

            // Đặt giá trị cho Order
            order.OrderDate = DateTime.Now;
            order.TotalPrice = totalPrice;
            order.UserName = userName;
            order.Email = email;

            // Thêm đơn hàng vào cơ sở dữ liệu
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            _context.Orders.Add(order);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            _context.SaveChanges();

            // Xóa giỏ hàng sau khi đặt hàng
            HttpContext.Session.Remove("CartItems");

            TempData["Message"] = "Đơn hàng của bạn đã được ghi nhận!";
            return RedirectToAction("Confirmation");
        }

        public IActionResult Confirmation()
        {
            ViewData["Message"] = TempData["Message"];
            return View();
        }
    }
}
