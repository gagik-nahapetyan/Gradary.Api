using Microsoft.Extensions.DependencyInjection;
using OnlineLibrary.Application.Abstractions.Repositories;
using OnlineLibrary.Persistence.Repositories;

namespace OnlineLibrary.Persistence.Extensions;

public static class ServiceCollectionExtensions
{
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
