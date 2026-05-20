using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineLibrary.Application.Abstractions.Repositories;
using OnlineLibrary.Application.Abstractions.Services;
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
                .UseSqlServer(configuration["ConnectionStrings:OnlineLibraryDb"])
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
        var provider = configuration["FileStorage:Provider"] ?? "Local";

        if (string.Equals(provider, "AzureBlob", StringComparison.OrdinalIgnoreCase))
        {
            var accountUrl = configuration["FileStorage:AzureBlob:AccountUrl"]
                ?? throw new InvalidOperationException("FileStorage:AzureBlob:AccountUrl is required when provider is AzureBlob");
            var containerName = configuration["FileStorage:AzureBlob:PublicContainerName"]
                ?? throw new InvalidOperationException("FileStorage:AzureBlob:PublicContainerName is required when provider is AzureBlob");

            var serviceClient = new BlobServiceClient(new Uri(accountUrl), new DefaultAzureCredential());
            var containerClient = serviceClient.GetBlobContainerClient(containerName);

            services.AddSingleton(containerClient);
            services.AddScoped<IFileStorageService, AzureBlobFileStorageService>();
        }
        else
        {
            services.AddScoped<IFileStorageService, LocalFileStorageService>();
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
