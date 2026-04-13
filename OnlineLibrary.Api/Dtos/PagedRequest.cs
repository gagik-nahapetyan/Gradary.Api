using System.ComponentModel.DataAnnotations;
using OnlineLibrary.Domain.Enums;

namespace OnlineLibrary.Api.Dtos;

public record PagedRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Page must be >= 1.")]
    public int Page { get; init; } = 1;

    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100.")]
    public int PageSize { get; init; } = 20;

    public string? OrderBy { get; init; }

    public OrderType OrderType { get; init; } = OrderType.Asc;
}
