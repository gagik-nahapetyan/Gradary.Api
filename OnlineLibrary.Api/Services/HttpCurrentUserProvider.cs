using System.Security.Claims;
using OnlineLibrary.Application.Abstractions;

namespace OnlineLibrary.Api.Services;

/// <summary>
/// Resolves the current user ID from the active HTTP request's JWT claims.
/// </summary>
public class HttpCurrentUserProvider(IHttpContextAccessor httpContextAccessor) : ICurrentUserProvider
{
    public int? GetUserId()
    {
        var value = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(value, out var id) ? id : null;
    }
}
