using Baselib.Business.DTOs;
using Baselib.Business.Content;
using Baselib.Business.Interfaces;
using Baselib.Core.Enums;
using Baselib.Core.Interfaces;
using Baselib.Core.Messages;
using Baselib.Core.Results;
using Baselib.Entities;

namespace Baselib.Business.Services;

public sealed class RecycleBinService(
    IEntityRepository<User> users,
    IEntityRepository<Role> roles,
    IEntityRepository<Department> departments,
    IEntityRepository<Menu> menus,
    IEntityRepository<Permission> permissions,
    IEntityRepository<Slider> sliders,
    IEntityRepository<JobListing> jobs,
    IEntityRepository<Institution> institutions,
    IEnumerable<IContentModule> contentModules,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IRecycleBinService
{
    public async Task<IDataResult<IEnumerable<RecycleBinItemDto>>> GetAllDeletedItemsAsync(CancellationToken cancellationToken = default)
    {
        // These repositories share one DbContext: execute queries sequentially.
        var items = new List<RecycleBinItemDto>();
        items.AddRange(await ReadAsync(users, RecycleBinType.User, item => item.Username, cancellationToken));
        items.AddRange(await ReadAsync(roles, RecycleBinType.Role, item => item.Name, cancellationToken));
        items.AddRange(await ReadAsync(departments, RecycleBinType.Department, item => item.Name, cancellationToken));
        items.AddRange(await ReadAsync(menus, RecycleBinType.Menu, item => item.Name, cancellationToken));
        items.AddRange(await ReadAsync(permissions, RecycleBinType.Permission, item => item.Name, cancellationToken));
        items.AddRange(await ReadAsync(sliders, RecycleBinType.Slider, item => item.Title, cancellationToken));
        items.AddRange(await ReadAsync(jobs, RecycleBinType.JobListing, item => item.Institution + " — " + item.Summary, cancellationToken));
        items.AddRange(await ReadAsync(institutions, RecycleBinType.Institution, item => item.Name, cancellationToken));
        foreach (var module in contentModules) items.AddRange(await module.DeletedAsync(cancellationToken));
        return DataResult<IEnumerable<RecycleBinItemDto>>.Ok(items.OrderByDescending(item => item.DeletedDate));
    }

    public async Task<IResult> RestoreAsync(string type, int id, CancellationToken cancellationToken = default)
    {
        if (type.StartsWith("Content:", StringComparison.Ordinal))
        {
            var module = contentModules.FirstOrDefault(x => x.Slug == type[8..]);
            return module is null ? Result.NotFound() : await module.RestoreAsync(id, cancellationToken);
        }
        if (!RecycleBinTypeExtensions.TryParse(type, out var binType))
            return Result.BadRequest(Messages.RecycleBin.InvalidType);
        return await RestoreAsync(binType, id, cancellationToken);
    }

    public async Task<IResult> RestoreAsync(RecycleBinType type, int id, CancellationToken cancellationToken = default)
    {
        SoftDeleteEntity? entity = type switch
        {
            RecycleBinType.User => await users.GetByIdAsync(id, ignoreQueryFilters: true, cancellationToken: cancellationToken),
            RecycleBinType.Role => await roles.GetByIdAsync(id, ignoreQueryFilters: true, cancellationToken: cancellationToken),
            RecycleBinType.Department => await departments.GetByIdAsync(id, ignoreQueryFilters: true, cancellationToken: cancellationToken),
            RecycleBinType.Menu => await menus.GetByIdAsync(id, ignoreQueryFilters: true, cancellationToken: cancellationToken),
            RecycleBinType.Permission => await permissions.GetByIdAsync(id, ignoreQueryFilters: true, cancellationToken: cancellationToken),
            RecycleBinType.Slider => await sliders.GetByIdAsync(id, ignoreQueryFilters: true, cancellationToken: cancellationToken),
            RecycleBinType.JobListing => await jobs.GetByIdAsync(id, ignoreQueryFilters: true, cancellationToken: cancellationToken),
            RecycleBinType.Institution => await institutions.GetByIdAsync(id, ignoreQueryFilters: true, cancellationToken: cancellationToken),
            _ => null
        };
        if (entity is null || !entity.IsDeleted)
            return Result.NotFound(Messages.General.NotFound);

        // Restoring never grants access or republishes content. Activation uses normal module permissions.
        entity.IsDeleted = false;
        entity.IsActive = false;
        entity.UpdatedDate = timeProvider.GetUtcNow().UtcDateTime;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok("Kayıt pasif olarak geri yüklendi.");
    }

    private static async Task<IEnumerable<RecycleBinItemDto>> ReadAsync<T>(
        IEntityRepository<T> repository, RecycleBinType type, Func<T, string> name,
        CancellationToken cancellationToken) where T : SoftDeleteEntity
    {
        var items = await repository.GetAllAsync(item => item.IsDeleted,
            ignoreQueryFilters: true, asNoTracking: true, cancellationToken: cancellationToken);
        return items.Select(item => new RecycleBinItemDto
        {
            Id = item.Id, Type = type.ToString(), TypeName = type.GetDisplayName(),
            Name = name(item), DeletedDate = item.UpdatedDate
        });
    }
}
