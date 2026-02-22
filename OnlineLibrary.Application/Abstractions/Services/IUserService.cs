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
    Task<List<UserModel>> GetAsync();

    /// <summary>
    /// Retrieves the user model by id.
    /// </summary>
    /// <param name="id">The id of the user.</param>
    /// <returns>Task representing an asynchronous operation, wrapping the user model.</returns>
    Task<UserModel> GetByIdAsync(int id);

    /// <summary>
    /// Creates a user.
    /// </summary>
    /// <param name="model">The user model provided.</param>
    /// <returns>Task representing an asynchronous operation, wrapping the user model created.</returns>
    Task<UserModel> CreateAsync(UserModel model);

    /// <summary>
    /// Updates a user.
    /// </summary>
    /// <param name="model">The user model provided.</param>
    Task UpdateAsync(UserModel model);
}
