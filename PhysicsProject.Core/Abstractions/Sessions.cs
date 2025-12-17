using PhysicsProject.Core.Domain;

namespace PhysicsProject.Core.Abstractions;

public interface ISessionService
{
    Task<TestSession> CreateSessionAsync(Guid userId, IReadOnlyList<SessionItem> items, string mode, Guid? sectionId, CancellationToken ct);
    Task<TestSession> CreateSessionFromTemplateAsync(Guid userId, Guid templateId, int numItems, string mode, Guid? sectionId, CancellationToken ct);
    Task<SubmissionResult> SubmitAnswerAsync(Guid sessionId, Guid itemId, string answer, CancellationToken ct);
    Task<FinishSessionResult> FinishSessionAsync(Guid sessionId, CancellationToken ct);
}

public sealed record SubmissionResult(bool IsCorrect, decimal ScoreAwarded, string Feedback, int TotalSubmissions, int CorrectSubmissions);

public sealed record FinishSessionResult(decimal TotalScore, int CorrectAnswers, int TotalAnswers);


