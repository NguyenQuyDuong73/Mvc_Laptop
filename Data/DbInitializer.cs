using MvcLaptop.Models;
using Microsoft.AspNetCore.Identity;

namespace MvcLaptop.Data;

public class DbInitializer
{
    private readonly MvcLaptopContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly string AdminRoleName = "Administrator";
    private readonly string ManagerRoleName = "Manager";
    private readonly string UserRoleName = "Member";

    public DbInitializer(UserManager<User> userManager, RoleManager<Role> roleManager, MvcLaptopContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    public async Task Seed()
    {
        #region Role
        var predefinedRoles = GetPredefinedRoles();
        if (!_roleManager.Roles.Any())
        {
            foreach (var role in predefinedRoles)
            {
                await _roleManager.CreateAsync(role);
            }
        }
        #endregion

        #region User
        var predefinedUsers = GetPredefinedUsers();
        if (!_userManager.Users.Any())
        {
            foreach (var userModel in predefinedUsers)
            {
                var result = await _userManager.CreateAsync(userModel.User, userModel.Password);
                if (result.Succeeded)
                {
                    await _context.SaveChangesAsync();
                    var user = await _userManager.FindByNameAsync(userModel.User.UserName!);
                    if (user != null)
                    {
                        await _userManager.AddToRolesAsync(user, userModel.Roles);
                    }
                }
            }
        }
        #endregion

        #region Function
        var predefinedFunctions = GetPredefinedFunctions();
        if (!_context.Functions.Any())
        {
            _context.Functions.AddRange(predefinedFunctions);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region Command
        var predefinedCommands = GetPredefinedCommands();
        if (!_context.Commands.Any())
        {
            _context.Commands.AddRange(predefinedCommands);
        }
        #endregion

        #region Permissions
        if (!_context.Permissions.Any())
        {
            var adminRole = await _roleManager.FindByNameAsync(AdminRoleName);
            var managerRole = await _roleManager.FindByNameAsync(ManagerRoleName);
            var permissions = GetPredefinedPermissions(predefinedFunctions, adminRole!, managerRole!);
            _context.Permissions.AddRange(permissions);
        }
        #endregion

        await _context.SaveChangesAsync();
    }

    private List<Role> GetPredefinedRoles()
    {
        return new List<Role>
        {
            new Role { Id = AdminRoleName.ToUpper(), Name = AdminRoleName, NormalizedName = AdminRoleName.ToUpper() },
            new Role { Id = UserRoleName.ToUpper(), Name = UserRoleName, NormalizedName = UserRoleName.ToUpper() },
            new Role { Id = ManagerRoleName.ToUpper(), Name = ManagerRoleName, NormalizedName = ManagerRoleName.ToUpper() }
        };
    }

    private List<(User User, string Password, string[] Roles)> GetPredefinedUsers()
    {
        return new List<(User, string, string[])>
        {
            (
                new User
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "trungnguyen7358@gmail.com",
                    Email = "trungnguyen7358@gmail.com",
                    LockoutEnabled = false,
                    EmailConfirmed = true,
                    PhoneNumber = "0976982240"
                },
                "Admin@123",
                new[] { AdminRoleName, ManagerRoleName }
            ),
            (
                new User
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "trungnguyen7358@gmail.com",
                    Email = "trungnguyen7358@gmail.com",
                    EmailConfirmed = false,
                    LockoutEnabled = false,
                    PhoneNumber = "0976982240"
                },
                "Admin@123",
                new[] { UserRoleName, ManagerRoleName }
            )
        };
    }

    private List<Function> GetPredefinedFunctions()
    {
        return new List<Function>
        {
            new Function { Id = "DASHBOARD", Name = "Dashboard", ParentId = null, SortOrder = 1, Url = "/dashboard", Icon = "bi bi-house-door" },
            new Function { Id = "SYSTEM", Name = "System", ParentId = null, Url = "/systems", Icon = "bi bi-gear" },
            new Function { Id = "SYSTEM_USER", Name = "User", ParentId = "SYSTEM", Url = "/systems/users", Icon = "bi bi-person" },
            new Function { Id = "SYSTEM_ROLE", Name = "Role", ParentId = "SYSTEM", Url = "/systems/roles", Icon = "bi bi-shield-lock" },
            new Function { Id = "SYSTEM_FUNCTION", Name = "Function", ParentId = "SYSTEM", Url = "/systems/functions", Icon = "bi bi-list" },
            new Function { Id = "SYSTEM_PERMISSION", Name = "Permission", ParentId = "SYSTEM", Url = "/systems/permissions", Icon = "bi bi-lock" }
        };
    }

    private List<Command> GetPredefinedCommands()
    {
        return new List<Command>
        {
            new Command { Id = "VIEW", Name = "View" },
            new Command { Id = "CREATE", Name = "Create" },
            new Command { Id = "UPDATE", Name = "Update" },
            new Command { Id = "DELETE", Name = "Delete" },
            new Command { Id = "APPROVE", Name = "Approve" },
            new Command { Id = "REJECT", Name = "Reject" }
        };
    }

    private List<Permission> GetPredefinedPermissions(IEnumerable<Function> functions, Role adminRole, Role managerRole)
    {
        var permissions = new List<Permission>();

        if (adminRole != null)
        {
            foreach (var function in functions)
            {
                permissions.AddRange(new[]
                {
                    new Permission(function.Id!, adminRole.Id, "VIEW"),
                    new Permission(function.Id!, adminRole.Id, "CREATE"),
                    new Permission(function.Id!, adminRole.Id, "UPDATE"),
                    new Permission(function.Id!, adminRole.Id, "DELETE"),
                    new Permission(function.Id!, adminRole.Id, "APPROVE"),
                    new Permission(function.Id!, adminRole.Id, "REJECT")
                });
            }
        }

        if (managerRole != null)
        {
            foreach (var function in functions)
            {
                if (!function.Id!.Contains("SYSTEM_ROLE"))
                {
                    permissions.AddRange(new[]
                    {
                        new Permission(function.Id, managerRole.Id, "VIEW"),
                        new Permission(function.Id, managerRole.Id, "CREATE"),
                        new Permission(function.Id, managerRole.Id, "UPDATE"),
                        new Permission(function.Id, managerRole.Id, "DELETE"),
                        new Permission(function.Id, managerRole.Id, "APPROVE"),
                        new Permission(function.Id, managerRole.Id, "REJECT")
                    });
                }
            }
            permissions.Add(new Permission("SYSTEM_ROLE", managerRole.Id, "VIEW"));
        }

        return permissions;
    }
}