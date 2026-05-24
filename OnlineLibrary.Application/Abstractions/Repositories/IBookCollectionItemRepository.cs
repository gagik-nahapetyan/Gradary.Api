using OnlineLibrary.Domain.Entities;

namespace OnlineLibrary.Application.Abstractions.Repositories;

/// <summary>
/// Represents the <see cref="IBookCollectionItemRepository"/> interface.
/// </summary>
public interface IBookCollectionItemRepository : IRepository<BookCollectionItem>
{
    /// <summary>
    /// Gets a specific item by collection id and book id.
    /// </summary>
    Task<BookCollectionItem?> GetByCollectionAndBookAsync(int collectionId, int bookId, CancellationToken cancellationToken = default, bool tracked = false);
}
