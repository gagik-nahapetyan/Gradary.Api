using OnlineLibrary.Domain.Entities;

namespace OnlineLibrary.Application.Abstractions.Repositories;

/// <summary>
/// Represents the <see cref="IReviewRepository"/> interface.
/// </summary>
public interface IReviewRepository : IRepository<Review>
{
    /// <summary>
    /// Gets the reviews by book id.
    /// </summary>
    /// <param name="bookId">The id of the book.</param>
    /// <returns>The reviews by book id.</returns>
    Task<IEnumerable<Review>> GetByBookIdAsync(int bookId, CancellationToken cancellationToken = default);
}
