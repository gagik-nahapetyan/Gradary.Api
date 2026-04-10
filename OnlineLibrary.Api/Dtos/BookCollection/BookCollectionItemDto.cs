using OnlineLibrary.Domain.Enums;

namespace OnlineLibrary.Api.Dtos.BookCollection;

/// <summary>
/// Response DTO for a book inside a collection.
/// </summary>
public class BookCollectionItemDto
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public BookCollectionItemStatus Status { get; set; }
    public int Order { get; set; }
}
