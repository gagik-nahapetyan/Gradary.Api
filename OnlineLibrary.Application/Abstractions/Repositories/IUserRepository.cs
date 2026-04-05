using OnlineLibrary.Domain.Entities;

namespace OnlineLibrary.Application.Abstractions.Repositories;

/// <summary>
/// Represents an abstraction of the user repository.
/// </summary>
public interface IUserRepository : IRepository<User>
{
    /// <summary>
    /// Retrieves a user by email address.
    /// </summary>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
