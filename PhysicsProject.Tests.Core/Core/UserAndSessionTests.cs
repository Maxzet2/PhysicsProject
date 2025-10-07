using Microsoft.Extensions.DependencyInjection;
using PhysicsProject.Core;
using PhysicsProject.Core.Abstractions;
using PhysicsProject.Core.Domain;
using Xunit;

namespace PhysicsProject.Tests.Core;

public sealed class InMemoryUserRepository : IUserRepository
{
    private readonly Dictionary<string, User> _byEmail = new();

    public Task<User?> FindByEmailAsync(string email, CancellationToken ct)
        => Task.FromResult(_byEmail.TryGetValue(email, out var u) ? u : null);

    public Task AddAsync(User user, CancellationToken ct)
    {
        _byEmail[user.Email] = user;
        return Task.CompletedTask;
    }
}

public sealed class InMemorySessionRepository : ISessionRepository
{
    private readonly Dictionary<Guid, TestSession> _store = new();
    public Task<TestSession?> GetByIdAsync(Guid id, CancellationToken ct)
        => Task.FromResult(_store.TryGetValue(id, out var s) ? s : null);
    public Task SaveAsync(TestSession session, CancellationToken ct)
    {
        _store[session.Id] = session;
        return Task.CompletedTask;
    }
}

public class UserAndSessionTests
{
    private ServiceProvider BuildProvider()
    {
        var services = new ServiceCollection();
        services.AddCore();
        services.AddScoped<IUserRepository, InMemoryUserRepository>();
        services.AddScoped<ISessionRepository, InMemorySessionRepository>();
        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task CanRegisterUser()
    {
        var sp = BuildProvider();
        var users = sp.GetRequiredService<IUserService>();
        var ct = CancellationToken.None;
        var user = await users.RegisterAsync("alice@example.com", "Password123", ct);
        Assert.Equal("alice@example.com", user.Email);
        Assert.NotEmpty(user.PasswordHash);
    }

    [Fact]
    public async Task CanCreateSession()
    {
        var sp = BuildProvider();
        var sessions = sp.GetRequiredService<ISessionService>();
        var ct = CancellationToken.None;
        var items = new List<SessionItem>
        {
            new SessionItem { Id = Guid.NewGuid(), InstanceId = Guid.NewGuid(), OrderIndex = 1, MaxScore = 1m }
        };
        var session = await sessions.CreateSessionAsync(Guid.NewGuid(), items, "Normal", ct);
        Assert.Equal("Normal", session.Mode);
        Assert.Single(session.Items);
    }
}


