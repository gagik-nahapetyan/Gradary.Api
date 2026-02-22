namespace OnlineLibrary.Domain.Entities;

/// <summary>
/// Represents the <see cref="Author"/> entity.
/// </summary>
public class Author : AuditEntity
{
    /// <summary>
    /// The full name of the author.
    /// </summary>
    public required string FullName { get; set; }

    /// <summary>
    /// The biography of the author.
    /// </summary>
    public string? Biography { get; set; }


    /// <summary>
    /// The list of related books.
    /// </summary>
    public virtual ICollection<Book> Books { get; set; } = [];
}
