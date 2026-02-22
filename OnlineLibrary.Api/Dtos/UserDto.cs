using OnlineLibrary.Domain.Enums;

namespace OnlineLibrary.Api.Dtos;

/// <summary>
/// Represents the <see cref="UserDto"/> class. Does not include password or password hash.
/// </summary>
public class UserDto
{
    /// <summary>
    /// The id of the user.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The full name of the user.
    /// </summary>
    public required string FullName { get; set; }

    /// <summary>
    /// The email of the user.
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// The role of the user.
    /// </summary>
    public UserRole Role { get; set; }
}
