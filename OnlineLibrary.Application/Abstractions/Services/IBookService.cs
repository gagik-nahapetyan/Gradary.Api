using OnlineLibrary.Domain.Models;

namespace OnlineLibrary.Application.Abstractions.Services;

/// <summary>
/// Represents an abstraction of a book service.
/// </summary>
public interface IBookService
{
    /// <summary>
    /// Creates a book model.
    /// </summary>
    /// <param name="model">The book model provided.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Task representing an asynchronous operation, wrapping the book model created.</returns>
    Task<BookModel> CreateAsync(BookModel model, CancellationToken cancellationToken = default);


    /// <summary>
    /// Updates a book model.
    /// </summary>
    /// <param name="model">The book model provided.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Task representing an asynchronous operation.</returns>
    Task UpdateAsync(BookModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the book model by id.
    /// </summary>
    /// <param name="id">The id of the book.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>Task representing an asynchronous operation, wrapping the book model.</returns>
    Task<BookModel> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the list of book models.
    /// </summary>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>Task representing an asynchronous operation, wrapping the list of book models.</returns>
    Task<List<BookModel>> GetAsync(CancellationToken cancellationToken = default);
}
