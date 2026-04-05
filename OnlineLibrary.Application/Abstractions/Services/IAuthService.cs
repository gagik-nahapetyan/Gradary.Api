using OnlineLibrary.Domain.Models;

namespace OnlineLibrary.Application.Abstractions.Services;

/// <summary>
/// Represents an abstraction for authentication operations.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new user with the Member role and returns an access token.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when the email is already taken.</exception>
    Task<AuthResult> RegisterAsync(string fullName, string email, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates credentials and returns an access token.
    /// </summary>
    /// <exception cref="UnauthorizedAccessException">Thrown when credentials are invalid.</exception>
    Task<AuthResult> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
}
