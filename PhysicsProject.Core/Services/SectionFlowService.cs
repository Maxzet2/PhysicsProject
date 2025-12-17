using PhysicsProject.Core.Abstractions;
using PhysicsProject.Core.Domain;

namespace PhysicsProject.Core.Services;

public sealed class SectionFlowService : ISectionFlowService
{
    private readonly IChapterRepository _chapterRepository;
    private readonly ISectionProgressRepository _progressRepository;
    private readonly ISessionService _sessionService;

    public SectionFlowService(
        IChapterRepository chapterRepository,
        ISectionProgressRepository progressRepository,
        ISessionService sessionService)
    {
        _chapterRepository = chapterRepository;
        _progressRepository = progressRepository;
        _sessionService = sessionService;
    }

    public async Task<TestSession> StartTrainingSessionAsync(Guid userId, Guid sectionId, CancellationToken ct)
    {
        var section = await RequireSectionAsync(sectionId, ct);
        var session = await _sessionService.CreateSessionFromTemplateAsync(userId, section.TemplateId, section.DefaultQuestionCount, "training", sectionId, ct);
        return session;
    }

    public async Task<TestSession> StartTestSessionAsync(Guid userId, Guid sectionId, CancellationToken ct)
    {
        var section = await RequireSectionAsync(sectionId, ct);
        var progress = await GetOrCreateProgressAsync(userId, sectionId, ct);
        await EnsureActiveTestStateAsync(progress, ct);
        if (!progress.HasAttemptsAvailable)
        {
            throw new InvalidOperationException("Вы израсходовали 3 попытки. Сбросьте прогресс раздела, чтобы начать заново.");
        }

        var session = await _sessionService.CreateSessionFromTemplateAsync(userId, section.TemplateId, section.DefaultQuestionCount, "test", sectionId, ct);
        var expiresAt = DateTimeOffset.UtcNow.AddSeconds(section.TestTimeLimitSeconds);
        progress.StartTestAttempt(session.Id, expiresAt);

        var attempt = new SectionTestAttempt
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            SectionId = sectionId,
            SessionId = session.Id,
            AttemptIndex = progress.AttemptsUsedInCycle,
            AttemptCycle = progress.AttemptCycle,
            StartedAt = session.StartedAt,
            TimeLimit = TimeSpan.FromSeconds(section.TestTimeLimitSeconds)
        };

        await _progressRepository.SaveAttemptAsync(attempt, ct);
        await _progressRepository.SaveAsync(progress, ct);
        return session;
    }

    public async Task RecordTrainingCompletionAsync(Guid userId, Guid sectionId, CancellationToken ct)
    {
        var progress = await GetOrCreateProgressAsync(userId, sectionId, ct);
        progress.MarkTrainingCompleted(DateTimeOffset.UtcNow);
        await _progressRepository.SaveAsync(progress, ct);
    }

    public async Task RecordTestCompletionAsync(Guid sessionId, bool passed, CancellationToken ct)
    {
        var attempt = await _progressRepository.GetAttemptBySessionAsync(sessionId, ct);
        if (attempt is null)
        {
            return;
        }

        if (attempt.Status != SectionTestAttemptStatus.InProgress)
        {
            return;
        }

        var completionTime = DateTimeOffset.UtcNow;
        if (passed)
        {
            attempt.MarkPassed(completionTime);
        }
        else if (attempt.StartedAt + attempt.TimeLimit < completionTime)
        {
            attempt.MarkExpired(completionTime);
        }
        else
        {
            attempt.MarkFailed(completionTime);
        }

        await _progressRepository.SaveAttemptAsync(attempt, ct);

        var progress = await GetOrCreateProgressAsync(attempt.UserId, attempt.SectionId, ct);
        progress.CompleteTestAttempt(completionTime, passed);
        await _progressRepository.SaveAsync(progress, ct);
    }

    public async Task ResetTestAttemptsAsync(Guid userId, Guid sectionId, CancellationToken ct)
    {
        var progress = await GetOrCreateProgressAsync(userId, sectionId, ct);
        progress.ResetTestCycle();
        await _progressRepository.SaveAsync(progress, ct);
    }

    private async Task<Section> RequireSectionAsync(Guid sectionId, CancellationToken ct)
    {
        var section = await _chapterRepository.GetSectionByIdAsync(sectionId, ct);
        return section ?? throw new InvalidOperationException("Раздел не найден");
    }

    private async Task<SectionProgress> GetOrCreateProgressAsync(Guid userId, Guid sectionId, CancellationToken ct)
    {
        var existing = await _progressRepository.GetAsync(userId, sectionId, ct);
        if (existing is not null)
        {
            return existing;
        }

        var progress = SectionProgress.CreateNew(userId, sectionId);
        await _progressRepository.SaveAsync(progress, ct);
        return progress;
    }

    private async Task EnsureActiveTestStateAsync(SectionProgress progress, CancellationToken ct)
    {
        if (progress.ActiveTestSessionId is null)
        {
            return;
        }

        var now = DateTimeOffset.UtcNow;
        var expiresAt = progress.ActiveTestExpiresAt;
        if (expiresAt is DateTimeOffset activeUntil && activeUntil > now)
        {
            throw new InvalidOperationException("Сначала завершите текущий тест, прежде чем начинать новый.");
        }

        var attempt = await _progressRepository.GetAttemptBySessionAsync(progress.ActiveTestSessionId.Value, ct);
        if (attempt is not null && attempt.Status == SectionTestAttemptStatus.InProgress)
        {
            var expirationMoment = expiresAt ?? now;
            attempt.MarkExpired(expirationMoment);
            await _progressRepository.SaveAttemptAsync(attempt, ct);
        }

        progress.CompleteTestAttempt(now, passed: false);
        await _progressRepository.SaveAsync(progress, ct);
    }
}
