using OnlineLibrary.Domain.Enums;

namespace OnlineLibrary.Api.Dtos.BookCollection;

/// <summary>
/// Request body for creating or updating a book collection.
/// </summary>
public class BookCollectionRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public BookCollectionStatus Status { get; set; }
}
