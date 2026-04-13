using System.Linq.Expressions;
using OnlineLibrary.Domain.Entities;
using OnlineLibrary.Domain.Models;

namespace OnlineLibrary.Application.Abstractions.Repositories;

/// <summary>
/// Represents an abstraction of the Repository class.
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public interface IRepository<TEntity>
    where TEntity : EntityBase
{
    /// <summary>
    /// Inserts the entity supplied into the database.
    /// </summary>
    /// <param name="entity">The entity to insert.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>The task representing the asynchronous operation wrapped the entity.</returns>
    Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the entity supplied.
    /// </summary>
    /// <param name="entity">The updating entity.</param>
    void Update(TEntity entity);

    /// <summary>
    /// Retrieves the entity by its id.
    /// </summary>
    /// <param name="id">The id supplied.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <param name="includeDeleted">When true, soft-deleted entities are included.</param>
    /// <returns>The task representing the asynchronous operation wrapped the entity.</returns>
    Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default, bool includeDeleted = false);

    /// <summary>
    /// Retrieves the list of all entities.
    /// </summary>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <param name="includeDeleted">When true, soft-deleted entities are included.</param>
    /// <returns>The task representing the asynchronous operation wrapped the list of entities.</returns>
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default, bool includeDeleted = false);

    /// <summary>
    /// Filters the entities by the condition supplied.
    /// </summary>
    /// <param name="predicate">The predicate supplied.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <param name="includeDeleted">When true, soft-deleted entities are included.</param>
    /// <returns>The task representing the asynchronous operation wrapped the list of filtered entities.</returns>
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default, bool includeDeleted = false);

    /// <summary>
    /// Determines if any entity satisfying the provided condition exists.
    /// </summary>
    /// <param name="predicate">The predicate supplied.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <param name="includeDeleted">When true, soft-deleted entities are included.</param>
    /// <returns>The task representing the asynchronous operation wrapped the boolean flag.</returns>
    Task<bool> ExistAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default, bool includeDeleted = false);

    /// <summary>
    /// Returns the count of entities satisfying the provided condition.
    /// </summary>
    /// <param name="predicate">The predicate supplied.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <param name="includeDeleted">When true, soft-deleted entities are included.</param>
    /// <returns>The task representing the asynchronous operation wrapped the count.</returns>
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default, bool includeDeleted = false);

    /// <summary>
    /// Retrieves a paginated list of all entities.
    /// </summary>
    /// <param name="page">The 1-based page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="orderBy">Optional ordering delegate applied before pagination.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <param name="includeDeleted">When true, soft-deleted entities are included.</param>
    /// <returns>The task representing the asynchronous operation wrapped the paged list of entities.</returns>
    Task<PagedList<TEntity>> GetPagedAsync(
        int page, 
        int pageSize, 
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, 
        CancellationToken cancellationToken = default, 
        bool includeDeleted = false);

    /// <summary>
    /// Retrieves a paginated list of entities matching the predicate.
    /// </summary>
    /// <param name="predicate">The predicate supplied.</param>
    /// <param name="page">The 1-based page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="orderBy">Optional ordering delegate applied before pagination.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <param name="includeDeleted">When true, soft-deleted entities are included.</param>
    /// <returns>The task representing the asynchronous operation wrapped the paged list of filtered entities.</returns>
    Task<PagedList<TEntity>> FindPagedAsync(
        Expression<Func<TEntity, bool>> predicate, 
        int page, 
        int pageSize, 
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, 
        CancellationToken cancellationToken = default, 
        bool includeDeleted = false);

    /// <summary>
    /// Removes the record from the database.
    /// </summary>
    /// <param name="entity">The entity to remove the record for.</param>
    void Delete(TEntity entity);

    /// <summary>
    /// Saves the changes made in the database.
    /// </summary>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>The task representing the asynchronous operation.</returns>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
