namespace OnlineLibrary.Domain.Models;

/// <summary>
/// Represents the paginated list of items.
/// </summary>
public class PagedList<TModel> where TModel : class
{
    public List<TModel>? Items { get; set; }

    public int TotalCount { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
}
