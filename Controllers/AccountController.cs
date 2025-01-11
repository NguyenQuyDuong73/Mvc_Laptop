// using Microsoft.AspNetCore.Mvc;
// using MvcLaptop.Models;
// using Microsoft.AspNetCore.Identity;
// using System.Linq;
// using System.Threading.Tasks;
// using MvcLaptop.Data;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.AspNetCore.Authorization;
// using System.Security.Claims;
// using Microsoft.AspNetCore.Authentication.Cookies;
// using Microsoft.AspNetCore.Authentication;
// using Microsoft.AspNetCore.Components.Forms;
// namespace MvcLaptop.Controllers;

// public class AccountController : Controller
// {
//     private readonly UserManager<User> _userManager;
//     private readonly SignInManager<User> _signInManager;
//     private readonly MvcLaptopContext _context;
//     public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, MvcLaptopContext context)
//     {
//         _userManager = userManager;
//         _signInManager = signInManager;
//         _context = context;
//     }
//     // GET: Login
//     public IActionResult Login()
//     {
//         return View();
//     }

//     // POST: Login
//     [HttpPost]
//     [ValidateAntiForgeryToken]
//     public async Task<IActionResult> Login(string userName, string password)
//     {
//         if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
//         {
//             ModelState.AddModelError(string.Empty, "Username Và password Không được để trống.");
//             return View();
//         }
//         var user = await _userManager.FindByNameAsync(userName);
//         if (user == null)
//         {
//             ModelState.AddModelError(string.Empty, "username hoặc password không hợp lệ.");
//             return View();
//         }
//         var result = await _signInManager.PasswordSignInAsync(userName,password,true,lockoutOnFailure:false);
//         return RedirectToAction("Index", "Home");
//     }

//     // GET: Register
//     public IActionResult Register()
//     {
//         return View(new User());
//     }

//     [HttpPost]
//     [ValidateAntiForgeryToken]
//     public async Task<IActionResult> Register([Bind("UserName,Email,Password")] User user)
//     {
//         if (ModelState.IsValid)
//         {
//             // Kiểm tra xem tên người dùng đã tồn tại trong cơ sở dữ liệu chưa
//             var existingUser = await _context.Users!.FirstOrDefaultAsync(u => u.UserName == user.UserName);
//             if (existingUser != null)
//             {
//                 ModelState.AddModelError("UserName", "Tên tài khoản này đã tồn tại. Vui lòng chọn tên khác.");
//                 return View(user);  // Trả về view đăng ký với thông báo lỗi
//             }
//             // Nếu tên người dùng chưa tồn tại, thêm người dùng mới vào cơ sở dữ liệu
//             _context.Add(user);
//             await _context.SaveChangesAsync();
//             // Đăng ký thành công, chuyển hướng tới trang đăng nhập
//             return RedirectToAction(nameof(Login));
//         }
//         return View(user);
//     }
//     // GET: Logout
//     public async Task<IActionResult> Logout()
//     {
//         await _signInManager.SignOutAsync();
//         HttpContext.Session.Clear();
//         return RedirectToAction("Index", "Home");
//     }
//     [Authorize]
//     public async Task<IActionResult> Profile()
//     {
//         var user = await _userManager.GetUserAsync(User);
//         if (user == null)
//         {
//             return RedirectToAction(nameof(Login));
//         }
//         var model = new UserViewModel
//         {
//             UserName = user.UserName,
//             Email = user.Email
//             // Bổ sung các trường khác nếu cần
//         };
//         return View(model);
//     }


// }
