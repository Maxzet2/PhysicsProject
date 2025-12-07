using Microsoft.EntityFrameworkCore;
using PhysicsProject.Core.Abstractions;
using PhysicsProject.Core.Domain;
using PhysicsProject.Infrastructure.Persistence.Entities;

namespace PhysicsProject.Infrastructure.Persistence.Repositories;

public sealed class EfSectionProgressRepository : ISectionProgressRepository
{
    private readonly PhysicsProjectDbContext _dbContext;

    public EfSectionProgressRepository(PhysicsProjectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SectionProgress?> GetAsync(Guid userId, Guid sectionId, CancellationToken ct)
    {
        var entity = await _dbContext.SectionProgress
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == userId && p.SectionId == sectionId, ct);
        return entity is null ? null : MapProgress(entity);
    }

    public async Task SaveAsync(SectionProgress progress, CancellationToken ct)
    {
        var entity = await _dbContext.SectionProgress
            .FirstOrDefaultAsync(p => p.UserId == progress.UserId && p.SectionId == progress.SectionId, ct);

        if (entity is null)
        {
            entity = new SectionProgressEntity
            {
                UserId = progress.UserId,
                SectionId = progress.SectionId
            };
            _dbContext.SectionProgress.Add(entity);
        }

        entity.AttemptCycle = progress.AttemptCycle;
        entity.AttemptsUsedInCycle = progress.AttemptsUsedInCycle;
        entity.ActiveTestSessionId = progress.ActiveTestSessionId;
        entity.ActiveTestExpiresAt = progress.ActiveTestExpiresAt;
        entity.LastTrainingCompletedAt = progress.LastTrainingCompletedAt;
        entity.LastTestPassedAt = progress.LastTestPassedAt;

        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task<SectionTestAttempt?> GetAttemptBySessionAsync(Guid sessionId, CancellationToken ct)
    {
        var entity = await _dbContext.SectionTestAttempts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.SessionId == sessionId, ct);
        return entity is null ? null : MapAttempt(entity);
    }

    public async Task SaveAttemptAsync(SectionTestAttempt attempt, CancellationToken ct)
    {
        var entity = await _dbContext.SectionTestAttempts
            .FirstOrDefaultAsync(a => a.Id == attempt.Id, ct);

        if (entity is null)
        {
            entity = new SectionTestAttemptEntity
            {
                Id = attempt.Id
            };
            _dbContext.SectionTestAttempts.Add(entity);
        }

        entity.UserId = attempt.UserId;
        entity.SectionId = attempt.SectionId;
        entity.SessionId = attempt.SessionId;
        entity.AttemptIndex = attempt.AttemptIndex;
        entity.AttemptCycle = attempt.AttemptCycle;
        entity.StartedAt = attempt.StartedAt;
        entity.FinishedAt = attempt.FinishedAt;
        entity.Status = (int)attempt.Status;
        entity.TimeLimitSeconds = (int)attempt.TimeLimit.TotalSeconds;

        await _dbContext.SaveChangesAsync(ct);
    }

    private static SectionProgress MapProgress(SectionProgressEntity entity)
        => SectionProgress.Rehydrate(
            entity.UserId,
            entity.SectionId,
            entity.AttemptCycle,
            entity.AttemptsUsedInCycle,
            entity.ActiveTestSessionId,
            entity.ActiveTestExpiresAt,
            entity.LastTrainingCompletedAt,
            entity.LastTestPassedAt);

    private static SectionTestAttempt MapAttempt(SectionTestAttemptEntity entity)
        => SectionTestAttempt.Rehydrate(
            entity.Id,
            entity.UserId,
            entity.SectionId,
            entity.SessionId,
            entity.AttemptIndex,
            entity.AttemptCycle,
            entity.StartedAt,
            TimeSpan.FromSeconds(entity.TimeLimitSeconds),
            (SectionTestAttemptStatus)entity.Status,
            entity.FinishedAt);
}
