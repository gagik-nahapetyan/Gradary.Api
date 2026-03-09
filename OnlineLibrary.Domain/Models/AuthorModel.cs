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
}
