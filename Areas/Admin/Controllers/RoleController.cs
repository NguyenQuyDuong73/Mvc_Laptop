using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MvcLaptop.Data;
using MvcLaptop.Models;

namespace MvcLaptop.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly MvcLaptopContext _context;
        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager, MvcLaptopContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
        }
        // Phương thức lấy thông tin người dùng và vai trò
        private async Task SetUserViewDataAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                ViewData["UserName"] = user.UserName;
                var roles = await _userManager.GetRolesAsync(user);
                ViewData["UserRoles"] = roles;
            }
        }
        // Hiển thị danh sách vai trò
        public async Task<IActionResult> Index()
        {
            var userName = HttpContext.Session.GetString("UserName");
            ViewData["UserName"] = userName;
            // await SetUserViewDataAsync();
            // var roles = _roleManager.Roles.ToList();
            // return View(roles);
            if (User.Identity!.IsAuthenticated)
            {
                Console.WriteLine($"Người dùng đã đăng nhập: {User.Identity.Name}");
                Console.WriteLine($"Quyền: {string.Join(", ", User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value))}");
            }
            else
            {
                Console.WriteLine("Người dùng chưa đăng nhập.");
            }
            var roles = _roleManager.Roles.ToList();
            var roleUserCounts = new Dictionary<string, int>();

            foreach (var role in roles)
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
                roleUserCounts[role.Name!] = usersInRole.Count;
            }

            ViewBag.RoleUserCounts = roleUserCounts;
            return View(roles);
        }

        // Tạo vai trò mới
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string roleName)
        {
            if (!string.IsNullOrEmpty(roleName))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(roleName);
        }

        // Gán vai trò cho người dùng
        [HttpGet]
        public async Task<IActionResult> AssignRole(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            ViewBag.Roles = _roleManager.Roles.ToList();
            ViewBag.UserId = userId;

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            if (!await _roleManager.RoleExistsAsync(role))
            {
                ModelState.AddModelError(string.Empty, "Role không tồn tại.");
                return View(user);
            }

            var result = await _userManager.AddToRoleAsync(user, role);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(user);
        }
        [HttpGet]
        public async Task<IActionResult> UserList()
        {
            var users = _userManager.Users.ToList();
            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles.ToList() // Gắn danh sách vai trò
                });
            }

            return View(userViewModels);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.RemoveFromRoleAsync(user, role);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(UserList));
            }

            ModelState.AddModelError(string.Empty, "Không thể xóa vai trò.");
            return RedirectToAction(nameof(UserList));
        }

    }
}
