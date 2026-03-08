using System;

namespace OnlineLibrary.Api.Dtos;

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

    /// <summary>
    /// The created date of the author record.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The updated date of the author record.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

