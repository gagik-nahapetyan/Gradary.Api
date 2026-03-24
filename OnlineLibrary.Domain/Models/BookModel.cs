namespace OnlineLibrary.Domain.Models;

/// <summary>
/// Represents the <see cref="BookModel"/> class.
/// </summary>
public class BookModel : AuditModel
{
    /// <summary>
    /// The main title of the book.
    /// </summary>
    public required string ShortTitle { get; set; }
    
    /// <summary>
    /// The main title with the subtitle of the book.
    /// </summary>
    public string? FullTitle { get; set; }
    
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
