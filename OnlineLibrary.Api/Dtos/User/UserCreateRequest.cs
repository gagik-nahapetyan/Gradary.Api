using System.ComponentModel.DataAnnotations;

namespace OnlineLibrary.Api.Dtos.User;

/// <summary>
/// Represents the request body for creating a new user.
/// </summary>
public class UserCreateRequest : UserRequest
{
    /// <summary>
    /// The password for the new account.
    /// </summary>
    [MinLength(1)]
    public required string Password { get; set; }
}
