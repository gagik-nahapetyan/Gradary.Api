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
    Task<List<AuthorModel>> GetAsync();

    /// <summary>
    /// Retrieves the author model by id.
    /// </summary>
    /// <param name="id">The id of the author.</param>
    /// <returns>Task representing an asynchronous operation, wrapping the author model.</returns>
    Task<AuthorModel> GetByIdAsync(int id);

    /// <summary>
    /// Creates an author model.
    /// </summary>
    /// <param name="model">The author model provided.</param>
    /// <returns>Task representing an asynchronous operation, wrapping the author model created.</returns>
    Task<AuthorModel> CreateAsync(AuthorModel model);

    /// <summary>
    /// Updates an author model.
    /// </summary>
    /// <param name="model">The author model provided.</param>
    Task UpdateAsync(AuthorModel model);
}

