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

    public static class Authentication
    {
        public const int DefaultMaxLoginAttempts = 5;
        public const int LockoutMinutes = 15;
    }

}
