using Microsoft.EntityFrameworkCore;
using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Baselib.Data;
using Baselib.Entities;

namespace Baselib.Business.Services;

public class MenuService : IMenuService
{
    private readonly AppDbContext _context;

    public MenuService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MenuDto>> GetAllAsync()
    {
        var menus = await _context.Menus
            .Include(m => m.SubMenus)
            .Include(m => m.Permission)
            .Where(m => m.ParentId == null && m.IsActive)
            .OrderBy(m => m.Order)
            .ToListAsync();

        return menus.Select(MapTreeToDto);
    }

    public async Task<IEnumerable<MenuDto>> GetMenusByUserIdAsync(int userId)
    {
        var userRoleIds = await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync();

        var rolePermissionIds = await _context.RolePermissions
            .Where(rp => userRoleIds.Contains(rp.RoleId))
            .Select(rp => rp.PermissionId)
            .Distinct()
            .ToListAsync();

        var menus = await _context.Menus
            .Include(m => m.SubMenus)
                .ThenInclude(sm => sm.Permission)
            .Include(m => m.Permission)
            .Where(m => m.ParentId == null && m.IsActive && m.PermissionId != null && rolePermissionIds.Contains(m.PermissionId.Value))
            .OrderBy(m => m.Order)
            .ToListAsync();

        return menus.Select(MapTreeToDto);
    }

    public async Task<MenuDto?> GetByIdAsync(int id)
    {
        var menu = await _context.Menus
            .Include(m => m.SubMenus)
            .Include(m => m.Permission)
            .FirstOrDefaultAsync(m => m.Id == id);

        return menu == null ? null : MapToDto(menu);
    }

    public async Task<MenuDto> CreateAsync(CreateMenuDto dto)
    {
        var menu = new Menu
        {
            Name = dto.Name,
            Url = dto.Url,
            Icon = dto.Icon,
            ParentId = dto.ParentId,
            Order = dto.Order,
            PermissionId = dto.PermissionId,
            CreatedDate = DateTime.Now,
            IsActive = true
        };

        _context.Menus.Add(menu);
        await _context.SaveChangesAsync();

        return (await GetByIdAsync(menu.Id))!;
    }

    public async Task UpdateAsync(int id, UpdateMenuDto dto)
    {
        var menu = await _context.Menus.FindAsync(id);
        if (menu == null) throw new Exception("Menu not found");

        menu.Name = dto.Name;
        menu.Url = dto.Url;
        menu.Icon = dto.Icon;
        menu.ParentId = dto.ParentId;
        menu.Order = dto.Order;
        menu.PermissionId = dto.PermissionId;
        menu.IsActive = dto.IsActive;
        menu.UpdatedDate = DateTime.Now;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var menu = await _context.Menus.FindAsync(id);
        if (menu == null) throw new Exception("Menu not found");

        menu.IsActive = false;
        menu.UpdatedDate = DateTime.Now;
        await _context.SaveChangesAsync();
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
            PermissionCode = menu.Permission?.Code
        };
    }

    private static MenuDto MapTreeToDto(Menu menu)
    {
        return new MenuDto
        {
            Id = menu.Id,
            Name = menu.Name,
            Url = menu.Url,
            Icon = menu.Icon,
            ParentId = menu.ParentId,
            SubMenus = menu.SubMenus
                .Where(m => m.IsActive)
                .Select(MapTreeToDto)
                .ToList(),
            Order = menu.Order,
            PermissionId = menu.PermissionId,
            PermissionCode = menu.Permission?.Code
        };
    }
}