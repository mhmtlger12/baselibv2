using System.Linq.Expressions;

namespace Baselib.Core.Interfaces;

// Composite-key relations use IRepository<T>, which does not assume an int Id.
public interface IEntityRepository<T> : IRepository<T> where T : class, IEntity
{
    Task<T?> GetByIdAsync(int id, bool ignoreQueryFilters = false,
        CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includes);
    Task<T?> GetByIdAsync(int id, Func<IQueryable<T>, IQueryable<T>> include,
        bool ignoreQueryFilters = false, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
