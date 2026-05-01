using OnlineLibrary.Domain.Enums;
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
    /// Retrieves a paginated list of book models.
    /// </summary>
    /// <param name="page">The page number, starting at 1.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="orderBy">The field to sort by: "title", "created". Defaults to "title".</param>
    /// <param name="orderType">The sort direction.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>Task representing an asynchronous operation, wrapping the paged list of book models.</returns>
    Task<PagedList<BookModel>> GetAsync(int page, int pageSize, string? orderBy = null, OrderType orderType = OrderType.Asc, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a paginated list of books belonging to the given category.
    /// </summary>
    /// <param name="categoryId">The id of the category.</param>
    /// <param name="page">The page number, starting at 1.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="orderBy">The field to sort by: "title", "created". Defaults to "title".</param>
    /// <param name="orderType">The sort direction.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>Task representing an asynchronous operation, wrapping the paged list of book models.</returns>
    Task<PagedList<BookModel>> GetByCategoryIdAsync(int categoryId, int page, int pageSize, string? orderBy = null, OrderType orderType = OrderType.Asc, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads the file for a book.
    /// </summary>
    /// <param name="id">The id of the book.</param>
    /// <param name="openStream">A delegate that opens the file stream when invoked.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    Task UploadFileAsync(int id, Func<Stream> openStream, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads or replaces the cover image for a book.
    /// </summary>
    /// <param name="id">The id of the book.</param>
    /// <param name="contentType">The MIME type of the image (e.g. image/jpeg).</param>
    /// <param name="openStream">A delegate that opens the image stream when invoked.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    Task UploadImageAsync(int id, string contentType, Func<Stream> openStream, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads the cover image for a book.
    /// </summary>
    /// <param name="id">The id of the book.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>The image stream and its MIME content type.</returns>
    Task<(Stream stream, string contentType)> GetImageAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Soft-deletes a book by id.
    /// </summary>
    /// <param name="id">The id of the book.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
