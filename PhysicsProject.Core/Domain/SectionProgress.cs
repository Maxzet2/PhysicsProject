using System;

namespace PhysicsProject.Core.Domain;

public sealed class SectionProgress
{
    public const int MaxTestAttemptsPerCycle = 3;

    private SectionProgress()
    {
    }

    public Guid UserId { get; private set; }
    public Guid SectionId { get; private set; }
    public int AttemptCycle { get; private set; } = 1;
    public int AttemptsUsedInCycle { get; private set; }
    public Guid? ActiveTestSessionId { get; private set; }
    public DateTimeOffset? ActiveTestExpiresAt { get; private set; }
    public DateTimeOffset? LastTrainingCompletedAt { get; private set; }
    public DateTimeOffset? LastTestPassedAt { get; private set; }

    public int AttemptsRemaining => Math.Max(0, MaxTestAttemptsPerCycle - AttemptsUsedInCycle);
    public bool HasAttemptsAvailable => AttemptsRemaining > 0;

    public static SectionProgress CreateNew(Guid userId, Guid sectionId)
        => new()
        {
            UserId = userId,
            SectionId = sectionId,
            AttemptCycle = 1,
            AttemptsUsedInCycle = 0
        };

    public static SectionProgress Rehydrate(
        Guid userId,
        Guid sectionId,
        int attemptCycle,
        int attemptsUsed,
        Guid? activeSessionId,
        DateTimeOffset? activeExpires,
        DateTimeOffset? lastTraining,
        DateTimeOffset? lastPassed)
        => new()
        {
            UserId = userId,
            SectionId = sectionId,
            AttemptCycle = attemptCycle == 0 ? 1 : attemptCycle,
            AttemptsUsedInCycle = attemptsUsed,
            ActiveTestSessionId = activeSessionId,
            ActiveTestExpiresAt = activeExpires,
            LastTrainingCompletedAt = lastTraining,
            LastTestPassedAt = lastPassed
        };

    public void MarkTrainingCompleted(DateTimeOffset timestamp)
    {
        LastTrainingCompletedAt = timestamp;
    }

    public void StartTestAttempt(Guid sessionId, DateTimeOffset expiresAt)
    {
        if (!HasAttemptsAvailable)
        {
            throw new InvalidOperationException("Нет оставшихся попыток. Сбросьте прогресс раздела, чтобы начать заново.");
        }

        AttemptsUsedInCycle += 1;
        ActiveTestSessionId = sessionId;
        ActiveTestExpiresAt = expiresAt;
    }

    public void CompleteTestAttempt(DateTimeOffset completedAt, bool passed)
    {
        ActiveTestSessionId = null;
        ActiveTestExpiresAt = null;

        if (passed)
        {
            LastTestPassedAt = completedAt;
            AttemptsUsedInCycle = 0;
            AttemptCycle += 1;
        }
    }

    public void ResetTestCycle()
    {
        AttemptsUsedInCycle = 0;
        AttemptCycle += 1;
    }
}

public enum SectionTestAttemptStatus
{
    InProgress,
    Passed,
    Failed,
    Expired
}

public sealed class SectionTestAttempt
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public Guid SectionId { get; init; }
    public Guid SessionId { get; init; }
    public int AttemptIndex { get; init; }
    public int AttemptCycle { get; init; }
    public DateTimeOffset StartedAt { get; init; }
    public DateTimeOffset? FinishedAt { get; private set; }
    public SectionTestAttemptStatus Status { get; private set; } = SectionTestAttemptStatus.InProgress;
    public TimeSpan TimeLimit { get; init; }

    public static SectionTestAttempt Rehydrate(
        Guid id,
        Guid userId,
        Guid sectionId,
        Guid sessionId,
        int attemptIndex,
        int attemptCycle,
        DateTimeOffset startedAt,
        TimeSpan timeLimit,
        SectionTestAttemptStatus status,
        DateTimeOffset? finishedAt)
    {
        var attempt = new SectionTestAttempt
        {
            Id = id,
            UserId = userId,
            SectionId = sectionId,
            SessionId = sessionId,
            AttemptIndex = attemptIndex,
            AttemptCycle = attemptCycle,
            StartedAt = startedAt,
            TimeLimit = timeLimit
        };

        if (finishedAt is DateTimeOffset completed)
        {
            switch (status)
            {
                case SectionTestAttemptStatus.Passed:
                    attempt.MarkPassed(completed);
                    break;
                case SectionTestAttemptStatus.Failed:
                    attempt.MarkFailed(completed);
                    break;
                case SectionTestAttemptStatus.Expired:
                    attempt.MarkExpired(completed);
                    break;
            }
        }

        return attempt;
    }

    public void MarkPassed(DateTimeOffset completedAt)
    {
        FinishedAt = completedAt;
        Status = SectionTestAttemptStatus.Passed;
    }

    public void MarkFailed(DateTimeOffset completedAt)
    {
        FinishedAt = completedAt;
        Status = SectionTestAttemptStatus.Failed;
    }

    public void MarkExpired(DateTimeOffset completedAt)
    {
        FinishedAt = completedAt;
        Status = SectionTestAttemptStatus.Expired;
    }
}
