using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MvcLaptop.Data;
using MvcLaptop.Models;

namespace MvcLaptop.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Staff")]
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
        [Authorize(Roles = "Admin")]
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
        [HttpGet]
        public async Task<IActionResult> EditRole(string roleId)
        {
            var userName = HttpContext.Session.GetString("UserName");
            ViewData["UserName"] = userName;
            if (string.IsNullOrEmpty(roleId))
            {
                return BadRequest("Id vai trò không hợp lệ.");
            }

            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return NotFound("Không tìm thấy vai trò.");
            }

            var model = new EditRoleViewModel
            {
                RoleId = role.Id,
                RoleName = role.Name!
            };

            return View(model);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); // Trả về view nếu dữ liệu không hợp lệ
            }
            var role = await _roleManager.FindByIdAsync(model.RoleId);
            if (role == null)
            {
                return NotFound("Không tìm thấy vai trò.");
            }

            if (string.IsNullOrEmpty(model.RoleName))
            {
                ModelState.AddModelError("", "Tên vai trò không được để trống.");
                return View(model);
            }

            role.Name = model.RoleName;
            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
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
            var userName = HttpContext.Session.GetString("UserName");
            ViewData["UserName"] = userName;
            var users = _userManager.Users.ToList();
            var userViewModels = new List<UserViewModel>();
            var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();
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
            // Truyền danh sách tất cả các vai trò vào ViewBag

            ViewBag.AllRoles = allRoles ?? new List<string>()!; // Đảm bảo không null
            return View(userViewModels);
        }
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> UpdateUserRoles(Dictionary<string, string[]> userRoles)
        {
            foreach (var userId in userRoles.Keys)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) continue;

                // Lấy tất cả vai trò hiện tại của người dùng
                var currentRoles = await _userManager.GetRolesAsync(user);

                // Vai trò mới được chọn
                var selectedRoles = userRoles[userId];

                // Xóa vai trò không được chọn
                var rolesToRemove = currentRoles.Except(selectedRoles).ToList();
                await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

                // Thêm vai trò mới
                var rolesToAdd = selectedRoles.Except(currentRoles).ToList();
                await _userManager.AddToRolesAsync(user, rolesToAdd);
            }

            return RedirectToAction(nameof(UserList));
        }
        [HttpPost]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            // Tìm vai trò cần xóa
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return NotFound(); // Trả về lỗi nếu vai trò không tồn tại
            }

            // Xóa vai trò khỏi hệ thống
            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index)); // Chuyển hướng về danh sách vai trò
            }

            // Nếu không thể xóa, thêm lỗi vào ModelState
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
