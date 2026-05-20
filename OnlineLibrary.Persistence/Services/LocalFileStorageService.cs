using OnlineLibrary.Application.Abstractions.Services;

namespace OnlineLibrary.Persistence.Services;

/// <summary>
/// Stores and retrieves files on the local filesystem.
/// Base directory: &lt;app-parent&gt;/LocalStorage
/// </summary>
public class LocalFileStorageService : IFileStorageService
{
    private readonly string _basePath = Path.Combine(
        Directory.GetParent(Environment.CurrentDirectory)!.FullName,
        "LocalStorage");

    public async Task UploadAsync(string key, Stream stream, string contentType, CancellationToken ct = default)
    {
        var path = ToPath(key);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);

        using var fileStream = new FileStream(path, FileMode.Create);
        await stream.CopyToAsync(fileStream, ct);
    }

    public Task<(Stream stream, string contentType)?> DownloadAsync(string key, CancellationToken ct = default)
    {
        var path = ToPath(key);
        if (!File.Exists(path))
            return Task.FromResult<(Stream, string)?>(null);

        var contentType = ContentTypeFromExtension(Path.GetExtension(path));
        
        return Task.FromResult<(Stream, string)?>((File.OpenRead(path), contentType));
    }

    public Task<string?> FindKeyByPrefixAsync(string prefix, CancellationToken ct = default)
    {
        var (dir, filePrefix) = SplitPrefix(prefix);
        var dirPath = ToPath(dir);

        if (!Directory.Exists(dirPath))
            return Task.FromResult<string?>(null);

        var match = Directory.GetFiles(dirPath, $"{filePrefix}*").FirstOrDefault();
        if (match is null)
            return Task.FromResult<string?>(null);

        var fileName = Path.GetFileName(match);
        var key = string.IsNullOrEmpty(dir) ? fileName : $"{dir}/{fileName}";
        
        return Task.FromResult<string?>(key);
    }

    public Task DeleteByPrefixAsync(string keyPrefix, CancellationToken ct = default)
    {
        var (dir, filePrefix) = SplitPrefix(keyPrefix);
        var dirPath = ToPath(dir);

        if (!Directory.Exists(dirPath))
            return Task.CompletedTask;

        foreach (var file in Directory.GetFiles(dirPath, $"{filePrefix}*"))
            File.Delete(file);

        return Task.CompletedTask;
    }

    public string? GetPublicUrl(string key) => null;

    private string ToPath(string key) =>
        Path.Combine(_basePath, key.Replace('/', Path.DirectorySeparatorChar));

    private static (string dir, string filePrefix) SplitPrefix(string prefix)
    {
        var lastSlash = prefix.LastIndexOf('/');
        
        return lastSlash < 0
            ? (string.Empty, prefix)
            : (prefix[..lastSlash], prefix[(lastSlash + 1)..]);
    }

    private static string ContentTypeFromExtension(string ext) => ext.ToLowerInvariant() switch
    {
        ".jpg" or ".jpeg" => "image/jpeg",
        ".png"            => "image/png",
        ".webp"           => "image/webp",
        ".gif"            => "image/gif",
        _                 => "application/octet-stream"
    };
}
