using System.ComponentModel.DataAnnotations;
using OnlineLibrary.Domain.Enums;

namespace OnlineLibrary.Api.Dtos.BookCollection;

/// <summary>
/// Request body for adding or updating a book inside a collection.
/// </summary>
public class BookCollectionItemRequest
{
    public int BookId { get; set; }

    public BookCollectionItemStatus Status { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Position must be greater than or equal to 0.")]
    public int Position { get; set; }
}
    