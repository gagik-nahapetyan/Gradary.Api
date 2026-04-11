using OnlineLibrary.Domain.Models;

namespace OnlineLibrary.Application.Abstractions.Services;

/// <summary>
/// Represents an abstraction of a user service.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Retrieves the list of user models.
    /// </summary>
    /// <returns>Task representing an asynchronous operation, wrapping the list of user models.</returns>
    Task<List<UserModel>> GetAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the user model by id.
    /// </summary>
    /// <param name="id">The id of the user.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>Task representing an asynchronous operation, wrapping the user model.</returns>
    Task<UserModel> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a user.
    /// </summary>
    /// <param name="model">The user model provided.</param>
    /// <param name="password">The plain-text password to hash and store.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>Task representing an asynchronous operation, wrapping the user model created.</returns>
    Task<UserModel> CreateAsync(UserModel model, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a user.
    /// </summary>
    /// <param name="model">The user model provided.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    Task UpdateAsync(UserModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the password of a user.
    /// </summary>
    /// <param name="id">The id of the user.</param>
    /// <param name="newPassword">The plain-text new password to hash and store.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <exception cref="KeyNotFoundException">Thrown when no user with the given id exists.</exception>
    Task UpdatePasswordAsync(int id, string newPassword, CancellationToken cancellationToken = default);

    /// <summary>
    /// Soft-deletes a user by id.
    /// </summary>
    /// <param name="id">The id of the user.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
