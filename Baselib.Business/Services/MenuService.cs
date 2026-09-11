using AutoMapper;
using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Baselib.Core.Interfaces;
using Baselib.Core.Messages;
using Baselib.Core.Results;
using Baselib.Entities;

namespace Baselib.Business.Services;

public class MenuService : IMenuService
{
    private readonly IRepository<Menu> _menus;
    private readonly IRepository<UserRole> _userRoles;
    private readonly IRepository<RolePermission> _rolePermissions;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public MenuService(
        IRepository<Menu> menus,
        IRepository<UserRole> userRoles,
        IRepository<RolePermission> rolePermissions,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _menus = menus;
        _userRoles = userRoles;
        _rolePermissions = rolePermissions;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IDataResult<IEnumerable<MenuDto>>> GetAllAsync()
    {
        var menus = await _menus.GetAllAsync(
            includes: [m => m.Permission!]);

        return DataResult<IEnumerable<MenuDto>>.Ok(
            menus.OrderBy(m => m.ParentId)
                .ThenBy(m => m.Order)
                .ThenBy(m => m.Name)
                .Select(m => _mapper.Map<MenuDto>(m)));
    }

    public async Task<IDataResult<IEnumerable<MenuDto>>> GetMenusByUserIdAsync(int userId)
    {
        var userRoles = await _userRoles.GetAllAsync(ur => ur.UserId == userId);
        var userRoleIds = userRoles.Select(ur => ur.RoleId).ToList();

        var rolePermissions = await _rolePermissions.GetAllAsync(rp => userRoleIds.Contains(rp.RoleId));
        var rolePermissionIds = rolePermissions.Select(rp => rp.PermissionId).Distinct().ToHashSet();

        var menus = await _menus.GetAllAsync(
            predicate: m => m.PermissionId == null || rolePermissionIds.Contains(m.PermissionId.Value),
            includes: [m => m.Permission!]);

        return DataResult<IEnumerable<MenuDto>>.Ok(BuildTree(menus.ToList(), null));
    }

    public async Task<IDataResult<MenuDto>> GetByIdAsync(int id)
    {
        var menu = await _menus.GetByIdAsync(id, m => m.Permission!);

        if (menu == null)
            return DataResult<MenuDto>.NotFound(Messages.Menu.NotFound);

        return DataResult<MenuDto>.Ok(_mapper.Map<MenuDto>(menu));
    }

    public async Task<IDataResult<MenuDto>> CreateAsync(CreateMenuDto dto)
    {
        var menu = new Menu
        {
            Name = dto.Name.Trim(),
            Url = dto.Url?.Trim(),
            Icon = dto.Icon?.Trim(),
            ParentId = dto.ParentId,
            Order = dto.Order,
            PermissionId = dto.PermissionId,
            CreatedDate = DateTime.UtcNow,
            IsActive = true
        };

        await _menus.AddAsync(menu);
        await _unitOfWork.SaveChangesAsync();

        var created = await GetByIdAsync(menu.Id);
        return DataResult<MenuDto>.Created(created.Data!, Messages.General.Saved);
    }

    public async Task<IResult> UpdateAsync(int id, UpdateMenuDto dto)
    {
        var menu = await _menus.GetByIdAsync(id);
        if (menu == null)
            return Result.NotFound(Messages.Menu.NotFound);

        if (dto.ParentId == id)
            return Result.BadRequest(Messages.General.SelfReferenceNotAllowed);

        menu.Name = dto.Name.Trim();
        menu.Url = dto.Url?.Trim();
        menu.Icon = dto.Icon?.Trim();
        menu.ParentId = dto.ParentId;
        menu.Order = dto.Order;
        menu.PermissionId = dto.PermissionId;
        menu.IsActive = dto.IsActive;
        menu.UpdatedDate = DateTime.UtcNow;

        _menus.Update(menu);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok(Messages.General.Updated);
    }

    public async Task<IResult> DeleteAsync(int id)
    {
        var menu = await _menus.GetByIdAsync(id);
        if (menu == null)
            return Result.NotFound(Messages.Menu.NotFound);

        menu.IsActive = false;
        menu.UpdatedDate = DateTime.UtcNow;
        _menus.Update(menu);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok(Messages.General.Deleted);
    }

    private List<MenuDto> BuildTree(List<Menu> menus, int? parentId)
    {
        return menus
            .Where(m => m.ParentId == parentId)
            .OrderBy(m => m.Order)
            .ThenBy(m => m.Name)
            .Select(m =>
            {
                var dto = _mapper.Map<MenuDto>(m);
                dto.SubMenus = BuildTree(menus, m.Id);
                return dto;
            })
            .ToList();
    }
}
