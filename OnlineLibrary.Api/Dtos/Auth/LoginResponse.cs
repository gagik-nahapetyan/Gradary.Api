using OnlineLibrary.Api.Dtos.User;

namespace OnlineLibrary.Api.Dtos.Auth;

/// <summary>
/// Represents the response returned after a successful authentication.
/// </summary>
public class LoginResponse
{
    /// <summary>The JWT access token.</summary>
    public required string AccessToken { get; set; }

    /// <summary>UTC expiry time of the access token.</summary>
    public required DateTime ExpiresAt { get; set; }

    /// <summary>The authenticated user.</summary>
    public required UserDto User { get; set; }
}
