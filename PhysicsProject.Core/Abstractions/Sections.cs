using PhysicsProject.Core.Domain;

namespace PhysicsProject.Core.Abstractions;

public interface ISectionProgressRepository
{
    Task<SectionProgress?> GetAsync(Guid userId, Guid sectionId, CancellationToken ct);
    Task SaveAsync(SectionProgress progress, CancellationToken ct);
    Task<SectionTestAttempt?> GetAttemptBySessionAsync(Guid sessionId, CancellationToken ct);
    Task SaveAttemptAsync(SectionTestAttempt attempt, CancellationToken ct);
}

public interface ISectionFlowService
{
    Task<TestSession> StartTrainingSessionAsync(Guid userId, Guid sectionId, CancellationToken ct);
    Task<TestSession> StartTestSessionAsync(Guid userId, Guid sectionId, CancellationToken ct);
    Task RecordTrainingCompletionAsync(Guid userId, Guid sectionId, CancellationToken ct);
    Task RecordTestCompletionAsync(Guid sessionId, bool passed, CancellationToken ct);
    Task ResetTestAttemptsAsync(Guid userId, Guid sectionId, CancellationToken ct);
}
