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
    Task<List<CategoryModel>> GetAsync();

    /// <summary>
    /// Retrieves the category model by id.
    /// </summary>
    /// <param name="id">The id of the category.</param>
    /// <returns>Task representing an asynchronous operation, wrapping the category model.</returns>
    Task<CategoryModel> GetByIdAsync(int id);

    /// <summary>
    /// Creates a category model.
    /// </summary>
    /// <param name="model">The category model provided.</param>
    /// <returns>Task representing an asynchronous operation, wrapping the category model created.</returns>
    Task<CategoryModel> CreateAsync(CategoryModel model);

    /// <summary>
    /// Updates a category model.
    /// </summary>
    /// <param name="model">The category model provided.</param>
    Task UpdateAsync(CategoryModel model);
}

