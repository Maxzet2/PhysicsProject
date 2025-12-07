namespace PhysicsProject.Infrastructure.Persistence.Entities;

public sealed class TestSessionEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid? SectionId { get; set; }
    public string Mode { get; set; } = "Normal";
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset? FinishedAt { get; set; }

    public UserEntity? User { get; set; }
    public SectionEntity? Section { get; set; }
    public ICollection<SessionItemEntity> Items { get; set; } = new List<SessionItemEntity>();
    public ICollection<SubmissionEntity> Submissions { get; set; } = new List<SubmissionEntity>();
}

public sealed class SessionItemEntity
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public Guid InstanceId { get; set; }
    public int OrderIndex { get; set; }
    public decimal MaxScore { get; set; }

    public TestSessionEntity? Session { get; set; }
}

public sealed class SubmissionEntity
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public Guid SessionItemId { get; set; }
    public DateTimeOffset SubmittedAt { get; set; }
    public string RawAnswer { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public decimal ScoreAwarded { get; set; }
    public string Feedback { get; set; } = string.Empty;

    public TestSessionEntity? Session { get; set; }
}
