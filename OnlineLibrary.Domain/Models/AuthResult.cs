namespace OnlineLibrary.Domain.Models;

/// <summary>
/// Represents the result of a successful authentication operation.
/// </summary>
public record AuthResult(string AccessToken, UserModel User);
