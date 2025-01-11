namespace MvcLaptop.Utils.Constants;

public static class SystemConstants
{
    public class Claims
    {
        public const string Permissions = "Permissions";
        public const string GivenName = "GivenName";
        public const string Avatar = "Avatar";
        public const string Roles = "Roles";
        public const string UserSettings = "UserSettings";
    }

    public static class Roles
    {
        public const string Administrator = "Administrator";
        public const string Manager = "Manager";
        public const string User = "User";
    }

    public static class Policies
    {
        public const string RequireAdmin = "RequireAdmin";
        public const string RequireManager = "RequireManager";
    }
}
