using System.ComponentModel.DataAnnotations;

namespace OnlineLibrary.Api.Dtos.User;

/// <summary>
/// Represents the request body for updating a user's password.
/// </summary>
public class UpdatePasswordRequest
{
    /// <summary>
    /// The new password.
    /// </summary>
    [MinLength(1)]
    public required string NewPassword { get; set; }
}
