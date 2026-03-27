using System.Linq.Expressions;

namespace OnlineLibrary.Application.Abstractions.Repositories;

/// <summary>
/// Represents an abstraction of the Repository class.
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public interface IRepository<TEntity>
    where TEntity : class
{
    /// <summary>
    /// Inserts the entity supplied into the database.
    /// </summary>
    /// <param name="entity">The entity to insert.</param>
    /// <returns>The task representing the asynchronous operation wrapped the entity.</returns>
    Task<TEntity> InsertAsync(TEntity entity);

    /// <summary>
    /// Updates the entity supplied.
    /// </summary>
    /// <param name="entity">The updating entity.</param>
    void Update(TEntity entity);

    /// <summary>
    /// Retrieves the entity by its id.
    /// </summary>
    /// <param name="id">The id supplied.</param>
    /// <returns>The task representing the asynchronous operation wrapped the entity.</returns>
    Task<TEntity?> GetByIdAsync(int id);

    /// <summary>
    /// Retrieves the list of all entities.
    /// </summary>
    /// <returns>The task representing the asynchronous operation wrapped the list of entities.</returns>
    Task<IEnumerable<TEntity>> GetAllAsync();

    /// <summary>
    /// Filters the entities by the condition supplied.
    /// </summary>
    /// <param name="predicate">The predicate supplied.</param>
    /// <returns>The task representing the asynchronous operation wrapped the list of filtered entities.</returns>
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Determines if any entity satisfying the provided condition exists.
    /// </summary>
    /// <param name="predicate">The predicate supplied.</param>
    /// <returns>The task representing the asynchronous operation wrapped the boolean flag.</returns>
    Task<bool> ExistAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Removes the record from the database.
    /// </summary>
    /// <param name="entity">The entity to remove the record for.</param>
    void Delete(TEntity entity);

    /// <summary>
    /// Saves the changes made in the database.
    /// </summary>
    /// <returns>The task representing the asynchronous operation.</returns>
    Task SaveChangesAsync();
}
