using PhysicsProject.Core.Domain;

namespace PhysicsProject.Core.Abstractions;

public interface IChapterRepository
{
    Task<IReadOnlyList<Chapter>> GetAllAsync(CancellationToken ct);
    Task<Section?> GetSectionByIdAsync(Guid sectionId, CancellationToken ct);
}

public interface IStudyPlanService
{
    Task<IReadOnlyList<Chapter>> GetStudyPlanAsync(CancellationToken ct);
}
