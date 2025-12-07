using System.Linq;
using Microsoft.EntityFrameworkCore;
using PhysicsProject.Core.Abstractions;
using PhysicsProject.Core.Domain;
using PhysicsProject.Infrastructure.Persistence.Entities;
using PhysicsProject.Infrastructure.Persistence;

namespace PhysicsProject.Infrastructure.Persistence.Repositories;

public sealed class EfSessionRepository : ISessionRepository
{
    private readonly PhysicsProjectDbContext _dbContext;

    public EfSessionRepository(PhysicsProjectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TestSession?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.TestSessions
            .AsNoTracking()
            .Include(s => s.Items)
            .Include(s => s.Submissions)
            .FirstOrDefaultAsync(s => s.Id == id, ct);

        return entity is null ? null : MapToDomain(entity);
    }

    public async Task SaveAsync(TestSession session, CancellationToken ct)
    {
        var entity = await _dbContext.TestSessions
            .Include(s => s.Items)
            .Include(s => s.Submissions)
            .FirstOrDefaultAsync(s => s.Id == session.Id, ct);

        if (entity is null)
        {
            entity = new TestSessionEntity { Id = session.Id };
            _dbContext.TestSessions.Add(entity);
        }

        entity.UserId = session.UserId;
        entity.SectionId = session.SectionId;
        entity.Mode = session.Mode;
        entity.StartedAt = session.StartedAt;
        entity.FinishedAt = session.FinishedAt;

        entity.Items.Clear();
        foreach (var item in session.Items)
        {
            entity.Items.Add(new SessionItemEntity
            {
                Id = item.Id,
                SessionId = session.Id,
                InstanceId = item.InstanceId,
                OrderIndex = item.OrderIndex,
                MaxScore = item.MaxScore
            });
        }

        entity.Submissions.Clear();
        foreach (var submission in session.Submissions)
        {
            entity.Submissions.Add(new SubmissionEntity
            {
                Id = submission.Id,
                SessionId = session.Id,
                SessionItemId = submission.SessionItemId,
                SubmittedAt = submission.SubmittedAt,
                RawAnswer = submission.RawAnswer,
                IsCorrect = submission.IsCorrect,
                ScoreAwarded = submission.ScoreAwarded,
                Feedback = submission.Feedback
            });
        }

        await _dbContext.SaveChangesAsync(ct);
    }

    private static TestSession MapToDomain(TestSessionEntity entity)
    {
        var items = entity.Items
            .OrderBy(i => i.OrderIndex)
            .Select(i => new SessionItem
            {
                Id = i.Id,
                InstanceId = i.InstanceId,
                OrderIndex = i.OrderIndex,
                MaxScore = i.MaxScore
            })
            .ToList();

        var session = new TestSession
        {
            Id = entity.Id,
            UserId = entity.UserId,
            SectionId = entity.SectionId,
            Mode = entity.Mode,
            StartedAt = entity.StartedAt,
            Items = items
        };

        session.RestoreFinishedAt(entity.FinishedAt);
        session.ReplaceSubmissions(entity.Submissions
            .OrderBy(s => s.SubmittedAt)
            .Select(s => new Submission
            {
                Id = s.Id,
                SessionItemId = s.SessionItemId,
                SubmittedAt = s.SubmittedAt,
                RawAnswer = s.RawAnswer,
                IsCorrect = s.IsCorrect,
                ScoreAwarded = s.ScoreAwarded,
                Feedback = s.Feedback
            }));

        return session;
    }
}
