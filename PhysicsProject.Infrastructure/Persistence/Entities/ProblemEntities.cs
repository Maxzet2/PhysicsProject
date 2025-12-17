namespace PhysicsProject.Infrastructure.Persistence.Entities;

public sealed class ProblemTemplateEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TemplateType { get; set; } = string.Empty;
    public string JsonSpec { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<ProblemInstanceEntity> Instances { get; set; } = new List<ProblemInstanceEntity>();
}

public sealed class ProblemInstanceEntity
{
    public Guid Id { get; set; }
    public Guid TemplateId { get; set; }
    public long Seed { get; set; }
    public string TemplateType { get; set; } = string.Empty;
    public string Statement { get; set; } = string.Empty;
    public string ParametersJson { get; set; } = "{}";
    public string NormalizedCorrectAnswer { get; set; } = string.Empty;

    public ProblemTemplateEntity? Template { get; set; }
}
