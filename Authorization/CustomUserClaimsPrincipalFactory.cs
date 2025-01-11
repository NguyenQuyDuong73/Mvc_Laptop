using System.Security.Claims;
using MvcLaptop.Models;
using Mvclaptop.Repositories;
using MvcLaptop.Utils.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace MvcLaptop.Authorization;
public class CustomUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<User, Role>
{
    private readonly UserManager<User> _userManager;
    private readonly IPermissionRepository _permissionRepository;

    public CustomUserClaimsPrincipalFactory(
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IPermissionRepository permissionRepository,
        IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, roleManager, optionsAccessor)
    {
        _userManager = userManager;
        _permissionRepository = permissionRepository;
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

        var permissions = await _permissionRepository.GetPermissionsByRolesAsync(roles.ToList());

        // Thêm claim Permissions vào ClaimsIdentity
        identity.AddClaim(new Claim(SystemConstants.Claims.Permissions, JsonConvert.SerializeObject(permissions)));
        identity.AddClaim(new Claim(SystemConstants.Claims.Roles, JsonConvert.SerializeObject(roles)));
        return identity;
    }
}
