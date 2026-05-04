using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Baselib.Core.Interfaces;
using Baselib.Data.Interfaces;
using Baselib.Entities;
using Microsoft.EntityFrameworkCore;

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

    public async Task<IEnumerable<RecycleBinItemDto>> GetAllDeletedItemsAsync()
    {
        var items = new List<RecycleBinItemDto>();

        var deletedUsers = await _users.Query().IgnoreQueryFilters().Where(u => !u.IsActive).ToListAsync();
        items.AddRange(deletedUsers.Select(u => new RecycleBinItemDto { Id = u.Id, Type = "Kullanıcı", Name = u.Username, DeletedDate = u.UpdatedDate }));

        var deletedRoles = await _roles.Query().IgnoreQueryFilters().Where(r => !r.IsActive).ToListAsync();
        items.AddRange(deletedRoles.Select(r => new RecycleBinItemDto { Id = r.Id, Type = "Rol", Name = r.Name, DeletedDate = r.UpdatedDate }));

        var deletedDepts = await _departments.Query().IgnoreQueryFilters().Where(d => !d.IsActive).ToListAsync();
        items.AddRange(deletedDepts.Select(d => new RecycleBinItemDto { Id = d.Id, Type = "Departman", Name = d.Name, DeletedDate = d.UpdatedDate }));

        return items.OrderByDescending(i => i.DeletedDate);
    }

    public async Task RestoreAsync(string type, int id)
    {
        BaseEntity? entity = type switch
        {
            "Kullanıcı" => await _users.Query().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id),
            "Rol" => await _roles.Query().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id),
            "Departman" => await _departments.Query().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id),
            _ => throw new ArgumentException("Geçersiz tür")
        };

        if (entity == null)
            throw new KeyNotFoundException("Kayıt bulunamadı");

        entity.IsActive = true;
        entity.UpdatedDate = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();
    }
}
