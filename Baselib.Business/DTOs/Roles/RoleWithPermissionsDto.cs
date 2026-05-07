namespace Baselib.Business.DTOs;

public class RoleWithPermissionsDto
{
    public UpdateRoleDto Role { get; set; } = null!;
    public List<PermissionGroupDto> PermissionGroups { get; set; } = new();
}
