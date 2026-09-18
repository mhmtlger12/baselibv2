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
    private readonly TimeProvider _timeProvider;

    public MenuService(
        IRepository<Menu> menus,
        IRepository<UserRole> userRoles,
        IRepository<RolePermission> rolePermissions,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        TimeProvider timeProvider)
    {
        _menus = menus;
        _userRoles = userRoles;
        _rolePermissions = rolePermissions;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _timeProvider = timeProvider;
    }

    public async Task<IDataResult<IEnumerable<MenuDto>>> GetAllAsync()
    {
        var menus = await _menus.GetAllAsync(
            asNoTracking: true,
            includes: [m => m.Permission!]);

        return DataResult<IEnumerable<MenuDto>>.Ok(
            menus.OrderBy(m => m.ParentId)
                .ThenBy(m => m.Order)
                .ThenBy(m => m.Name)
                .Select(m => _mapper.Map<MenuDto>(m)));
    }

    public async Task<IDataResult<IEnumerable<MenuDto>>> GetMenusForUserAsync(int userId, int? activeRoleId)
    {
        var rolePermissionIds = new HashSet<int>();

        // ActiveRoleId yoksa veya token'daki rol artık kullanıcıya ait/değil aktif değilse,
        // rol gerektiren menüler gösterilmez. Böylece menu görünürlüğü ile API yetki denetimi aynı kalır.
        if (activeRoleId.HasValue && await _userRoles.AnyAsync(ur =>
                ur.UserId == userId &&
                ur.RoleId == activeRoleId.Value &&
                ur.User.IsActive &&
                ur.Role.IsActive))
        {
            var rolePermissions = await _rolePermissions.GetAllAsync(
                rp => rp.RoleId == activeRoleId.Value && rp.Permission.IsActive);
            rolePermissionIds = rolePermissions.Select(rp => rp.PermissionId).ToHashSet();
        }

        var menus = await _menus.GetAllAsync(
            predicate: m => m.PermissionId == null ||
                            (m.Permission!.IsActive && rolePermissionIds.Contains(m.PermissionId.Value)),
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
        var parentValidation = await ValidateParentAsync(dto.ParentId, null);
        if (!parentValidation.Success)
            return DataResult<MenuDto>.ErrorDataResult(parentValidation.Message, parentValidation.StatusCode);

        var menu = new Menu
        {
            Name = dto.Name.Trim(),
            Url = dto.Url?.Trim(),
            Icon = dto.Icon?.Trim(),
            ParentId = dto.ParentId,
            Order = dto.Order,
            PermissionId = dto.PermissionId,
            CreatedDate = _timeProvider.GetUtcNow().UtcDateTime,
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

        var parentValidation = await ValidateParentAsync(dto.ParentId, id);
        if (!parentValidation.Success)
            return parentValidation;

        menu.Name = dto.Name.Trim();
        menu.Url = dto.Url?.Trim();
        menu.Icon = dto.Icon?.Trim();
        menu.ParentId = dto.ParentId;
        menu.Order = dto.Order;
        menu.PermissionId = dto.PermissionId;
        menu.IsActive = dto.IsActive;
        menu.UpdatedDate = _timeProvider.GetUtcNow().UtcDateTime;

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
        menu.UpdatedDate = _timeProvider.GetUtcNow().UtcDateTime;
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

    private async Task<IResult> ValidateParentAsync(int? parentId, int? menuId)
    {
        if (!parentId.HasValue)
            return Result.Ok();

        var visitedMenuIds = new HashSet<int>();
        var currentParentId = parentId;

        while (currentParentId.HasValue)
        {
            if (menuId == currentParentId || !visitedMenuIds.Add(currentParentId.Value))
                return Result.BadRequest(Messages.General.HierarchyCycleNotAllowed);

            var parent = await _menus.GetByIdAsync(currentParentId.Value);
            if (parent == null)
                return Result.BadRequest(Messages.General.ParentNotFoundOrInactive);

            currentParentId = parent.ParentId;
        }

        return Result.Ok();
    }
}
