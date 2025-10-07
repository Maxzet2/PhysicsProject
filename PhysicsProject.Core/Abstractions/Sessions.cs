using PhysicsProject.Core.Domain;

namespace PhysicsProject.Core.Abstractions;

public interface ISessionService
{
    Task<TestSession> CreateSessionAsync(Guid userId, IReadOnlyList<SessionItem> items, string mode, CancellationToken ct);
}


