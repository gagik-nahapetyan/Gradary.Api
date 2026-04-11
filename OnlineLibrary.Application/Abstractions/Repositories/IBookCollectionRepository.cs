using OnlineLibrary.Domain.Entities;

namespace OnlineLibrary.Application.Abstractions.Repositories;

/// <summary>
/// Represents the <see cref="IBookCollectionRepository"/> interface.
/// </summary>
public interface IBookCollectionRepository : IRepository<BookCollection>
{
    /// <summary>
    /// Gets all collections belonging to a user, including their items.
    /// </summary>
    Task<IEnumerable<BookCollection>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a collection by id including its items.
    /// </summary>
    Task<BookCollection?> GetByIdWithItemsAsync(int id, CancellationToken cancellationToken = default);
}
