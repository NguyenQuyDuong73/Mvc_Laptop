using System.Security.Claims;

namespace MvcLaptop.Extensions
{
    public static class Identity_Extensions
    {
        public static string GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            var claim = ((ClaimsIdentity?)claimsPrincipal.Identity)?
                .Claims
                .SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            return claim!.Value;
        }

        public static string? GetAccessToken(this ClaimsPrincipal claimsPrincipal)
        {
            var accessToken = claimsPrincipal.FindFirstValue("access_token");
            return accessToken;
        }

    }
}