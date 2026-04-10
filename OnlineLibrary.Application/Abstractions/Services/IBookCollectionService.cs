using OnlineLibrary.Domain.Models;

namespace OnlineLibrary.Application.Abstractions.Services;

/// <summary>
/// Represents an abstraction of a book collection service.
/// </summary>
public interface IBookCollectionService
{
    /// <summary>Creates a new collection for the given user.</summary>
    Task<BookCollectionModel> CreateAsync(BookCollectionModel model, CancellationToken cancellationToken = default);

    /// <summary>Updates the name, description, or status of a collection.</summary>
    Task<BookCollectionModel> UpdateAsync(BookCollectionModel model, int callerId, CancellationToken cancellationToken = default);

    /// <summary>Gets a single collection with its items. Enforces ownership.</summary>
    Task<BookCollectionModel> GetByIdAsync(int id, int callerId, CancellationToken cancellationToken = default);

    /// <summary>Gets all collections for the caller.</summary>
    Task<List<BookCollectionModel>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>Adds a book to a collection.</summary>
    Task<BookCollectionItemModel> AddBookAsync(int collectionId, BookCollectionItemModel model, int callerId, CancellationToken cancellationToken = default);

    /// <summary>Updates the status or order of a book in a collection.</summary>
    Task<BookCollectionItemModel> UpdateBookAsync(int collectionId, BookCollectionItemModel model, int callerId, CancellationToken cancellationToken = default);

    /// <summary>Removes a book from a collection.</summary>
    Task RemoveBookAsync(int collectionId, int bookId, int callerId, CancellationToken cancellationToken = default);
}
