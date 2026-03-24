namespace OnlineLibrary.Domain.Models;

/// <summary>
/// Represents the <see cref="BookModel"/> class.
/// </summary>
public class BookModel : AuditModel
{
    /// <summary>
    /// The main title of the book.
    /// </summary>
    public required string Title { get; set; }
    
    /// <summary>
    /// The subtitle of the book.
    /// </summary>
    public string? Subtitle { get; set; }
    
    /// <summary>
    /// The author id of the book.
    /// </summary>
    public required int AuthorId { get; set; }
    
    /// <summary>
    /// The category id of the book.
    /// </summary>
    public required int CategoryId { get; set; }

    /// <summary>
    /// The description of the book.
    /// </summary>
    public string? Description { get; set; }
}
