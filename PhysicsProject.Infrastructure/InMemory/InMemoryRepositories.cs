using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PhysicsProject.Core.Abstractions;
using PhysicsProject.Core.Domain;

namespace PhysicsProject.Infrastructure.InMemory;

public sealed class InMemoryUserRepository : IUserRepository
{
    private readonly ConcurrentDictionary<string, User> _usersByUserName = new(StringComparer.OrdinalIgnoreCase);

    public Task<User?> FindByUserNameAsync(string userName, CancellationToken ct)
    {
        _usersByUserName.TryGetValue(userName, out var user);
        return Task.FromResult(user);
    }

    public Task AddAsync(User user, CancellationToken ct)
    {
        _usersByUserName[user.UserName] = user;
        return Task.CompletedTask;
    }
}

public sealed class InMemoryProblemRepository : IProblemRepository
{
    private readonly ConcurrentDictionary<Guid, ProblemTemplate> _templates = new();
    private readonly ConcurrentDictionary<Guid, ProblemInstance> _instances = new();

    public Task<ProblemTemplate?> GetTemplateByIdAsync(Guid id, CancellationToken ct)
    {
        _templates.TryGetValue(id, out var template);
        return Task.FromResult(template);
    }

    public Task SaveInstanceAsync(ProblemInstance instance, CancellationToken ct)
    {
        _instances[instance.Id] = instance;
        return Task.CompletedTask;
    }

    public Task<ProblemInstance?> GetInstanceByIdAsync(Guid id, CancellationToken ct)
    {
        _instances.TryGetValue(id, out var instance);
        return Task.FromResult(instance);
    }

    public Task<IEnumerable<ProblemTemplate>> GetTemplatesAsync(CancellationToken ct)
        => Task.FromResult<IEnumerable<ProblemTemplate>>(_templates.Values.ToArray());

    public void SeedTemplates(IEnumerable<ProblemTemplate> templates)
    {
        foreach (var template in templates)
        {
            _templates[template.Id] = template;
        }
    }
}

public sealed class InMemorySessionRepository : ISessionRepository
{
    private readonly ConcurrentDictionary<Guid, TestSession> _sessions = new();

    public Task<TestSession?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        _sessions.TryGetValue(id, out var session);
        return Task.FromResult(session);
    }

    public Task SaveAsync(TestSession session, CancellationToken ct)
    {
        _sessions[session.Id] = session;
        return Task.CompletedTask;
    }
}

