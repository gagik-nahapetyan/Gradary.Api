using System.ComponentModel.DataAnnotations;

namespace OnlineLibrary.Api.Dtos.Auth;

/// <summary>
/// Represents the request body for login.
/// </summary>
public class LoginRequest
{
    /// <summary>The email address.</summary>
    [EmailAddress]
    public required string Email { get; set; }

    /// <summary>The plain-text password.</summary>
    [MinLength(1)]
    public required string Password { get; set; }
}
