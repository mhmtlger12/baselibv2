using System.Security.Claims;

namespace Baselib.Business.Helpers;

/// <summary>
/// ClaimsPrincipal üzerinden kullanıcı bilgisi çıkaran yardımcı sınıf.
/// </summary>
public static class ClaimsPrincipalHelper
{
    public static int GetUserId(ClaimsPrincipal principal)
    {
        var userIdStr = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
            throw new UnauthorizedAccessException("Oturum bulunamadı.");
        return userId;
    }

    public static int? GetActiveRoleId(ClaimsPrincipal principal)
    {
        var activeRoleIdStr = principal.FindFirst("ActiveRoleId")?.Value;
        return int.TryParse(activeRoleIdStr, out var roleId) ? roleId : null;
    }
}
