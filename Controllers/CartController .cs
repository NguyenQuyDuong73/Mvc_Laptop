using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcLaptop.Data;
using MvcLaptop.Models;
using MvcLaptop.Services;
using System.Text;
namespace MvcLaptop.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IVnPayService _vnPayService;
        // Constructor để inject DbContext
        public CartController(ICartService cartService, IVnPayService vnPayService)
        {
            _cartService = cartService;
            _vnPayService = vnPayService;
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
        public async Task<IActionResult> Index()
        {
            var userName = HttpContext.Session.GetString("UserName");
            ViewData["UserName"] = userName;
            var cartProducts = await _cartService.GetCartProductsAsync();
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
        public async Task<IActionResult> Checkout(Dictionary<int, int> quantities)
        {
            var user = await _cartService.GetCurrentUserAsync();
            if (user == null)
            {
                TempData["Message"] = "Bạn cần đăng nhập để tiếp tục đặt hàng.";
                TempData["ShowLoginModal"] = true;
                return RedirectToAction("Index");
            }

            var isStockAvailable = await _cartService.CheckProductStockAsync(quantities);
            if (!isStockAvailable)
            {
                TempData["Error"] = "Một số sản phẩm không đủ số lượng trong kho!";
                return RedirectToAction("Index");
            }

            var cartProducts = await _cartService.GetCartProductsAsync();
            var totalPrice = _cartService.CalculateTotalPrice(quantities);
            ViewData["CartItems"] = cartProducts;
            ViewData["TotalPrice"] = totalPrice;
            return View(new Order());
        }
        [HttpPost]
        public async Task<IActionResult> ProcessCheckout(Order order, string paymentMethod)
        {
            try
            {
                // Log phương thức thanh toán
                Console.WriteLine($"[CartController] PaymentMethod: {paymentMethod}");
                // Lấy giỏ hàng từ session
                var cartItems = _cartService.GetCartFromSession();
                Console.WriteLine($"[CartController] CartItems Count: {cartItems.Count}");
                if (!cartItems.Any())
                {
                    TempData["Error"] = "Giỏ hàng của bạn trống.";
                    return RedirectToAction("Index");
                }
                // Tính tổng tiền
                var totalPrice = _cartService.CalculateTotalPrice(cartItems);
                Console.WriteLine($"[CartController] TotalPrice: {totalPrice}");

                // Kiểm tra phương thức thanh toán
                if (paymentMethod == "Banking")
                {
                    // Tạo đơn hàng với trạng thái "Đang chờ thanh toán"
                    order.TotalPrice = totalPrice;
                    order.PaymentMethod = paymentMethod;
                    order.Status = "Đang chờ thanh toán";
                    await _cartService.ProcessCheckoutAsync(order, cartItems, paymentMethod);
                    // Tạo URL thanh toán VNPay
                    var vnPayModel = new VnPaymentRequestModel
                    {
                        Id = order.Id,
                        User = order.User,
                        OrderDate = DateTime.Now,
                        PhoneNumber = order.PhoneNumber,
                        Address = order.Address,
                        PaymentMethod = paymentMethod,
                        UserId = order.UserId,
                        Amount = order.TotalPrice,
                        CreatedDate = DateTime.Now,
                        Description = $"{order.FullName} {order.PhoneNumber}",
                        FullName = order.FullName,
                    };
                    Console.WriteLine($"[CartController] VnPayModel Amount: {vnPayModel.Amount}");
                    Console.WriteLine($"[Controller] TotalPrice received: {totalPrice}");

                    var paymentUrl = _vnPayService.CreatePaymentUrl(HttpContext, vnPayModel);
                    Console.WriteLine($"[CartController] Redirecting to VNPay: {paymentUrl}");
                    return Redirect(paymentUrl); // Chuyển hướng đến trang thanh toán
                }
                else if (paymentMethod == "COD")
                {
                    order.Status = "Thanh toán khi nhận hàng";
                    // Xử lý thanh toán COD
                    var isSuccess = await _cartService.ProcessCheckoutAsync(order, cartItems, paymentMethod);

                    if (isSuccess)
                    {
                        TempData["Message"] = "Đơn hàng của bạn đã được ghi nhận!";
                        return RedirectToAction("Confirmation");
                    }
                    else
                    {
                        TempData["Error"] = "Có lỗi xảy ra trong quá trình thanh toán.";
                        return RedirectToAction("Index");
                    }
                }

                TempData["Error"] = "Phương thức thanh toán không hợp lệ.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Có lỗi xảy ra: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
        public static Dictionary<string, string> vnp_TransactionStatus = new Dictionary<string, string>()
        {
            {"00","Giao dịch thành công" },
            {"01","Giao dịch chưa hoàn tất" },
            {"02","Giao dịch bị lỗi" },
            {"04","Giao dịch đảo (Khách hàng đã bị trừ tiền tại Ngân hàng nhưng GD chưa thành công ở VNPAY)" },
            {"05","VNPAY đang xử lý giao dịch này (GD hoàn tiền)" },
            {"06","VNPAY đã gửi yêu cầu hoàn tiền sang Ngân hàng (GD hoàn tiền)" },
            {"07","Giao dịch bị nghi ngờ gian lận" },
            {"09","GD Hoàn trả bị từ chối" },
            {"24","Giao dịch bị từ chối" }
        };
        public async Task<IActionResult> PaymentCallBack()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            try
            {
                Console.WriteLine($"[PaymentCallBack] OrderId from response: {response.OrderId}");
                // Kiểm tra OrderId hợp lệ
                if (string.IsNullOrEmpty(response.OrderId) || !int.TryParse(response.OrderId, out var orderId))
                {
                    TempData["Error"] = $"ID đơn hàng không hợp lệ: {response.OrderId ?? "null"}";
                    return RedirectToAction(nameof(Confirmation));
                }

                // Gọi phương thức xử lý callback
                await _cartService.HandlePaymentCallbackAsync(orderId, response.VNPayResponseCode!);
                if (response.VNPayResponseCode == "00")
                {
                    TempData["Message"] = "Thanh toán thành công. Đơn hàng của bạn đã được ghi nhận!";
                }
                else
                {
                    if (vnp_TransactionStatus.TryGetValue(response.VNPayResponseCode!, out var message))
                    {
                        TempData["Error"] = $"Lỗi thanh toán: {message}";
                    }
                    else
                    {
                        TempData["Error"] = $"Lỗi không xác định: {response.VNPayResponseCode}";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi trong quá trình xử lý thanh toán: {ex.Message}";
            }

            return RedirectToAction(nameof(Confirmation));
        }
        public IActionResult Confirmation()
        {
            ViewData["Message"] = TempData["Message"] ?? TempData["Error"];
            return View();
        }
    }
}
