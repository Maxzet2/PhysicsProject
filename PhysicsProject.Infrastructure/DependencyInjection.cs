using Microsoft.Extensions.DependencyInjection;

namespace PhysicsProject.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Register database, repositories, caching, etc. in future edits.
        return services;
    }
}


