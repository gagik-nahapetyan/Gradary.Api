using Microsoft.Extensions.DependencyInjection;
using OnlineLibrary.Application.Abstractions.Services;
using OnlineLibrary.Application.Services;
using OnlineLibrary.Application.Abstractions;

namespace OnlineLibrary.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IAuthorService, AuthorService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
