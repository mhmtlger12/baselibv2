namespace Baselib.Core.Constants;

public static class Constants
{
    public static class Jwt
    {
        public const string Key = "Jwt:Key";
        public const string Issuer = "Jwt:Issuer";
        public const string Audience = "Jwt:Audience";
        public const int AccessTokenExpiryMinutes = 15;
        public const int RefreshTokenExpiryDays = 7;
    }

    public static class Pagination
    {
        public const int DefaultPageSize = 10;
        public const int MaxPageSize = 100;
    }

    public static class Cache
    {
        public const string UserPrefix = "user_";
        public const string RolePrefix = "role_";
        public const string MenuPrefix = "menu_";
    }

    public static class Roles
    {
        public const string Admin = "Admin";
        public const string User = "User";
        public const string Manager = "Manager";
    }
}