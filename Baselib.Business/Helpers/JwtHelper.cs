using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Baselib.Entities;
using Microsoft.IdentityModel.Tokens;

namespace Baselib.Business.Helpers;

/// <summary>
/// JWT ve Refresh Token üretim işlemlerini merkezileştiren yardımcı sınıf.
/// </summary>
public static class JwtHelper
{
    public static string GenerateAccessToken(User user, int? activeRoleId, string key, string issuer, string audience, int expiryMinutes = 15)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email)
        };

        if (activeRoleId.HasValue)
        {
            claims.Add(new Claim("ActiveRoleId", activeRoleId.Value.ToString()));

            var activeRoleName = user.UserRoles.FirstOrDefault(ur => ur.RoleId == activeRoleId.Value)?.Role?.Name;
            if (activeRoleName != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, activeRoleName));
            }
        }
        else
        {
            foreach (var role in user.UserRoles.Select(ur => ur.Role.Name).Distinct())
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
        }

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
}
