using OnlineLibrary.Domain.Enums;

namespace OnlineLibrary.Api.Dtos;

/// <summary>
/// Represents a class containing parameters for pagination.
/// </summary>
public class PageParameters
{
    public int PageSize { get; set; } = 20;
    public int PageNumber { get; set; } = 1;
    public string? OrderBy { get; set; }
    public OrderType OrderType { get; set; } = OrderType.None;
}
