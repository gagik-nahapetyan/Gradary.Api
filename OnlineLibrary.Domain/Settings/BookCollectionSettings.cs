namespace OnlineLibrary.Domain.Settings;

/// <summary>
/// Represents the configuration limits for book collections.
/// </summary>
public class BookCollectionSettings
{
    /// <summary>Maximum number of active (NotStarted or InProgress) collections per user.</summary>
    public int MaxActiveCollections { get; set; }

    /// <summary>Maximum number of active (WantToRead or Reading) books per collection.</summary>
    public int MaxActiveBooksPerCollection { get; set; }
}
