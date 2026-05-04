using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Baselib.Core.Interfaces;
using Baselib.Data.Interfaces;
using Baselib.Entities;
using Microsoft.EntityFrameworkCore;

namespace Baselib.Business.Services;

public class MenuService : IMenuService
{
    private readonly IRepository<Menu> _menus;
    private readonly IRepository<UserRole> _userRoles;
    private readonly IRepository<RolePermission> _rolePermissions;
    private readonly IUnitOfWork _unitOfWork;

    public MenuService(
        IRepository<Menu> menus,
        IRepository<UserRole> userRoles,
        IRepository<RolePermission> rolePermissions,
        IUnitOfWork unitOfWork)
    {
        _menus = menus;
        _userRoles = userRoles;
        _rolePermissions = rolePermissions;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<MenuDto>> GetAllAsync()
    {
        var menus = await _menus.Query()
            .Include(m => m.Permission)
            .Where(m => m.IsActive)
            .OrderBy(m => m.ParentId)
            .ThenBy(m => m.Order)
            .ThenBy(m => m.Name)
            .ToListAsync();

        return menus.Select(MapToDto);
    }

    public async Task<IEnumerable<MenuDto>> GetMenusByUserIdAsync(int userId)
    {
        var userRoleIds = await _userRoles.Query()
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync();

        var rolePermissionIds = await _rolePermissions.Query()
            .Where(rp => userRoleIds.Contains(rp.RoleId))
            .Select(rp => rp.PermissionId)
            .Distinct()
            .ToListAsync();

        var menus = await _menus.Query()
            .Include(m => m.Permission)
            .Where(m =>
                m.IsActive &&
                (m.PermissionId == null || rolePermissionIds.Contains(m.PermissionId.Value)))
            .OrderBy(m => m.Order)
            .ThenBy(m => m.Name)
            .ToListAsync();

        return BuildTree(menus, null);
    }

    public async Task<MenuDto?> GetByIdAsync(int id)
    {
        var menu = await _menus.Query()
            .Include(m => m.Permission)
            .FirstOrDefaultAsync(m => m.Id == id);

        return menu == null ? null : MapToDto(menu);
    }

    public async Task<MenuDto> CreateAsync(CreateMenuDto dto)
    {
        var menu = new Menu
        {
            Name = dto.Name.Trim(),
            Url = dto.Url?.Trim(),
            Icon = dto.Icon?.Trim(),
            ParentId = dto.ParentId,
            Order = dto.Order,
            PermissionId = dto.PermissionId,
            CreatedDate = DateTime.Now,
            IsActive = true
        };

        await _menus.AddAsync(menu);
        await _unitOfWork.SaveChangesAsync();

        return (await GetByIdAsync(menu.Id))!;
    }

    public async Task UpdateAsync(int id, UpdateMenuDto dto)
    {
        var menu = await _menus.GetByIdAsync(id);
        if (menu == null)
            throw new KeyNotFoundException("Menu not found");

        if (dto.ParentId == id)
            throw new InvalidOperationException("Menu cannot be its own parent");

        menu.Name = dto.Name.Trim();
        menu.Url = dto.Url?.Trim();
        menu.Icon = dto.Icon?.Trim();
        menu.ParentId = dto.ParentId;
        menu.Order = dto.Order;
        menu.PermissionId = dto.PermissionId;
        menu.IsActive = dto.IsActive;
        menu.UpdatedDate = DateTime.Now;

        await _menus.UpdateAsync(menu);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var menu = await _menus.GetByIdAsync(id);
        if (menu == null)
            throw new KeyNotFoundException("Menu not found");

        menu.IsActive = false;
        menu.UpdatedDate = DateTime.Now;
        await _menus.UpdateAsync(menu);
        await _unitOfWork.SaveChangesAsync();
    }

    private static List<MenuDto> BuildTree(IEnumerable<Menu> menus, int? parentId)
    {
        return menus
            .Where(m => m.ParentId == parentId)
            .OrderBy(m => m.Order)
            .ThenBy(m => m.Name)
            .Select(m =>
            {
                var dto = MapToDto(m);
                dto.SubMenus = BuildTree(menus, m.Id);
                return dto;
            })
            .ToList();
    }

    private static MenuDto MapToDto(Menu menu)
    {
        return new MenuDto
        {
            Id = menu.Id,
            Name = menu.Name,
            Url = menu.Url,
            Icon = menu.Icon,
            ParentId = menu.ParentId,
            SubMenus = new(),
            Order = menu.Order,
            PermissionId = menu.PermissionId,
            PermissionCode = menu.Permission?.Code,
            IsActive = menu.IsActive
        };
    }
}
