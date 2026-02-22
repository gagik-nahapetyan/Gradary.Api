using OnlineLibrary.Domain.Enums;

namespace OnlineLibrary.Domain.Entities;

/// <summary>
/// Represents the <see cref="User"/> entity.
/// </summary>
public class User : AuditEntity
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
    /// The password hash of the user.
    /// </summary>
    public required string PasswordHash { get; set; }

    /// <summary>
    /// The role of the user.
    /// </summary>
    public UserRole Role { get; set; }



    /// <summary>
    /// The list of the related BookReviews.
    /// </summary>
    public virtual ICollection<Review> Reviews { get; set; } = [];
}
