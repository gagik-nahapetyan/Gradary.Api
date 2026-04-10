using OnlineLibrary.Domain.Enums;

namespace OnlineLibrary.Api.Dtos.BookCollection;

/// <summary>
/// Response DTO for a book collection.
/// </summary>
public class BookCollectionDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public BookCollectionStatus Status { get; set; }
    public List<BookCollectionItemDto> Items { get; set; } = [];
}
