using System.Linq.Expressions;

namespace MeusPila.Application.Interfaces;

/// <summary>
/// Represents a generic repository interface for performing CRUD operations on entities of type TEntity.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public interface IRepository<TEntity>
    where TEntity : class
{
    /// <summary>
    /// Gets an entity by its unique identifier asynchronously.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>The entity with the specified identifier. Null if not found.</returns>
    public Task<TEntity> GetByIdAsync(Guid id);

    /// <summary>
    /// Gets all entities asynchronously.
    /// </summary>
    /// <returns>A list of all entities.</returns>
    public Task<IEnumerable<TEntity>> GetAllAsync();

    /// <summary>
    /// Gets all entities that match the specified predicate asynchronously.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>A list of entities that match the specified predicate.</returns>
    public Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Gets a paginated list of all entities asynchronously.
    /// </summary>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <returns>A paginated list of all entities.</returns>
    public Task<IEnumerable<TEntity>> GetAllAsync(int pageNumber, int pageSize);

    /// <summary>
    /// Gets a paginated list of all entities that match the specified predicate asynchronously.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <returns>A paginated list of entities that match the specified predicate.</returns>
    public Task<IEnumerable<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>> predicate,
        int pageNumber,
        int pageSize
    );

    /// <summary>
    /// Finds a single entity that matches the specified predicate asynchronously.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>The entity that matches the specified predicate. Null if not found.</returns>
    public Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Finds all entities that match the specified predicate asynchronously.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>A list of entities that match the specified predicate.</returns>
    public Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Counts the total number of entities asynchronously.
    /// </summary>
    /// <returns>The total number of entities.</returns>
    public Task<int> CountAsync();

    /// <summary>
    /// Counts the number of entities that match the specified predicate asynchronously.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>The number of entities that match the specified predicate.</returns>
    public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Adds a new entity to the repository asynchronously.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <returns>The added entity.</returns>
    public Task<TEntity> AddAsync(TEntity entity);

    /// <summary>
    /// Adds multiple new entities to the repository asynchronously.
    /// </summary>
    /// <param name="entities">The entities to add.</param>
    /// <returns>The added entities.</returns>
    public Task<IEnumerable<TEntity>> AddAsync(IEnumerable<TEntity> entities);

    /// <summary>
    /// Updates an existing entity in the repository asynchronously.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <returns>The updated entity.</returns>
    public Task<TEntity> UpdateAsync(TEntity entity);

    /// <summary>
    /// Updates multiple existing entities in the repository asynchronously.
    /// </summary>
    /// <param name="entities">The entities to update.</param>
    /// <returns>The updated entities.</returns>
    public Task<IEnumerable<TEntity>> UpdateAsync(IEnumerable<TEntity> entities);

    /// <summary>
    /// Deletes an entity from the repository asynchronously.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    public Task DeleteAsync(TEntity entity);

    /// <summary>
    /// Deletes multiple entities from the repository asynchronously.
    /// </summary>
    /// <param name="entities">The entities to delete.</param>
    public Task DeleteAsync(IEnumerable<TEntity> entities);
}
