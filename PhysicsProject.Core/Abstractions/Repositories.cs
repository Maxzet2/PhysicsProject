namespace PhysicsProject.Core.Abstractions;

using PhysicsProject.Core.Domain;

public interface IProblemRepository
{
    Task<ProblemTemplate?> GetTemplateByIdAsync(Guid id, CancellationToken ct);
    Task SaveInstanceAsync(ProblemInstance instance, CancellationToken ct);
    Task<ProblemInstance?> GetInstanceByIdAsync(Guid id, CancellationToken ct);
    Task<IEnumerable<ProblemTemplate>> GetTemplatesAsync(CancellationToken ct);
}

public interface ISessionRepository
{
    Task<TestSession?> GetByIdAsync(Guid id, CancellationToken ct);
    Task SaveAsync(TestSession session, CancellationToken ct);
}


