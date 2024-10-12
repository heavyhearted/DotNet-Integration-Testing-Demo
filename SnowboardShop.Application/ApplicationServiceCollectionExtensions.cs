using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SnowboardShop.Application.Database;
using SnowboardShop.Application.Repositories;
using SnowboardShop.Application.Services;

namespace SnowboardShop.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<ISnowboardRepository, SnowboardRepository>();
        services.AddSingleton<ISnowboardService, SnowboardService>();
        services.AddValidatorsFromAssemblyContaining<IApplicationMarker>(ServiceLifetime.Singleton);

        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services,
        string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(_ => 
            new NpgsqlConnectionFactory(connectionString));
        services.AddSingleton<DbInitializer>();
        return services;
    }
}