using System.ComponentModel.DataAnnotations;
using OnlineLibrary.Domain.Enums;

namespace OnlineLibrary.Api.Dtos.User;

/// <summary>
/// Represents the base request body for user operations.
/// </summary>
public class UserRequest
{
    /// <summary>
    /// The full name of the user.
    /// </summary>
    [MinLength(1)]
    public required string FullName { get; set; }

    /// <summary>
    /// The email of the user.
    /// </summary>
    [MinLength(1)]
    public required string Email { get; set; }

    /// <summary>
    /// The role of the user.
    /// </summary>
    public UserRole Role { get; set; }
}