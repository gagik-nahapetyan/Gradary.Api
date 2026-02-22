using OnlineLibrary.Domain.Enums;

namespace OnlineLibrary.Domain.Models;

/// <summary>
/// Represents the <see cref="UserModel"/> class.
/// </summary>
public class UserModel : AuditModel
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
    /// The password hash of the user. Never expose in API responses.
    /// </summary>
    public required string PasswordHash { get; set; }

    /// <summary>
    /// The role of the user.
    /// </summary>
    public UserRole Role { get; set; }

    /// <summary>
    /// Plain password from request (create or update). Used only in application layer to compute <see cref="PasswordHash"/>; never persisted.
    /// </summary>
    public string? Password { get; set; }
}
