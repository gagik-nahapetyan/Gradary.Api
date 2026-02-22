using OnlineLibrary.Domain;
using OnlineLibrary.Domain.Models;

namespace OnlineLibrary.Application.Abstractions.Services;

/// <summary>
/// Represents an abstraction of a book service.
/// </summary>
public interface IBookService
{
    /// <summary>
    /// Retrieves the list of book models.
    /// </summary>
    /// <returns>Task representing an asynchronous operation, wrapping the list of book models.</returns>
    Task<List<BookModel>> GetAsync();

    /// <summary>
    /// Retrieves the book model by id.
    /// </summary>
    /// <param name="id">The id of the book.</param>
    /// <returns>Task representing an asynchronous operation, wrapping the book model.</returns>
    Task<BookModel> GetByIdAsync(int id);

    /// <summary>
    /// Creates a book model.
    /// </summary>
    /// <param name="model">The book model provided.</param>
    /// <returns>Task representing an asynchronous operation, wrapping the book model created.</returns>
    Task<BookModel> CreateAsync(BookModel model);

    /// <summary>
    /// Updates a book model.
    /// </summary>
    /// <param name="model">The book model provided.</param>
    Task UpdateAsync(BookModel model);
}
