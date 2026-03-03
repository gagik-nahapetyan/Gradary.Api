using Microsoft.Extensions.DependencyInjection;
using OnlineLibrary.Application.Abstractions.Services;
using OnlineLibrary.Application.Services;

namespace OnlineLibrary.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}
