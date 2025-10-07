namespace PhysicsProject.Core.Domain;

public sealed class TestSession
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string Mode { get; init; } = "Normal";
    public DateTimeOffset StartedAt { get; init; }
    public DateTimeOffset? FinishedAt { get; private set; }
    public IReadOnlyList<SessionItem> Items { get; init; } = Array.Empty<SessionItem>();

    public void Finish() => FinishedAt = DateTimeOffset.UtcNow;
}

public sealed class SessionItem
{
    public Guid Id { get; init; }
    public Guid InstanceId { get; init; }
    public int OrderIndex { get; init; }
    public decimal MaxScore { get; init; }
}


