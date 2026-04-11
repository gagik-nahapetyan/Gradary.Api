using OnlineLibrary.Domain.Models;

namespace OnlineLibrary.Application.Abstractions.Services;

/// <summary>
/// Represents an abstraction of a category service.
/// </summary>
public interface ICategoryService
{
    /// <summary>
    /// Retrieves the list of category models.
    /// </summary>
    /// <returns>Task representing an asynchronous operation, wrapping the list of category models.</returns>
    Task<List<CategoryModel>> GetAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the category model by id.
    /// </summary>
    /// <param name="id">The id of the category.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>Task representing an asynchronous operation, wrapping the category model.</returns>
    Task<CategoryModel> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a category model.
    /// </summary>
    /// <param name="model">The category model provided.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>Task representing an asynchronous operation, wrapping the category model created.</returns>
    Task<CategoryModel> CreateAsync(CategoryModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a category model.
    /// </summary>
    /// <param name="model">The category model provided.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    Task UpdateAsync(CategoryModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Soft-deletes a category by id.
    /// </summary>
    /// <param name="id">The id of the category.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
