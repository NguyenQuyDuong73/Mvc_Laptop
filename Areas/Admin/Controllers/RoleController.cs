using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mvclaptop.Repositories;
using MvcLaptop.Authorization;
using MvcLaptop.Data;
using MvcLaptop.Models;
using MvcLaptop.Utils.Constants;

namespace MvcLaptop.Controllers
{
    [Area("Admin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IFunctionRepository _functionRepository;
        private readonly MvcLaptopContext _context;
        public RoleController(RoleManager<Role> roleManager, UserManager<User> userManager, MvcLaptopContext context, IRoleRepository roleRepository, IPermissionRepository permissionRepository, IFunctionRepository functionRepository)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
            _functionRepository = functionRepository;
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
        [ClaimRequirement(FunctionCode.SYSTEM_USER, CommandCode.VIEW)]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string searchName = "")
        {
            var roles = await _roleRepository.GetRolesPagedAsync(page, pageSize, searchName);
            ViewBag.SearchName = searchName;

            // Lấy số người dùng cho mỗi vai trò
            var roleUserCounts = new Dictionary<string, int>();
            foreach (var role in roles.Items!)
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
                roleUserCounts[role.Name!] = usersInRole.Count;
            }

            // Truyền số người dùng vào ViewBag
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
        [ClaimRequirement(FunctionCode.SYSTEM_USER, CommandCode.CREATE)]
        public async Task<IActionResult> Create(string roleName)
        {
            if (!string.IsNullOrEmpty(roleName))
            {
                var result = await _roleManager.CreateAsync(new Role { Name = roleName });
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
        // [HttpGet]
        // public async Task<IActionResult> EditRole(string roleId)
        // {
        //     var userName = HttpContext.Session.GetString("UserName");
        //     ViewData["UserName"] = userName;
        //     if (string.IsNullOrEmpty(roleId))
        //     {
        //         return BadRequest("Id vai trò không hợp lệ.");
        //     }

        //     var role = await _roleManager.FindByIdAsync(roleId);
        //     if (role == null)
        //     {
        //         return NotFound("Không tìm thấy vai trò.");
        //     }

        //     var model = new EditRoleViewModel
        //     {
        //         RoleId = role.Id,
        //         RoleName = role.Name!
        //     };

        //     return View(model);
        // }
        // [HttpPost]
        // [ClaimRequirement(FunctionCode.SYSTEM_USER, CommandCode.UPDATE)]
        // public async Task<IActionResult> EditRole(EditRoleViewModel model)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return View(model); // Trả về view nếu dữ liệu không hợp lệ
        //     }
        //     var role = await _roleManager.FindByIdAsync(model.RoleId);
        //     if (role == null)
        //     {
        //         return NotFound("Không tìm thấy vai trò.");
        //     }

        //     if (string.IsNullOrEmpty(model.RoleName))
        //     {
        //         ModelState.AddModelError("", "Tên vai trò không được để trống.");
        //         return View(model);
        //     }

        //     role.Name = model.RoleName;
        //     var result = await _roleManager.UpdateAsync(role);

        //     if (result.Succeeded)
        //     {
        //         return RedirectToAction(nameof(Index));
        //     }

        //     foreach (var error in result.Errors)
        //     {
        //         ModelState.AddModelError("", error.Description);
        //     }

        //     return View(model);
        // }
        // GET: Edit Role
        [HttpGet]
        [ClaimRequirement(FunctionCode.SYSTEM_ROLE, CommandCode.UPDATE)]
        public async Task<IActionResult> Edit(string id)
        {
            // Tìm kiếm role theo ID
            var role = await _roleRepository.GetRoleByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            // Load permissions cho role
            var commandInFunction = await _permissionRepository.GetAllCommandInFunctionsAsync();
            var assignedPermissions = await _permissionRepository.GetPermissionsByRoleIdAsync(role.Id);

            // Tạo HashSet để tra cứu nhanh các permission đã được gán
            var assignedPermissionSet = new HashSet<string>(
                assignedPermissions.Select(ap => $"{ap.FunctionId} - {ap.CommandId}")
            );

            // Tạo danh sách PermissionViewModel với tên và biểu tượng của Function
            var viewModel = new EditRoleViewModel
            {
                Role = role,
                Permissions = commandInFunction.Select(p => new PermissionViewModel
                {
                    FunctionId = p.Function!.Id,
                    RoleId = role.Id,
                    CommandId = p.Command!.Id,
                    // Kiểm tra xem permission này có được gán hay không
                    IsAssigned = assignedPermissionSet.Contains($"{p.Function.Id} - {p.Command.Id}"),
                    FunctionName = p.Function?.Name,
                    CommandName = p.Command?.Name,
                    FunctionIcon = p.Function?.Icon,
                    FunctionParentId = p.Function?.ParentId,
                }).ToList(),
                SelectedPermissions = assignedPermissions.Select(ap => $"{ap.FunctionId} - {ap.CommandId}").ToList() // Gán SelectedPermissions cho ViewModel
            };

            // foreach (var permission in viewModel.Permissions)
            // {
            //     permission.AssignCommandIcon();
            // }

            return View(viewModel);
        }
        // POST: Edit Role
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditRoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (model.SelectedPermissions == null || !model.SelectedPermissions.Any())
            {
                ModelState.AddModelError("SelectedPermissions", "Vui lòng chọn ít nhất một quyền.");
                return View(model);
            }
            var role = await _roleRepository.GetRoleByIdAsync(model.Role!.Id);
            if (role == null)
            {
                return NotFound();
            }

            // Cập nhật thông tin của role
            role.Name = model.Role.Name;
            await _roleRepository.UpdateRoleAsync(role);

            // Cập nhật quyền cho role
            await _permissionRepository.UpdateRolePermissionsAsync(model.Role.Id, model.SelectedPermissions);

            return RedirectToAction(nameof(Index));
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
        [ClaimRequirement(FunctionCode.SYSTEM_USER, CommandCode.VIEW)]
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
        [HttpPost]
        [ClaimRequirement(FunctionCode.SYSTEM_USER, CommandCode.DELETE)]
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

        [HttpPost]
        [ClaimRequirement(FunctionCode.SYSTEM_USER, CommandCode.UPDATE)]
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
        [ClaimRequirement(FunctionCode.SYSTEM_USER, CommandCode.DELETE)]
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
