using OnlineLibrary.Domain.Enums;
using OnlineLibrary.Domain.Models;

namespace OnlineLibrary.Application.Abstractions.Services;

/// <summary>
/// Represents an abstraction of a review service.
/// </summary>
public interface IReviewService
{
    /// <summary>
    /// Creates a review model.
    /// </summary>
    /// <param name="model">The review model provided.</param>
    /// <returns>Task representing an asynchronous operation, wrapping the review model created.</returns>
    Task<ReviewModel> CreateAsync(ReviewModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a review model.
    /// </summary>
    /// <param name="model">The review model provided.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>Task representing an asynchronous operation, wrapping the review model updated.</returns>
    Task UpdateAsync(ReviewModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a paginated list of review models for the given book.
    /// </summary>
    /// <param name="bookId">The id of the book.</param>
    /// <param name="page">The page number, starting at 1.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="orderBy">The field to sort by: "rating", "created". Defaults to "created" descending.</param>
    /// <param name="orderType">The sort direction.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>Task representing an asynchronous operation, wrapping the paged list of review models.</returns>
    Task<PagedList<ReviewModel>> GetByBookIdAsync(int bookId, int page, int pageSize, string? orderBy = null, OrderType orderType = OrderType.Desc, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the review model by id.
    /// </summary>
    /// <param name="id">The id of the review.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>Task representing an asynchronous operation, wrapping the review model.</returns>
    Task<ReviewModel> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Soft-deletes a review by id. Only the owner may delete their review.
    /// </summary>
    /// <param name="id">The id of the review.</param>
    /// <param name="callerId">The id of the caller.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    Task DeleteAsync(int id, int callerId, CancellationToken cancellationToken = default);
}
