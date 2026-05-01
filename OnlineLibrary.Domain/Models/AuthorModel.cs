namespace OnlineLibrary.Domain.Models;

/// <summary>
/// Represents the <see cref="AuthorModel"/> class.
/// </summary>
public class AuthorModel : AuditModel
{
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
