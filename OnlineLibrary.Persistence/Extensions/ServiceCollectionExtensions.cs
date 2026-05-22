using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineLibrary.Application.Abstractions.Repositories;
using OnlineLibrary.Application.Abstractions.Services;
using OnlineLibrary.Domain.Enums;
using OnlineLibrary.Persistence.Interceptors;
using OnlineLibrary.Persistence.Repositories;
using OnlineLibrary.Persistence.Services;

namespace OnlineLibrary.Persistence.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OnlineLibraryDbContext>(
            (serviceProvider, options) => options
                .UseSqlServer(
                    configuration["ConnectionStrings:OnlineLibraryDb"],
                    sql => sql.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null))
                .AddInterceptors(serviceProvider.GetRequiredService<AuditInterceptor>())
        );

        return services;
    }

    public static IServiceCollection AddDatabaseInterceptors(this IServiceCollection services)
    {
        services.AddScoped<AuditInterceptor>();

        return services;
    }

    public static IServiceCollection AddFileStorage(this IServiceCollection services, IConfiguration configuration)
    {
        var providerValue = configuration["FileStorage:Provider"];
        if (!Enum.TryParse<FileStorageProvider>(providerValue, ignoreCase: true, out var provider))
            provider = FileStorageProvider.Local;

        switch (provider)
        {
            case FileStorageProvider.AzureBlob:
                var accountUrl = configuration["FileStorage:AzureBlob:AccountUrl"]
                    ?? throw new InvalidOperationException("FileStorage:AzureBlob:AccountUrl is required when provider is AzureBlob");
                var containerName = configuration["FileStorage:AzureBlob:PublicContainerName"]
                    ?? throw new InvalidOperationException("FileStorage:AzureBlob:PublicContainerName is required when provider is AzureBlob");

                var serviceClient = new BlobServiceClient(new Uri(accountUrl), new DefaultAzureCredential());
                var containerClient = serviceClient.GetBlobContainerClient(containerName);

                services.AddSingleton(containerClient);
                services.AddScoped<IFileStorageService, AzureBlobFileStorageService>();
                break;

            case FileStorageProvider.Local:
            default:
                services.AddScoped<IFileStorageService, LocalFileStorageService>();
                break;
        }

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IBookCollectionRepository, BookCollectionRepository>();
        services.AddScoped<IBookCollectionItemRepository, BookCollectionItemRepository>();

        return services;
    }
}
