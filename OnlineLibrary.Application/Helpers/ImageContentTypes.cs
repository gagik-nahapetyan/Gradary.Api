namespace OnlineLibrary.Application.Helpers;

internal static class ImageContentTypes
{
    internal static readonly HashSet<string> Supported =
        ["image/jpeg", "image/png", "image/webp", "image/gif"];

    internal static string GetExtension(string contentType) => contentType.ToLowerInvariant() switch
    {
        "image/jpeg" => ".jpg",
        "image/png"  => ".png",
        "image/webp" => ".webp",
        "image/gif"  => ".gif",
        _            => throw new ArgumentException($"Unsupported image content type: {contentType}")
    };
}
