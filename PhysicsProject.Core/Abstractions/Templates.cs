using PhysicsProject.Core.Domain;

namespace PhysicsProject.Core.Abstractions;

public interface ITemplateService
{
    Task<ProblemInstance> GenerateInstanceAsync(Guid templateId, long? seed, GenerationConstraints constraints, CancellationToken ct);
}





