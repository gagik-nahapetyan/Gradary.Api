using OnlineLibrary.Domain.Enums;

namespace OnlineLibrary.Domain.Models;

/// <summary>
/// Represents the book collection item model.
/// </summary>
public class BookCollectionItemModel
{
    public int Id { get; set; }
    public int BookCollectionId { get; set; }
    public int BookId { get; set; }
    public string? BookTitle { get; set; }
    public BookCollectionItemStatus Status { get; set; }
    public int Position { get; set; }
}
