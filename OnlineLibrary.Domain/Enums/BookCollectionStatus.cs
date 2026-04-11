namespace OnlineLibrary.Domain.Enums;

/// <summary>
/// Represents the lifecycle status of a <see cref="Entities.BookCollection"/>.
/// </summary>
public enum BookCollectionStatus
{
    NotStarted = 1,
    InProgress = 2,
    Completed = 3,
    Archived = 4
}
