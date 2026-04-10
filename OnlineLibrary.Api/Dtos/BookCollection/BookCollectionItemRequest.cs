using OnlineLibrary.Domain.Enums;

namespace OnlineLibrary.Api.Dtos.BookCollection;

/// <summary>
/// Request body for adding or updating a book inside a collection.
/// </summary>
public class BookCollectionItemRequest
{
    public int BookId { get; set; }
    public BookCollectionItemStatus Status { get; set; }
    public int Order { get; set; }
}
