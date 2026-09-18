using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Baselib.Core.Interfaces;
using Baselib.Entities;

namespace Baselib.Data.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _dbSet;
        if (asNoTracking)
            query = query.AsNoTracking();

        return await query.ToListAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>>? predicate = null,
        bool ignoreQueryFilters = false,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;
        if (ignoreQueryFilters)
            query = query.IgnoreQueryFilters();
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        if (predicate != null)
        {
            query = query.Where(predicate);
        }
        if (asNoTracking)
            query = query.AsNoTracking();

        return await query.ToListAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>>? predicate,
        Func<IQueryable<T>, IQueryable<T>>? include = null,
        bool ignoreQueryFilters = false,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _dbSet;
        if (ignoreQueryFilters)
            query = query.IgnoreQueryFilters();
        if (include != null)
            query = include(query);
        if (predicate != null)
        {
            query = query.Where(predicate);
        }
        if (asNoTracking)
            query = query.AsNoTracking();

        return await query.ToListAsync(cancellationToken);
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes)
    {
        return await GetByIdAsync(id, ignoreQueryFilters: false, includes);
    }

    public virtual async Task<T?> GetByIdAsync(int id, bool ignoreQueryFilters, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;
        if (ignoreQueryFilters)
            query = query.IgnoreQueryFilters();
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
    }

    public virtual async Task<T?> GetByIdAsync(int id, Func<IQueryable<T>, IQueryable<T>>? include, bool ignoreQueryFilters = false)
    {
        IQueryable<T> query = _dbSet;
        if (ignoreQueryFilters)
            query = query.IgnoreQueryFilters();
        if (include != null)
            query = include(query);
        return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
    }

    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }

    public virtual async Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        bool ignoreQueryFilters = false,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;
        if (ignoreQueryFilters)
            query = query.IgnoreQueryFilters();
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        return await query.FirstOrDefaultAsync(predicate);
    }

    public virtual async Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IQueryable<T>>? include,
        bool ignoreQueryFilters = false)
    {
        IQueryable<T> query = _dbSet;
        if (ignoreQueryFilters)
            query = query.IgnoreQueryFilters();
        if (include != null)
            query = include(query);
        return await query.FirstOrDefaultAsync(predicate);
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public virtual async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    public virtual void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public virtual async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }

    public virtual void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }

    public virtual void RemoveRange(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, bool ignoreQueryFilters = false, CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _dbSet;
        if (ignoreQueryFilters)
            query = query.IgnoreQueryFilters();
        return predicate == null
            ? await query.CountAsync(cancellationToken)
            : await query.CountAsync(predicate, cancellationToken);
    }

    public virtual async Task<IReadOnlyList<TResult>> SelectAsync<TResult>(
        Func<IQueryable<T>, IQueryable<TResult>> queryBuilder,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(queryBuilder);

        return await queryBuilder(_dbSet.AsNoTracking()).ToListAsync(cancellationToken);
    }
}
