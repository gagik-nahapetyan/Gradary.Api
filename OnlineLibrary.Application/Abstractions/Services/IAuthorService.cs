using OnlineLibrary.Domain.Enums;
using OnlineLibrary.Domain.Models;

namespace OnlineLibrary.Application.Abstractions.Services;

/// <summary>
/// Represents an abstraction of an author service.
/// </summary>
public interface IAuthorService
{
    /// <summary>
    /// Retrieves a paginated list of author models.
    /// </summary>
    /// <param name="page">The page number, starting at 1.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="orderBy">The field to sort by: "name", "created". Defaults to "name".</param>
    /// <param name="orderType">The sort direction.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>Task representing an asynchronous operation, wrapping the paged list of author models.</returns>
    Task<PagedList<AuthorModel>> GetAsync(int page, int pageSize, string? orderBy = null, OrderType orderType = OrderType.Asc, CancellationToken cancellationToken = default);

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

    /// <summary>
    /// Soft-deletes an author by id.
    /// </summary>
    /// <param name="id">The id of the author.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
