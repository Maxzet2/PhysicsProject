using PhysicsProject.Core.Abstractions;
using PhysicsProject.Core.Domain;

namespace PhysicsProject.Core.Services;

public sealed class SessionService : ISessionService
{
    private readonly ISessionRepository _sessions;

    public SessionService(ISessionRepository sessions)
    {
        _sessions = sessions;
    }

    public async Task<TestSession> CreateSessionAsync(Guid userId, IReadOnlyList<SessionItem> items, string mode, CancellationToken ct)
    {
        var session = new TestSession
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            StartedAt = DateTimeOffset.UtcNow,
            Items = items,
            Mode = mode
        };
        await _sessions.SaveAsync(session, ct);
        return session;
    }
}


