using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Net.Sockets;

namespace PhysicsProject.Infrastructure.Persistence;

public sealed class DatabaseInitializer : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public DatabaseInitializer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        const int maxAttempts = 5;
        var delay = TimeSpan.FromSeconds(5);

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            using var scope = _serviceProvider.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<DatabaseInitializer>>();
            var context = scope.ServiceProvider.GetRequiredService<PhysicsProjectDbContext>();
            var templateSeeder = scope.ServiceProvider.GetRequiredService<ProblemTemplateSeeder>();
            var studyPlanSeeder = scope.ServiceProvider.GetRequiredService<StudyPlanSeeder>();

            try
            {
                await context.Database.MigrateAsync(cancellationToken);
                await templateSeeder.EnsureSeededAsync(cancellationToken);
                await studyPlanSeeder.EnsureSeededAsync(cancellationToken);
                return;
            }
            catch (Exception ex) when (attempt < maxAttempts && IsTransient(ex))
            {
                logger.LogWarning(ex, "Database initialization attempt {Attempt}/{MaxAttempts} failed. Retrying in {Delay}s...", attempt, maxAttempts, delay.TotalSeconds);
                await Task.Delay(delay, cancellationToken);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private static bool IsTransient(Exception ex)
    {
        return ex switch
        {
            NpgsqlException => true,
            SocketException => true,
            TimeoutException => true,
            _ when ex.InnerException is not null => IsTransient(ex.InnerException),
            _ => false
        };
    }
}
