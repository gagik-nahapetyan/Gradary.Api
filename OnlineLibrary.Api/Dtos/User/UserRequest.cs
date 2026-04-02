using OnlineLibrary.Domain.Enums;

namespace OnlineLibrary.Api.Dtos.User;

/// <summary>
/// Represents the request body for creating or updating a user.
/// </summary>
public class UserRequest
{
    /// <summary>
    /// The full name of the user.
    /// </summary>
    public required string FullName { get; set; }

    /// <summary>
    /// The email of the user.
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// The password. Required for create; optional for update (only sent when changing password).
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// The role of the user.
    /// </summary>
    public UserRole Role { get; set; }
}