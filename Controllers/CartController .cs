using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcLaptop.Data;
using MvcLaptop.Models;
using MvcLaptop.Services;
namespace MvcLaptop.Controllers
{
    public class CartController : Controller
    {
        private readonly MvcLaptopContext _context; //Dependency Injection
        private readonly ICartService _cartService;
        // Constructor để inject DbContext
        public CartController(MvcLaptopContext context,ICartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }
        // Thêm sản phẩm vào giỏ hàng
        public IActionResult AddToCart(int id, int quantity = 1)
        {
            _cartService.AddToCart(id, quantity);
            TempData["Message"] = "Sản phẩm đã được thêm vào giỏ hàng!";
            return RedirectToAction("Details", "Home", new { id = id });
        }

        // Mua sản phẩm ngay lập tức
        public IActionResult BuyNow(int id, int quantity = 1)
        {   
            _cartService.AddToCart(id, quantity);
            return RedirectToAction("Index");
        }

        // Hiển thị giỏ hàng (tuỳ chọn)
        public IActionResult Index()
        {
            var userName = HttpContext.Session.GetString("UserName");
            ViewData["UserName"] = userName;
            var cartItems = _cartService.GetCartFromSession();
            var cartProducts = cartItems.Select(ci => new
            {
                Product = _context.Product.FirstOrDefault(l => l.Id == ci.Key),
                Quantity = ci.Value
            }).ToList();
            var totalQuantity = cartItems.Values.Sum(); // Cộng tất cả số lượng sản phẩm trong giỏ hàng
            ViewData["CartItemCount"] = totalQuantity;
            // Truyền danh sách sản phẩm và số lượng vào View
            return View(cartProducts);
        }
        public IActionResult UpdateQuantity(int id, int quantity)
        {
            var cartItems = _cartService.GetCartFromSession();
            if (cartItems.ContainsKey(id))
            {
                cartItems[id] = quantity;
            }
            _cartService.SaveCartToSession(cartItems);
            return Json(new { success = true });
        }
        public IActionResult GetCartItemCount()
        {
            var cartItems = _cartService.GetCartFromSession();

            var totalQuantity = cartItems.Values.Sum();

            return Json(new { totalQuantity });
        }
        public IActionResult RemoveFromCart(int id)
        {
            _cartService.RemoveFromCart(id);
            TempData["Message"] = "Sản phẩm đã được xóa khỏi giỏ hàng!";
            return RedirectToAction("Index");
        }
        // Thanh toán giỏ hàng
        [HttpPost]
        public IActionResult Checkout(Dictionary<int, int> quantities)
        {
            var cartItems = _cartService.GetCartFromSession();
            var userName = HttpContext.Session.GetString("UserName");
            ViewData["UserName"] = userName;
            if (string.IsNullOrEmpty(userName))
            {
                TempData["Message"] = "Bạn cần đăng nhập để tiếp tục đặt hàng.";
                TempData["ShowLoginModal"] = true; 
                return RedirectToAction("Index");
            }
            foreach (var item in quantities)
            {
                int productId = item.Key;
                int quantityInCart = item.Value;

                var product = _context.Product.FirstOrDefault(l => l.Id == productId);

                if (product != null && product.Quantity < quantityInCart)
                {
                    TempData["Error"] = $"Sản phẩm {product.Title} không đủ số lượng trong kho!";
                    return RedirectToAction("Index");
                }
            }
            var cartProducts = cartItems.Select(ci => new
            {
                Product = _context.Product.FirstOrDefault(l => l.Id == ci.Key),
                Quantity = ci.Value
            }).ToList();
            var totalPrice = _cartService.CalculateTotalPrice(cartItems);
            ViewData["CartItems"] = cartProducts;
            ViewData["TotalPrice"] = totalPrice;
            return View(new Order());
        }
        [HttpPost]
        public IActionResult ProcessCheckout(Order order)
        {
            var cartItems = _cartService.GetCartFromSession();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Giỏ hàng của bạn trống.";
                return RedirectToAction("Index");
            }

            var totalPrice = _cartService.CalculateTotalPrice(cartItems);
            
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

            // Thêm đơn hàng vào cơ sở dữ liệu
            _context.Orders!.Add(order);
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
