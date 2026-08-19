using System.Linq.Expressions;
using MeusPila.Application.Interfaces;
using MeusPila.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MeusPila.Infrastructure.Repositories;

public class BaseRepository<TEntity> : IRepository<TEntity>
    where TEntity : class
{
    protected readonly ApplicationDbContext _dbContext;
    protected readonly DbSet<TEntity> _dbSet;

    public BaseRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<TEntity>();
    }

    public Task<TEntity?> GetByIdAsync(Guid id)
    {
        return _dbSet.FindAsync(id).AsTask();
    }

    public Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return Task.FromResult(_dbSet.AsEnumerable());
    }

    public Task<IEnumerable<TEntity>> GetAllAsync(IEnumerable<IQueryOrdering> orderings)
    {
        if (orderings.Any())
        {
            IQueryable<TEntity> query = _dbSet;
            foreach (var ordering in orderings)
            {
                var propertyInfo = typeof(TEntity).GetProperty(ordering.OrderingPropertyName);
                if (propertyInfo != null)
                {
                    query = ordering.Descending
                        ? query.OrderByDescending(e => propertyInfo.GetValue(e, null))
                        : query.OrderBy(e => propertyInfo.GetValue(e, null));
                }
            }
            return Task.FromResult(query.AsEnumerable());
        }
        return Task.FromResult(_dbSet.AsEnumerable());
    }

    public Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return Task.FromResult(_dbSet.Where(predicate).AsEnumerable());
    }

    public Task<IEnumerable<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>> predicate,
        IEnumerable<IQueryOrdering> orderings
    )
    {
        IQueryable<TEntity> query = _dbSet.Where(predicate);
        if (orderings.Any())
        {
            foreach (var ordering in orderings)
            {
                var propertyInfo = typeof(TEntity).GetProperty(ordering.OrderingPropertyName);
                if (propertyInfo != null)
                {
                    query = ordering.Descending
                        ? query.OrderByDescending(e => propertyInfo.GetValue(e, null))
                        : query.OrderBy(e => propertyInfo.GetValue(e, null));
                }
            }
        }
        return Task.FromResult(query.AsEnumerable());
    }

    public Task<IEnumerable<TEntity>> GetAllAsync(int pageNumber, int pageSize)
    {
        IQueryable<TEntity> query = _dbSet;
        return Task.FromResult(query.Skip((pageNumber - 1) * pageSize).Take(pageSize).AsEnumerable());
    }

    public Task<IEnumerable<TEntity>> GetAllAsync(int pageNumber, int pageSize, IEnumerable<IQueryOrdering> orderings)
    {
        IQueryable<TEntity> query = _dbSet;
        if (orderings.Any())
        {
            foreach (var ordering in orderings)
            {
                var propertyInfo = typeof(TEntity).GetProperty(ordering.OrderingPropertyName);
                if (propertyInfo != null)
                {
                    query = ordering.Descending
                        ? query.OrderByDescending(e => propertyInfo.GetValue(e, null))
                        : query.OrderBy(e => propertyInfo.GetValue(e, null));
                }
            }
        }
        return Task.FromResult(query.Skip((pageNumber - 1) * pageSize).Take(pageSize).AsEnumerable());
    }

    public Task<IEnumerable<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>> predicate,
        int pageNumber,
        int pageSize
    )
    {
        IQueryable<TEntity> query = _dbSet.Where(predicate);
        return Task.FromResult(query.Skip((pageNumber - 1) * pageSize).Take(pageSize).AsEnumerable());
    }

    public Task<IEnumerable<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>> predicate,
        int pageNumber,
        int pageSize,
        IEnumerable<IQueryOrdering> orderings
    )
    {
        IQueryable<TEntity> query = _dbSet.Where(predicate);
        if (orderings.Any())
        {
            foreach (var ordering in orderings)
            {
                var propertyInfo = typeof(TEntity).GetProperty(ordering.OrderingPropertyName);
                if (propertyInfo != null)
                {
                    query = ordering.Descending
                        ? query.OrderByDescending(e => propertyInfo.GetValue(e, null))
                        : query.OrderBy(e => propertyInfo.GetValue(e, null));
                }
            }
        }
        return Task.FromResult(query.Skip((pageNumber - 1) * pageSize).Take(pageSize).AsEnumerable());
    }

    public Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return _dbSet.FirstOrDefaultAsync(predicate);
    }

    public Task<int> CountAsync()
    {
        return _dbSet.CountAsync();
    }

    public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return _dbSet.CountAsync(predicate);
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        _dbSet.Add(entity);
        await _dbContext.SaveChangesAsync();
        return entity;
    }

    public async Task<IEnumerable<TEntity>> AddAsync(IEnumerable<TEntity> entities)
    {
        _dbSet.AddRange(entities);
        await _dbContext.SaveChangesAsync();
        return entities;
    }

    public Task<TEntity> UpdateAsync(TEntity entity)
    {
        _dbSet.Update(entity);
        return _dbContext.SaveChangesAsync().ContinueWith(_ => entity);
    }

    public Task<IEnumerable<TEntity>> UpdateAsync(IEnumerable<TEntity> entities)
    {
        _dbSet.UpdateRange(entities);
        return _dbContext.SaveChangesAsync().ContinueWith(_ => entities);
    }

    public Task DeleteAsync(TEntity entity)
    {
        _dbSet.Remove(entity);
        return _dbContext.SaveChangesAsync();
    }

    public Task DeleteAsync(Guid id)
    {
        var entity = _dbSet.Find(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
        return _dbContext.SaveChangesAsync();
    }

    public Task DeleteAsync(IEnumerable<TEntity> entities)
    {
        _dbSet.RemoveRange(entities);
        return _dbContext.SaveChangesAsync();
    }
}
