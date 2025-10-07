namespace PhysicsProject.Core.Domain;

public sealed class ProblemInstance
{
    public Guid Id { get; init; }
    public Guid TemplateId { get; init; }
    public long Seed { get; init; }
    public string Statement { get; init; } = string.Empty;
    public IReadOnlyDictionary<string, object> Parameters { get; init; } = new Dictionary<string, object>();
    public string NormalizedCorrectAnswer { get; init; } = string.Empty;
}


