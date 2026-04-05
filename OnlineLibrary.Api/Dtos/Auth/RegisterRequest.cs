using System.ComponentModel.DataAnnotations;

namespace OnlineLibrary.Api.Dtos.Auth;

/// <summary>
/// Represents the request body for user registration.
/// </summary>
public class RegisterRequest
{
    /// <summary>The full name of the user.</summary>
    [MinLength(1)]
    public required string FullName { get; set; }

    /// <summary>The email address.</summary>
    [EmailAddress]
    public required string Email { get; set; }

    /// <summary>The plain-text password.</summary>
    [MinLength(8)]
    public required string Password { get; set; }
}
