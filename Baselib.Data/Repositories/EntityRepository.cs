using System.Linq.Expressions;
using Baselib.Core.Interfaces;

namespace Baselib.Data.Repositories;

public class EntityRepository<T>(AppDbContext context) : Repository<T>(context), IEntityRepository<T>
    where T : class, IEntity
{
    public Task<T?> GetByIdAsync(int id, bool ignoreQueryFilters = false,
        CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includes) =>
        FirstOrDefaultAsync(entity => entity.Id == id, ignoreQueryFilters, cancellationToken, includes);

    public Task<T?> GetByIdAsync(int id, Func<IQueryable<T>, IQueryable<T>> include,
        bool ignoreQueryFilters = false, CancellationToken cancellationToken = default) =>
        FirstOrDefaultAsync(entity => entity.Id == id, include, ignoreQueryFilters, cancellationToken);

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken: cancellationToken);
        if (entity is not null) Remove(entity);
    }
}
