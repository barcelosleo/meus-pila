using System.Linq.Expressions;

namespace MeusPila.Application.Interfaces;

public interface IRepository<TEntity>
    where TEntity : class
{
    public Task<TEntity> GetByIdAsync(Guid id);
    public Task<IEnumerable<TEntity>> GetAllAsync();
    public Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate);
    public Task<IEnumerable<TEntity>> GetAllAsync(int pageNumber, int pageSize);
    public Task<IEnumerable<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>> predicate,
        int pageNumber,
        int pageSize
    );
    public Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate);
    public Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate);
    public Task<int> CountAsync();
    public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
    public Task<TEntity> AddAsync(TEntity entity);
    public Task<IEnumerable<TEntity>> AddAsync(IEnumerable<TEntity> entities);
    public Task<TEntity> UpdateAsync(TEntity entity);
    public Task<IEnumerable<TEntity>> UpdateAsync(IEnumerable<TEntity> entities);
    public Task DeleteAsync(TEntity entity);
    public Task DeleteAsync(IEnumerable<TEntity> entities);
}
