namespace PhysicsProject.Infrastructure.Persistence.Entities;

public sealed class ChapterEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int OrderIndex { get; set; }

    public ICollection<SectionEntity> Sections { get; set; } = new List<SectionEntity>();
}

public sealed class SectionEntity
{
    public Guid Id { get; set; }
    public Guid ChapterId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
    public Guid TemplateId { get; set; }
    public int DefaultQuestionCount { get; set; }
    public int TestTimeLimitSeconds { get; set; }

    public ChapterEntity? Chapter { get; set; }
    public ProblemTemplateEntity? Template { get; set; }
}
