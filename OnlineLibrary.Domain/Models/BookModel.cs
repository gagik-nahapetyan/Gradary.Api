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
    /// The full name of the book's author.
    /// </summary>
    public string? AuthorName { get; set; }

    /// <summary>
    /// The category id of the book.
    /// </summary>
    public required int CategoryId { get; set; }

    /// <summary>
    /// The name of the book's category.
    /// </summary>
    public string? CategoryName { get; set; }

    /// <summary>
    /// The description of the book.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>URL to the cover image, or null if no image has been uploaded.</summary>
    public string? ImageUrl { get; set; }
}
