using System.Linq.Expressions;

namespace Baselib.Core.Interfaces;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>>? predicate = null,
        bool ignoreQueryFilters = false,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>>? predicate,
        Func<IQueryable<T>, IQueryable<T>>? include,
        bool ignoreQueryFilters = false,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default);

    Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        bool ignoreQueryFilters = false,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes);
    Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IQueryable<T>>? include,
        bool ignoreQueryFilters = false, CancellationToken cancellationToken = default);

    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
    Task<bool> AnyAsync(
        Expression<Func<T, bool>> predicate,
        bool ignoreQueryFilters = false,
        CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, bool ignoreQueryFilters = false, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TResult>> SelectAsync<TResult>(
        Func<IQueryable<T>, IQueryable<TResult>> queryBuilder,
        CancellationToken cancellationToken = default);
}
