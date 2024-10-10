using Microsoft.Extensions.DependencyInjection;
using SnowboardShop.Application.Repositories;

namespace SnowboardShop.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<ISnowboardRepository, SnowboardRepository>();
        
        return services;
    }
}