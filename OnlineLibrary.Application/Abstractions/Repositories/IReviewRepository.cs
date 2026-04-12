using OnlineLibrary.Domain.Entities;
using OnlineLibrary.Domain.Models;

namespace OnlineLibrary.Application.Abstractions.Repositories;

/// <summary>
/// Represents the <see cref="IReviewRepository"/> interface.
/// </summary>
public interface IReviewRepository : IRepository<Review>
{
    /// <summary>
    /// Gets a paginated list of reviews for the given book.
    /// </summary>
    /// <param name="bookId">The id of the book.</param>
    /// <param name="page">The page number, starting at 1.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    Task<PagedList<Review>> GetByBookIdPagedAsync(int bookId, int page, int pageSize, CancellationToken cancellationToken = default);
}
