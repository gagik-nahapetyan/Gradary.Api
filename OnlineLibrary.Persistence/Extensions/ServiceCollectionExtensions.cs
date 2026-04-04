using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineLibrary.Application.Abstractions.Repositories;
using OnlineLibrary.Persistence.Interceptors;
using OnlineLibrary.Persistence.Repositories;

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

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();

        return services;
    }
}
