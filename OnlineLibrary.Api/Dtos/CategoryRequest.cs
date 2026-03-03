namespace OnlineLibrary.Api.Dtos;

/// <summary>
/// Represents the request body for creating or updating a category.
/// </summary>
public class CategoryRequest
{
    /// <summary>
    /// The name of the category.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// The description of the category.
    /// </summary>
    public string? Description { get; set; }
}

