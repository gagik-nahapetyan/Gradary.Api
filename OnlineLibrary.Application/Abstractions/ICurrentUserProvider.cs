namespace OnlineLibrary.Application.Abstractions;

/// <summary>
/// Provides the ID of the currently authenticated user.
/// Returns null when no user is authenticated (e.g. anonymous requests).
/// </summary>
public interface ICurrentUserProvider
{
    int? GetUserId();
}
