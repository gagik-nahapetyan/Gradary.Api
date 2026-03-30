using OnlineLibrary.Domain.Models;

namespace OnlineLibrary.Application.Abstractions.Services;

/// <summary>
/// Represents an abstraction of an author service.
/// </summary>
public interface IAuthorService
{
    /// <summary>
    /// Retrieves the list of author models.
    /// </summary>
    /// <returns>Task representing an asynchronous operation, wrapping the list of author models.</returns>
    Task<List<AuthorModel>> GetAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the author model by id.
    /// </summary>
    /// <param name="id">The id of the author.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>Task representing an asynchronous operation, wrapping the author model.</returns>
    Task<AuthorModel> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates an author model.
    /// </summary>
    /// <param name="model">The author model provided.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>Task representing an asynchronous operation, wrapping the author model created.</returns>
    Task<AuthorModel> CreateAsync(AuthorModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an author model.
    /// </summary>
    /// <param name="model">The author model provided.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    Task UpdateAsync(AuthorModel model, CancellationToken cancellationToken = default);
}

