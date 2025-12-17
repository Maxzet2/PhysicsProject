namespace PhysicsProject.Core.Domain;

public sealed class Chapter
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int OrderIndex { get; init; }
    public IReadOnlyList<Section> Sections { get; init; } = Array.Empty<Section>();
}

public sealed class Section
{
    public Guid Id { get; init; }
    public Guid ChapterId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int OrderIndex { get; init; }
    public Guid TemplateId { get; init; }
    public int DefaultQuestionCount { get; init; } = 10;
    public int TestTimeLimitSeconds { get; init; } = 600;
}
