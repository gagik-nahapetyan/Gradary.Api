namespace OnlineLibrary.Api.Dtos;

/// <summary>
/// Represents the request body for creating or updating an author.
/// </summary>
public class AuthorRequest
{
    /// <summary>
    /// The full name of the author.
    /// </summary>
    public required string FullName { get; set; }

    /// <summary>
    /// The biography of the author.
    /// </summary>
    public string? Biography { get; set; }
}

