using System.Linq.Expressions;
using System.Reflection;
using MeusPila.Application.Interfaces;
using MeusPila.Domain.Interfaces;
using MeusPila.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MeusPila.Infrastructure.Repositories;

public class BaseRepository<TEntity> : IRepository<TEntity>
    where TEntity : class, IEntity
{
    protected readonly ApplicationDbContext _dbContext;
    protected readonly DbSet<TEntity> _dbSet;

    public BaseRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<TEntity>();
    }

    protected IQueryable<TEntity> ApplyOrdering(
        IQueryable<TEntity> query,
        PropertyInfo propertyInfo,
        bool descending,
        bool isFirst
    )
    {
        var parameter = Expression.Parameter(typeof(TEntity), "e");
        var propertyAccess = Expression.Property(parameter, propertyInfo);
        var lambda = Expression.Lambda(propertyAccess, parameter);

        string methodName = isFirst
            ? (descending ? nameof(Queryable.OrderByDescending) : nameof(Queryable.OrderBy))
            : (descending ? nameof(Queryable.ThenByDescending) : nameof(Queryable.ThenBy));

        var method = typeof(Queryable)
            .GetMethods()
            .First(m => m.Name == methodName && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(TEntity), propertyInfo.PropertyType);
        return (IQueryable<TEntity>)method.Invoke(null, [query, lambda])!;
    }

    protected virtual IQueryable<TEntity> GetDbSetRelations()
    {
        return _dbSet.AsQueryable();
    }

    public virtual Task<TEntity?> GetByIdAsync(Guid id)
    {
        return GetDbSetRelations().FirstOrDefaultAsync(e => e.Id == id);
    }

    public virtual Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return Task.FromResult(_dbSet.AsEnumerable());
    }

    public virtual Task<IEnumerable<TEntity>> GetAllAsync(IEnumerable<IQueryOrdering> orderings)
    {
        IQueryable<TEntity> query = GetDbSetRelations();
        bool isFirst = true;
        foreach (var ordering in orderings)
        {
            var propertyInfo = typeof(TEntity).GetProperty(ordering.OrderingPropertyName);
            if (propertyInfo != null)
            {
                query = ApplyOrdering(query, propertyInfo, ordering.Descending, isFirst);
                isFirst = false;
            }
        }
        return Task.FromResult(query.AsEnumerable());
    }

    public virtual Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return Task.FromResult(GetDbSetRelations().Where(predicate).AsEnumerable());
    }

    public virtual Task<IEnumerable<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>> predicate,
        IEnumerable<IQueryOrdering> orderings
    )
    {
        IQueryable<TEntity> query = GetDbSetRelations().Where(predicate);
        bool isFirst = true;
        foreach (var ordering in orderings)
        {
            var propertyInfo = typeof(TEntity).GetProperty(ordering.OrderingPropertyName);
            if (propertyInfo != null)
            {
                query = ApplyOrdering(query, propertyInfo, ordering.Descending, isFirst);
                isFirst = false;
            }
        }
        return Task.FromResult(query.AsEnumerable());
    }

    public virtual Task<IEnumerable<TEntity>> GetAllAsync(int pageNumber, int pageSize)
    {
        IQueryable<TEntity> query = GetDbSetRelations();
        return Task.FromResult(query.Skip((pageNumber - 1) * pageSize).Take(pageSize).AsEnumerable());
    }

    public virtual Task<IEnumerable<TEntity>> GetAllAsync(
        int pageNumber,
        int pageSize,
        IEnumerable<IQueryOrdering> orderings
    )
    {
        IQueryable<TEntity> query = GetDbSetRelations();
        bool isFirst = true;
        foreach (var ordering in orderings)
        {
            var propertyInfo = typeof(TEntity).GetProperty(ordering.OrderingPropertyName);
            if (propertyInfo != null)
            {
                query = ApplyOrdering(query, propertyInfo, ordering.Descending, isFirst);
                isFirst = false;
            }
        }
        return Task.FromResult(query.Skip((pageNumber - 1) * pageSize).Take(pageSize).AsEnumerable());
    }

    public virtual Task<IEnumerable<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>> predicate,
        int pageNumber,
        int pageSize
    )
    {
        IQueryable<TEntity> query = GetDbSetRelations().Where(predicate);
        return Task.FromResult(query.Skip((pageNumber - 1) * pageSize).Take(pageSize).AsEnumerable());
    }

    public virtual Task<IEnumerable<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>> predicate,
        int pageNumber,
        int pageSize,
        IEnumerable<IQueryOrdering> orderings
    )
    {
        IQueryable<TEntity> query = GetDbSetRelations().Where(predicate);

        bool isFirst = true;
        foreach (var ordering in orderings)
        {
            var propertyInfo = typeof(TEntity).GetProperty(ordering.OrderingPropertyName);
            if (propertyInfo == null)
                continue;

            query = ApplyOrdering(query, propertyInfo, ordering.Descending, isFirst);
            isFirst = false;
        }
        return Task.FromResult(query.Skip((pageNumber - 1) * pageSize).Take(pageSize).AsEnumerable());
    }

    public virtual Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return GetDbSetRelations().FirstOrDefaultAsync(predicate);
    }

    public virtual Task<int> CountAsync()
    {
        return GetDbSetRelations().CountAsync();
    }

    public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return GetDbSetRelations().CountAsync(predicate);
    }

    public virtual async Task<TEntity> AddAsync(TEntity entity)
    {
        _dbSet.Add(entity);
        await _dbContext.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<IEnumerable<TEntity>> AddAsync(IEnumerable<TEntity> entities)
    {
        _dbSet.AddRange(entities);
        await _dbContext.SaveChangesAsync();
        return entities;
    }

    public virtual Task<TEntity> UpdateAsync(TEntity entity)
    {
        _dbSet.Update(entity);
        return _dbContext.SaveChangesAsync().ContinueWith(_ => entity);
    }

    public virtual Task<IEnumerable<TEntity>> UpdateAsync(IEnumerable<TEntity> entities)
    {
        _dbSet.UpdateRange(entities);
        return _dbContext.SaveChangesAsync().ContinueWith(_ => entities);
    }

    public virtual Task DeleteAsync(TEntity entity)
    {
        _dbSet.Remove(entity);
        return _dbContext.SaveChangesAsync();
    }

    public virtual Task DeleteAsync(Guid id)
    {
        var entity = _dbSet.Find(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
        return _dbContext.SaveChangesAsync();
    }

    public virtual Task DeleteAsync(IEnumerable<TEntity> entities)
    {
        _dbSet.RemoveRange(entities);
        return _dbContext.SaveChangesAsync();
    }
}
