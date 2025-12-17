namespace PhysicsProject.Infrastructure.Persistence.Entities;

public sealed class SectionProgressEntity
{
    public Guid UserId { get; set; }
    public Guid SectionId { get; set; }
    public int AttemptCycle { get; set; }
    public int AttemptsUsedInCycle { get; set; }
    public Guid? ActiveTestSessionId { get; set; }
    public DateTimeOffset? ActiveTestExpiresAt { get; set; }
    public DateTimeOffset? LastTrainingCompletedAt { get; set; }
    public DateTimeOffset? LastTestPassedAt { get; set; }

    public UserEntity? User { get; set; }
    public SectionEntity? Section { get; set; }
    public ICollection<SectionTestAttemptEntity> Attempts { get; set; } = new List<SectionTestAttemptEntity>();
}

public sealed class SectionTestAttemptEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid SectionId { get; set; }
    public Guid SessionId { get; set; }
    public int AttemptIndex { get; set; }
    public int AttemptCycle { get; set; }
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset? FinishedAt { get; set; }
    public int Status { get; set; }
    public int TimeLimitSeconds { get; set; }

    public SectionProgressEntity? Progress { get; set; }
}
