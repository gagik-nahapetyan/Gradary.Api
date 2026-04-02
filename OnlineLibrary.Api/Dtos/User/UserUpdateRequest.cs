namespace OnlineLibrary.Api.Dtos.User;

/// <summary>
/// Represents the request body for updating an existing user's profile.
/// Password changes use the dedicated PUT /api/users/{id}/password endpoint.
/// </summary>
public class UserUpdateRequest : UserRequest { }
