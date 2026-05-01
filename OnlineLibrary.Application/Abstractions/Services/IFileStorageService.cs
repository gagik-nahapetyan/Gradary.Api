namespace OnlineLibrary.Application.Abstractions.Services;

/// <summary>
/// Represents an abstraction over a file storage backend (local disk, S3, Azure Blob, etc.).
/// Implementations are swappable without changes to the services that depend on this interface.
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Uploads a file to the storage backend under the specified key.
    /// If a file with the same key already exists it will be overwritten.
    /// </summary>
    /// <param name="key">The unique path/key used to identify the file (e.g. "book-images/7.jpg").</param>
    /// <param name="stream">The file content to upload.</param>
    /// <param name="contentType">The MIME type of the file (e.g. "image/jpeg").</param>
    /// <param name="ct">The token to cancel the operation.</param>
    Task UploadAsync(string key, Stream stream, string contentType, CancellationToken ct = default);

    /// <summary>
    /// Downloads the file identified by <paramref name="key"/> and returns its stream and MIME content type.
    /// Returns <c>null</c> if no file exists at the given key.
    /// The caller is responsible for disposing the returned stream.
    /// </summary>
    /// <param name="key">The unique path/key of the file to download.</param>
    /// <param name="ct">The token to cancel the operation.</param>
    /// <returns>
    /// A tuple of the file stream and its MIME content type, or <c>null</c> if the file does not exist.
    /// </returns>
    Task<(Stream stream, string contentType)?> DownloadAsync(string key, CancellationToken ct = default);

    /// <summary>
    /// Returns the full key of the first file whose key starts with <paramref name="prefix"/>,
    /// or <c>null</c> if no matching file exists.
    /// Useful for locating a file when its extension is not known upfront
    /// (e.g. "book-images/7" may match "book-images/7.jpg").
    /// </summary>
    /// <param name="prefix">The key prefix to search for.</param>
    /// <param name="ct">The token to cancel the operation.</param>
    /// <returns>The full key of the first matching file, or <c>null</c> if none found.</returns>
    Task<string?> FindKeyByPrefixAsync(string prefix, CancellationToken ct = default);

    /// <summary>
    /// Deletes all files whose key starts with <paramref name="keyPrefix"/>.
    /// Used to remove an existing image before uploading a replacement in a different format.
    /// Does nothing if no matching files exist.
    /// </summary>
    /// <param name="keyPrefix">The key prefix identifying the files to delete.</param>
    /// <param name="ct">The token to cancel the operation.</param>
    Task DeleteByPrefixAsync(string keyPrefix, CancellationToken ct = default);
}
