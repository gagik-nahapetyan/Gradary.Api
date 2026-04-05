namespace OnlineLibrary.Application.Abstractions.Services;

/// <summary>
/// Represents an abstraction for password hashing and verification.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashes a plain-text password.
    /// </summary>
    string Hash(string password);

    /// <summary>
    /// Verifies a plain-text password against a stored hash.
    /// </summary>
    bool Verify(string password, string hash);
}