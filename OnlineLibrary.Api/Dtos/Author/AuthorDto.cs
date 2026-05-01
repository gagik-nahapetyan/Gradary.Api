using System;

namespace OnlineLibrary.Api.Dtos.Author;

/// <summary>
/// Represents the <see cref="AuthorDto"/> class.
/// </summary>
public class AuthorDto
{
    /// <summary>
    /// The id of the author.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The full name of the author.
    /// </summary>
    public required string FullName { get; set; }

    /// <summary>
    /// The biography of the author.
    /// </summary>
    public string? Biography { get; set; }

    /// <summary>URL to the author's photo, or null if no image has been uploaded.</summary>
    public string? ImageUrl { get; set; }
}

