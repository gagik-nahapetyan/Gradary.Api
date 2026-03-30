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
    /// Retrieves the list of review models by book id.
    /// </summary>
    /// <param name="bookId">The id of the book.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>Task representing an asynchronous operation, wrapping the list of review models.</returns>
    Task<List<ReviewModel>> GetByBookIdAsync(int bookId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the review model by id.
    /// </summary>
    /// <param name="id">The id of the review.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>Task representing an asynchronous operation, wrapping the review model.</returns>
    Task<ReviewModel> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
