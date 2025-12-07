using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhysicsProject.Core.Abstractions;
using PhysicsProject.Infrastructure.Persistence;
using PhysicsProject.Infrastructure.Persistence.Repositories;

namespace PhysicsProject.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? configuration["Database:ConnectionString"];

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Database connection string is not configured.");
        }

        services.AddDbContext<PhysicsProjectDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IUserRepository, EfUserRepository>();
        services.AddScoped<ISessionRepository, EfSessionRepository>();
        services.AddScoped<IProblemRepository, EfProblemRepository>();
        services.AddScoped<IChapterRepository, EfChapterRepository>();
        services.AddScoped<ISectionProgressRepository, EfSectionProgressRepository>();

        services.AddScoped<ProblemTemplateSeeder>();
        services.AddScoped<StudyPlanSeeder>();
        services.AddHostedService<DatabaseInitializer>();

        return services;
    }
}
