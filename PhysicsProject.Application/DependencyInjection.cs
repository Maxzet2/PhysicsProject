using Microsoft.Extensions.DependencyInjection;

namespace PhysicsProject.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register CQRS handlers, mediators, use-cases here in future edits.
        return services;
    }
}


