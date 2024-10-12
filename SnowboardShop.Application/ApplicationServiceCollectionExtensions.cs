using Microsoft.Extensions.DependencyInjection;
using SnowboardShop.Application.Database;
using SnowboardShop.Application.Repositories;

namespace SnowboardShop.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<ISnowboardRepository, SnowboardRepository>();

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