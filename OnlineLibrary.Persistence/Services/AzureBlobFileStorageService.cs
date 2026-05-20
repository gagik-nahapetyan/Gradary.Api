using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using OnlineLibrary.Application.Abstractions.Services;

namespace OnlineLibrary.Persistence.Services;

/// <summary>
/// Stores and retrieves files in an Azure Blob Storage container.
/// Authenticates via Managed Identity (DefaultAzureCredential).
/// </summary>
public class AzureBlobFileStorageService(BlobContainerClient containerClient) : IFileStorageService
{
    public async Task UploadAsync(string key, Stream stream, string contentType, CancellationToken ct = default)
    {
        var blob = containerClient.GetBlobClient(key);
        var headers = new BlobHttpHeaders { ContentType = contentType };

        await blob.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = headers }, ct);
    }

    public async Task<(Stream stream, string contentType)?> DownloadAsync(string key, CancellationToken ct = default)
    {
        var blob = containerClient.GetBlobClient(key);

        if (!await blob.ExistsAsync(ct))
            return null;

        var response = await blob.DownloadStreamingAsync(cancellationToken: ct);

        return (response.Value.Content, response.Value.Details.ContentType);
    }

    public async Task<string?> FindKeyByPrefixAsync(string prefix, CancellationToken ct = default)
    {
        await foreach (var blob in containerClient.GetBlobsAsync(BlobTraits.None, BlobStates.None, prefix, ct))
            return blob.Name;

        return null;
    }

    public async Task DeleteByPrefixAsync(string keyPrefix, CancellationToken ct = default)
    {
        await foreach (var blob in containerClient.GetBlobsAsync(BlobTraits.None, BlobStates.None, keyPrefix, ct))
            await containerClient.DeleteBlobIfExistsAsync(blob.Name, cancellationToken: ct);
    }

    public string? GetPublicUrl(string key) =>
        containerClient.GetBlobClient(key).Uri.ToString();
}
