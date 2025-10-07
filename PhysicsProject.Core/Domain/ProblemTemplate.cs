namespace PhysicsProject.Core.Domain;

public sealed class ProblemTemplate
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string TemplateType { get; init; } = string.Empty;
    public string JsonSpec { get; init; } = string.Empty;
    public bool IsActive { get; private set; } = true;

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}


