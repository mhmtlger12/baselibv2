using System.Collections.Frozen;

namespace Baselib.Core.Constants;

public static class SecurityPermissions
{
    // These operations can change identities, authorization or authentication policy.
    public static IReadOnlySet<string> CriticalCodes { get; } = new[]
    {
        "Users_Create", "Users_Update", "Users_Delete",
        Constants.Permissions.UsersAssignRoles, Constants.Permissions.UsersAssignPrivilegedRoles,
        Constants.Permissions.UsersResetPassword, Constants.Permissions.UsersResetPrivilegedPassword,
        "Roles_Create", "Roles_Update", "Roles_Delete",
        "Permissions_Create", "Permissions_Update", "Permissions_Delete",
        "RecycleBin_Restore", "Settings_Update"
    }.ToFrozenSet(StringComparer.Ordinal);
}
