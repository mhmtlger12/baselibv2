using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Baselib.Core.Interfaces;
using Baselib.Core.Messages;
using Baselib.Core.Results;
using Baselib.Entities;

namespace Baselib.Business.Services;

public class RecycleBinService : IRecycleBinService
{
    private readonly IRepository<User> _users;
    private readonly IRepository<Role> _roles;
    private readonly IRepository<Department> _departments;
    private readonly IUnitOfWork _unitOfWork;

    public RecycleBinService(
        IRepository<User> users,
        IRepository<Role> roles,
        IRepository<Department> departments,
        IUnitOfWork unitOfWork)
    {
        _users = users;
        _roles = roles;
        _departments = departments;
        _unitOfWork = unitOfWork;
    }

    public async Task<IDataResult<IEnumerable<RecycleBinItemDto>>> GetAllDeletedItemsAsync()
    {
        var items = new List<RecycleBinItemDto>();

        var deletedUsers = await _users.GetAllAsync(predicate: u => !u.IsActive, ignoreQueryFilters: true);
        items.AddRange(deletedUsers.Select(u => new RecycleBinItemDto { Id = u.Id, Type = "Kullanıcı", Name = u.Username, DeletedDate = u.UpdatedDate }));

        var deletedRoles = await _roles.GetAllAsync(predicate: r => !r.IsActive, ignoreQueryFilters: true);
        items.AddRange(deletedRoles.Select(r => new RecycleBinItemDto { Id = r.Id, Type = "Rol", Name = r.Name, DeletedDate = r.UpdatedDate }));

        var deletedDepts = await _departments.GetAllAsync(predicate: d => !d.IsActive, ignoreQueryFilters: true);
        items.AddRange(deletedDepts.Select(d => new RecycleBinItemDto { Id = d.Id, Type = "Departman", Name = d.Name, DeletedDate = d.UpdatedDate }));

        return DataResult<IEnumerable<RecycleBinItemDto>>.Ok(items.OrderByDescending(i => i.DeletedDate));
    }

    public async Task<IResult> RestoreAsync(string type, int id)
    {
        BaseEntity? entity;
        switch (type)
        {
            case "Kullanıcı":
                entity = await _users.FirstOrDefaultAsync(x => x.Id == id, ignoreQueryFilters: true);
                break;
            case "Rol":
                entity = await _roles.FirstOrDefaultAsync(x => x.Id == id, ignoreQueryFilters: true);
                break;
            case "Departman":
                entity = await _departments.FirstOrDefaultAsync(x => x.Id == id, ignoreQueryFilters: true);
                break;
            default:
                return Result.BadRequest(Messages.RecycleBin.InvalidType);
        }

        if (entity == null)
            return Result.NotFound(Messages.General.NotFound);

        entity.IsActive = true;
        entity.UpdatedDate = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();

        return Result.Ok(Messages.General.Updated);
    }
}
