using Microsoft.Extensions.DependencyInjection;
using PhysicsProject.Core.Abstractions;
using PhysicsProject.Core.Services;

namespace PhysicsProject.Core;

public static class CoreServiceCollectionExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddCoreAbstractions();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ISessionService, SessionService>();
        return services;
    }
}
