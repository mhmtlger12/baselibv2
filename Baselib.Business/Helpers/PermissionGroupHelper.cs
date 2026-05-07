using Baselib.Business.DTOs;
using Baselib.Core.Enums;
using Baselib.Entities;

namespace Baselib.Business.Helpers;

/// <summary>
/// Permission gruplandırma işlemlerini merkezileştiren yardımcı sınıf.
/// RoleService ve PermissionService tarafından ortak kullanılır.
/// </summary>
public static class PermissionGroupHelper
{
    public static List<PermissionGroupDto> BuildGroups(
        IEnumerable<Permission> permissions,
        IReadOnlyCollection<int> selectedIds)
    {
        return permissions
            .GroupBy(p => p.ControllerName)
            .OrderBy(g => g.Key)
            .Select(group =>
            {
                var items = group
                    .OrderBy(p => p.CRUDActionType)
                    .ThenBy(p => p.ActionName)
                    .Select(permission => new ControllerCrudDto
                    {
                        PermissionId = permission.Id,
                        CRUDActionType = permission.CRUDActionType,
                        Name = GetActionName(permission),
                        ActionName = permission.ActionName,
                        Code = permission.Code,
                        Checked = selectedIds.Contains(permission.Id)
                    })
                    .ToList();

                return new PermissionGroupDto
                {
                    ControllerName = group.Key,
                    ControllerCrudList = items,
                    Checked = items.All(c => c.Checked),
                    Indeterminate = items.Any(c => c.Checked) && !items.All(c => c.Checked)
                };
            })
            .ToList();
    }

    public static string GetActionName(Permission permission)
    {
        var crudName = CRUDActionTypes.GetName(permission.CRUDActionType);
        return crudName == permission.CRUDActionType.ToString()
            ? permission.ActionName
            : crudName;
    }
}
