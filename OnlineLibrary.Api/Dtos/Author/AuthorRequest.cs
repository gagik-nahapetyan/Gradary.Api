using System.ComponentModel.DataAnnotations;

namespace OnlineLibrary.Api.Dtos.Author;

/// <summary>
/// Represents the request body for creating and updating an author.
/// </summary>
public class AuthorRequest
{
    /// <summary>
    /// The full name of the author.
    /// </summary>
    [MinLength(1)]
    public required string FullName { get; set; }

    /// <summary>
    /// The biography of the author.
    /// </summary>
    public string? Biography { get; set; }
}
